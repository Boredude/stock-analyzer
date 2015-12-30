using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using BigData.UI.Client.Events;
using BigData.UI.Client.Infrastructure;
using BigData.UI.Client.Modules.Settings.Models;
using BigDataClient.BL.Stocks;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;

namespace BigData.UI.Client.Modules.Stocks.ViewModels
{
    [Export(typeof(IStocksViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class StocksViewModel : BindableBase, IStocksViewModel, IPartImportsSatisfiedNotification
    {
        #region Data Members

        [Import]
        private IStocksDataManager _stocksDataManager;
        [Import]
        private ISettingsModel _settingsModel;
        [Import]
        private IEventAggregator _eventAggregator;
        [Import]
        private IStocksAnalyzer _stocksAnalyzer;

        private EnhancedObservableCollection<IStockViewModel> _stocks;
        private CancellationTokenSource _cancellationTokenSource;
        private Task _downloadStocksDataTask;
        private Task<IEnumerable<IStock>> _getStocksTask;
        private bool _isBusy;
        private bool _isLoadingStockData;

        #endregion

        #region Ctor

        public StocksViewModel()
        {
            AnalyzeCommand = new DelegateCommand(async () =>
            {
                var result = await ((MetroWindow) Application.Current.MainWindow).ShowMessageAsync
                ("Analyze", 
                 "The operation you are about to perform may take a few minutes. Are you sure you want to continue?",
                 MessageDialogStyle.AffirmativeAndNegative);

                if (result == MessageDialogResult.Affirmative)
                {
                    // TODO: Launch MapReduce
                    _stocksAnalyzer.Analyze(Stocks.Select(s => s.GetStock()), 
                                            _settingsModel.FeaturesToAnalyze,
                                            _settingsModel.NumOfClusters);

                }
            });
        }

        #endregion

        #region Properties

        public EnhancedObservableCollection<IStockViewModel> Stocks
        {
            get { return _stocks; }
            set
            {
                _stocks = value;
                OnPropertyChanged(() => Stocks);

            }
        }

        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value;
                OnPropertyChanged(() => IsBusy);

            }
        }

        public bool IsLoadingStockData
        {
            get { return _isLoadingStockData; }
            set
            {
                _isLoadingStockData = value;
                OnPropertyChanged(() => IsLoadingStockData);
            }
        }

        public DelegateCommand AnalyzeCommand { get; set; }

        #endregion

        #region Methods

        public void OnImportsSatisfied()
        {
            // Subscribe to settings changed event
            _eventAggregator.GetEvent<SettingsChangedEvent>()
                            .Subscribe(async x =>
                            {
                                // if there is a task for downloads stocks data (either in progress or completed)
                                if (_downloadStocksDataTask != null)
                                {
                                    // request cancallation
                                    _cancellationTokenSource.Cancel();
                                    // wait for all background tasks to finish
                                    await Task.WhenAll(_getStocksTask, _downloadStocksDataTask);
                                    // refetch stocks
                                    GetStocks();
                                }
                            });

            // Get stocks
            GetStocks();
        }

        private async void GetStocks()
        {
            // set busy indicator
            IsBusy = true;
            // wait for stocks to be loaded
            _getStocksTask = GetStocksAsync();
            var stocks = await _getStocksTask;
            // set stocks
            SyncStocks(stocks);
            // reset busy indicator
            IsBusy = false;
            // indicate we are loading stock data
            IsLoadingStockData = true;
            // init cancellation token
            _cancellationTokenSource = new CancellationTokenSource();
            // start downloading stock data
            _downloadStocksDataTask = DownloadStocksDataAsync();
            await _downloadStocksDataTask;
            // reset loading indicator
            IsLoadingStockData = false;
        }

        private void SyncStocks(IEnumerable<IStock> stocks)
        {
            // create a factory method
            Func<IStock, IStockViewModel> factoryMethod = stock => new StockViewModel(stock);
            // init stock if the first time
            if (Stocks == null)
            {
                Stocks = new EnhancedObservableCollection<IStockViewModel>(stocks.Select(factoryMethod));
                return;
            }

            // reset data
            Stocks.Clear();
            Stocks.AddRange(stocks.Select(factoryMethod));
        }

        private Task<IEnumerable<IStock>> GetStocksAsync()
        {
            // get stocks
            return Task.Run(() => _stocksDataManager.GetStocks(_settingsModel.NumOfStocks));
        } 

        private Task DownloadStocksDataAsync()
        {
            return Task.Run(() =>
            {
                // Iterate the stocks in parallel
                var opts = new ParallelOptions
                {
                    MaxDegreeOfParallelism = 2,
                    CancellationToken = _cancellationTokenSource.Token
                };

                try
                {
                    Parallel.ForEach(Stocks, opts, stock =>
                    {
                        // exit loop if cancellation was requested
                        opts.CancellationToken
                            .ThrowIfCancellationRequested();
                        // download it's stock data in a blocking matter
                        stock.DownloadStockData();
                    });
                }
                catch (OperationCanceledException ex)
                {
                }
                catch (AggregateException ex)
                {
                }

            }, _cancellationTokenSource.Token);
        }

        #endregion

    }
}

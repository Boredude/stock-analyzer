using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BigData.UI.Client.Events;
using BigData.UI.Client.Infrastructure;
using BigData.UI.Client.Modules.Results.Views;
using BigDataClient.BL.Infrastructure;
using BigDataClient.BL.Stocks;
using Prism.Events;
using Prism.Mvvm;

namespace BigData.UI.Client.Modules.Results.ViewModels
{
    [Export(typeof(IResultsViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class ResultsViewModel : BindableBase, IResultsViewModel, IPartImportsSatisfiedNotification
    {
        #region Data Members

        [Import]
        private IEventAggregator _eventAggregator;
        [Import]
        private IStatusUpdater _statusUpdater;

        private EnhancedObservableCollection<IGrouping<int, IStockAnalysisResult>> _results;
        private bool _hasData;
        private bool _isLoadingData;
        private string _status;
        private string _error;

        #endregion

        #region Ctor

        public ResultsViewModel()
        {

        }

        #endregion

        #region Properties

        public EnhancedObservableCollection<IGrouping<int, IStockAnalysisResult>> Results
        {
            get { return _results; }
            set
            {
                _results = value;
                OnPropertyChanged(() => Results);
            }
        }

        public bool HasData
        {
            get { return _hasData; }
            set
            {
                _hasData = value;
                OnPropertyChanged(() => HasData);
            }
        }

        public bool HasError
        {
            get { return !(string.IsNullOrEmpty(Error)  || Error == string.Empty); }
        }

        public bool IsLoadingData
        {
            get { return _isLoadingData; }
            set
            {
                _isLoadingData = value;
                OnPropertyChanged(() => IsLoadingData);
            }
        }

        public string Status
        {
            get { return _status; }
            set
            {
                _status = value;
                OnPropertyChanged(() => Status);
            }
        }



        public string Error
        {
            get { return _error ?? string.Empty; }
            set
            {
                _error = value;
                OnPropertyChanged(() => Error);
                OnPropertyChanged(() => HasError);
            }
        }

        #endregion

        #region Methods

        public void OnImportsSatisfied()
        {
            // register to stock analysis completed event
            _eventAggregator.GetEvent<StockAnalysisStartedEvent>()
                            .Subscribe(OnStockAnalysisStarted);

            // register to stock analysis completed event
            _eventAggregator.GetEvent<StockAnalysisCompletedEvent>()
                            .Subscribe(OnStockAnalysisCompleted);

            // register to get status updates notifications
            _statusUpdater.StatusChanged += status => Status = status;
        }

        private void OnStockAnalysisStarted(DateTime timestamp)
        {
            // set loading data and error indicators
            IsLoadingData = true;
            HasData = false;
            Error = string.Empty;
        }

        private void OnStockAnalysisCompleted(IStockAnalysisResults args)
        {
            // set data indicators
            IsLoadingData = false;
            HasData = args.IsSuccess;
            Error = args.Error;

            // if anaylze was completed successfuly
            if (args.IsSuccess)
            {
                var clusters = args.Results
                                   .GroupBy(result => result.Cluster);
                // gether the results
                Results = new EnhancedObservableCollection<IGrouping<int, IStockAnalysisResult>>
                                                            (clusters.OrderBy(group => group.Key));
                // update status
                _statusUpdater.UpdateStatus($"Stock analysis completed with {args.EmptyClusters} empty clusters. " +
                                            $"Analysis took {args.Duration} ...");
            }
            else
            {
                // update status
                _statusUpdater.UpdateStatus($"Stock analysis failed. Analysis took {args.Duration} ...");
            }
        } 

        #endregion
    }
}

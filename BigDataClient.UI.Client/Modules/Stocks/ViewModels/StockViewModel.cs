using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using BigData.UI.Client.Infrastructure;
using BigData.UI.Client.Modules.Settings.Models;
using BigDataClient.BL.Stocks;
using Prism.Mvvm;

namespace BigData.UI.Client.Modules.Stocks.ViewModels
{
    [DebuggerDisplay("{Symbol}| {Name}")]
    public class StockViewModel : BindableBase, IStockViewModel
    {
        #region Data Members

        private IStockDataManager _stockDataManager;
        private ISettingsModel _settingsModel;

        private readonly IStock _stock;
        private bool _isLoadingData;
        private bool _hasData;

        #endregion

        #region Ctor

        public StockViewModel(IStock stock)
        {
            _stock = stock;
            _stockDataManager = Composition.Compose<IStockDataManager>();
            _settingsModel = Composition.Compose<ISettingsModel>();
        }

        #endregion

        #region Properties

        public string Symbol
        {
            get { return _stock.Symbol; }
            set
            {
                _stock.Symbol = value;
                OnPropertyChanged(() => Symbol);
            }
        }

        public string Name
        {
            get { return _stock.Name; }
            set
            {
                _stock.Name = value;
                OnPropertyChanged(() => Name);
            }
        }

        public bool IsLoadingData
        {
            get
            {
                return _isLoadingData;
            }
            set
            {
                _isLoadingData = value;
                OnPropertyChanged(() => IsLoadingData);
            }
        }

        public bool HasData
        {
            get
            {
                return _hasData;
            }
            set
            {
                _hasData = value;
                OnPropertyChanged(() => HasData);
            }
        }

        #endregion

        #region Methods

        public void DownloadStockData()
        {
            // set indicators
            Application.Current.Dispatcher.InvokeAsync(() =>
            {
                HasData = false;
                IsLoadingData = true;
            });

            // TODO: remark
            //Task.Delay(800).Wait();

            // get stock tickers
            var tickers = _stockDataManager.GetStockData(Symbol, _settingsModel.DaysToAnalyze)
                                           .ToList();
            // set stock tickers
            _stock.Tickers = tickers;

            // reset indicator
            Application.Current.Dispatcher.InvokeAsync(() =>
            {
                IsLoadingData = false;
                HasData = true;
            });
        }

        public IStock GetStock()
        {
            return _stock;
        }

        public override bool Equals(object obj)
        {
            var stockIdentifier = obj as IStockIdentifier;
            if (stockIdentifier != null)
            {
                return _stock.Equals(stockIdentifier);
            }

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return _stock.GetHashCode();
        }

        #endregion
    }
}

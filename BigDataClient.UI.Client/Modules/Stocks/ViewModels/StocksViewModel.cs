using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BigData.UI.Client.Modules.Settings.Models;
using BigDataClient.BL.Stocks;
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

        private ObservableCollection<IStock> _stocks;
        private bool _isBusy;

        #endregion

        #region Ctor

        public StocksViewModel()
        {

        }

        #endregion

        #region Methods

        public async void OnImportsSatisfied()
        {
            // set busy indicator
            IsBusy = true;
            // wait for stocks to be loaded
            var stocks = await _stocksDataManager.GetStocksAsync(_settingsModel.NumOfStocks);
            // set stocks
            Stocks = new ObservableCollection<IStock>(stocks);
            // reset busy indicator
            IsBusy = false;
        } 

        #endregion

        public ObservableCollection<IStock> Stocks
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
    }
}

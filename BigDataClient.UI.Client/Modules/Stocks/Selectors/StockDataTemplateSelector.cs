using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using BigData.UI.Client.Modules.Stocks.ViewModels;

namespace BigData.UI.Client.Modules.Stocks.Selectors
{
    public class StockDataTemplateSelector : DataTemplateSelector
    {
        #region Properties

        public DataTemplate NoDataTemplate { get; set; }

        public DataTemplate LoadingDataTemplate { get; set; }

        public DataTemplate HasDataTemplate { get; set; }

        #endregion

        #region Methods

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var stockViewModel = item as IStockViewModel;
            if (stockViewModel == null)
                return base.SelectTemplate(item, container);

            // listen on item's property changed event to force refresh on the template selector
            ListenOnPropertyChanged(container, stockViewModel);

            if (stockViewModel.IsLoadingData)
                return LoadingDataTemplate;

            return stockViewModel.HasData ? HasDataTemplate : NoDataTemplate;
        }

        private void ListenOnPropertyChanged(DependencyObject container, IStockViewModel stockViewModel)
        {
            PropertyChangedEventHandler lambda = null;
            lambda = (o, args) =>
            {
                if (args.PropertyName == nameof(stockViewModel.IsLoadingData) ||
                    args.PropertyName == nameof(stockViewModel.HasData))
                {
                    // unregister from property change event because the refresh we are forcing
                    // will add a different listener 
                    stockViewModel.PropertyChanged -= lambda;
                    // hack to force refresh on the template selector
                    var cp = (ContentPresenter) container;
                    cp.ContentTemplateSelector = null;
                    cp.ContentTemplateSelector = this;
                }
            };
            // listen to stockViewModel property changed event
            stockViewModel.PropertyChanged += lambda;
        }

        #endregion
    }
}

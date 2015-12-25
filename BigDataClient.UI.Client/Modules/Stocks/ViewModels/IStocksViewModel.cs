using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BigData.UI.Client.Infrastructure;
using BigDataClient.BL.Stocks;

namespace BigData.UI.Client.Modules.Stocks.ViewModels
{
    public interface IStocksViewModel
    {
        EnhancedObservableCollection<IStockViewModel> Stocks { get; set; } 
    }
}

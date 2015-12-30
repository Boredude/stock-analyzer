using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BigDataClient.BL.Stocks;

namespace BigData.UI.Client.Modules.Stocks.ViewModels
{
    public interface IStockViewModel : IStockIdentifier, INotifyPropertyChanged
    {
        bool IsLoadingData { get; set; }
        bool HasData { get; set; }
        void DownloadStockData();
        IStock GetStock();
    }
}

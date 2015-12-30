using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BigDataClient.BL.Stocks;

namespace BigData.UI.Client.Modules.Settings.Models
{
    public interface ISettingsModel
    {
        int NumOfStocks { get; set; }
        int DaysToAnalyze { get; set; }
        StockPriceType FeaturesToAnalyze { get; set; }
        int NumOfClusters { get; set; }

    }
}

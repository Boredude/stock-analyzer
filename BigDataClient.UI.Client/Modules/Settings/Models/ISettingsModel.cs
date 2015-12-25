using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigData.UI.Client.Modules.Settings.Models
{
    [Flags]
    public enum StockPriceType
    {
        None = 0,
        Open = 1,
        High = 2,
        Low = 4,
        Close = 8
    };

    public interface ISettingsModel
    {
        int NumOfStocks { get; set; }
        int DaysToAnalyze { get; set; }
        StockPriceType FeaturesToAnalyze { get; set; }
        int NumOfClusters { get; set; }

    }
}

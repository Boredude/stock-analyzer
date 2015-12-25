using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BigData.UI.Client.Infrastructure;

namespace BigData.UI.Client.Modules.Settings.Models
{

    [Export(typeof(ISettingsModel))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class SettingsModel : ISettingsModel
    {
        #region Properties 

        public int NumOfStocks
        {
            get { return Properties.Settings.Default.NumOfStocks; }
            set
            {
                Properties.Settings.Default.NumOfStocks = value;
                Properties.Settings.Default.Save();
            }
        }

        public int DaysToAnalyze
        {
            get { return Properties.Settings.Default.DaysToAnalyze; }
            set
            {
                Properties.Settings.Default.DaysToAnalyze = value;
                Properties.Settings.Default.Save();
            }
        }

        public StockPriceType FeaturesToAnalyze
        {
            get
            {
                return (StockPriceType)Enum.Parse(typeof(StockPriceType),
                                                  Properties.Settings.Default.FeaturesToAnalyze);
            }
            set
            {
                Properties.Settings.Default.FeaturesToAnalyze = value.ToString();
                Properties.Settings.Default.Save();
            }
        }

        public int NumOfClusters
        {
            get { return Properties.Settings.Default.NumOfClusters; }
            set
            {
                Properties.Settings.Default.NumOfClusters = value;
                Properties.Settings.Default.Save();
            }
        }

        #endregion

        #region Methods

        public override string ToString()
        {
            if (FeaturesToAnalyze == StockPriceType.None)
            {
                return "You probably want to analyze at least one feature";
            }
            else
            {
                if ((FeaturesToAnalyze & (FeaturesToAnalyze - 1)) != 0) //has more than 1 flag set
                {
                    var featuresToAnalyze = FeaturesToAnalyze.ToString()
                                         .ToLower()
                                         .ReplaceLastOccurrence(",", " and");

                    return $"Analyze {featuresToAnalyze} " +
                           $"features of {NumOfStocks} stocks from the last " +
                           $"{DaysToAnalyze} days | {NumOfClusters} clusters";
                }
                else
                {
                    return $"Analyze {FeaturesToAnalyze.ToString().ToLower()} " +
                           $"feature of {NumOfStocks} stocks from the last " +
                           $"{DaysToAnalyze} days | {NumOfClusters} clusters";
                }
            }
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using BigData.UI.Client.Events;
using BigData.UI.Client.Infrastructure;
using BigDataClient.BL.Stocks;
using Prism.Events;

namespace BigData.UI.Client.Modules.Settings.Models
{

    [Export(typeof(ISettingsModel))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class SettingsModel : ISettingsModel
    {
        #region Data Members

        [Import]
        private IEventAggregator _eventAggregator;
        private readonly Subject<object> _subject;

        #endregion

        #region Ctor

        public SettingsModel()
        {
            // create a new subject to observe
            _subject = new Subject<object>();

            // throttle the subject so it will invoke single SettingsChangedEvent only
            // after 1 second to avoid numerous tight invocation
            _subject.Throttle(TimeSpan.FromMilliseconds(1000))
                    .Subscribe(o => _eventAggregator.GetEvent<SettingsChangedEvent>().Publish(o));
        }

        #endregion

        #region Properties 

        public int NumOfStocks
        {
            get { return Properties.Settings.Default.NumOfStocks; }
            set
            {
                if (Properties.Settings.Default.NumOfStocks == value) return;
                Properties.Settings.Default.NumOfStocks = value;
                Properties.Settings.Default.Save();
                _subject.OnNext(null);
            }
        }

        public int DaysToAnalyze
        {
            get { return Properties.Settings.Default.DaysToAnalyze; }
            set
            {
                if (Properties.Settings.Default.DaysToAnalyze == value) return;
                Properties.Settings.Default.DaysToAnalyze = value;
                Properties.Settings.Default.Save();
                _subject.OnNext(null);
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
                if (Properties.Settings.Default.FeaturesToAnalyze == value.ToString()) return;
                Properties.Settings.Default.FeaturesToAnalyze = value.ToString();
                Properties.Settings.Default.Save();
                _subject.OnNext(null);
            }
        }

        public int NumOfClusters
        {
            get { return Properties.Settings.Default.NumOfClusters; }
            set
            {
                if (Properties.Settings.Default.NumOfClusters == value) return;
                Properties.Settings.Default.NumOfClusters = value;
                Properties.Settings.Default.Save();
                _subject.OnNext(null);
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

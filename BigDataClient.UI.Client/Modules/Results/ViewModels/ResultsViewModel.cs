using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BigData.UI.Client.Events;
using BigData.UI.Client.Modules.Results.Views;
using BigDataClient.BL.Infrastructure;
using BigDataClient.BL.Stocks;
using Prism.Events;

namespace BigData.UI.Client.Modules.Results.ViewModels
{
    [Export(typeof(IResultsViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class ResultsViewModel : IResultsViewModel, IPartImportsSatisfiedNotification
    {
        #region Data Members

        [Import]
        private IEventAggregator _eventAggregator;
        [Import]
        private IStatusUpdater _statusUpdater;

        #endregion

        #region Ctor

        public ResultsViewModel()
        {

        }

        #endregion

        public void OnImportsSatisfied()
        {
            _eventAggregator.GetEvent<StockAnalysisCompletedEvent>()
                            .Subscribe(OnStockAnalysisCompleted);
            
        }

        private void OnStockAnalysisCompleted(IStockAnalysisResults results)
        {
            _statusUpdater.UpdateStatus($"Stock analysis took {results.Duration}");
        }
    }
}

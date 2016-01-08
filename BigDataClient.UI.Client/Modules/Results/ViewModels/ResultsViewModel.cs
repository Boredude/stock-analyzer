using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BigData.UI.Client.Events;
using BigData.UI.Client.Infrastructure;
using BigData.UI.Client.Modules.Results.Views;
using BigDataClient.BL.Infrastructure;
using BigDataClient.BL.Stocks;
using Prism.Events;
using Prism.Mvvm;

namespace BigData.UI.Client.Modules.Results.ViewModels
{
    [Export(typeof(IResultsViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class ResultsViewModel : BindableBase, IResultsViewModel, IPartImportsSatisfiedNotification
    {
        #region Data Members

        [Import]
        private IEventAggregator _eventAggregator;
        [Import]
        private IStatusUpdater _statusUpdater;

        private EnhancedObservableCollection<IGrouping<int, IStockAnalysisResult>> _results;

        #endregion

        #region Ctor

        public ResultsViewModel()
        {

        }

        #endregion

        #region Properties

        public EnhancedObservableCollection<IGrouping<int, IStockAnalysisResult>> Results
        {
            get { return _results; }
            set
            {
                _results = value;
                OnPropertyChanged(() => Results);
            }
        }

        #endregion

        #region Methods

        public void OnImportsSatisfied()
        {
            _eventAggregator.GetEvent<StockAnalysisCompletedEvent>()
                            .Subscribe(OnStockAnalysisCompleted);

        }

        private void OnStockAnalysisCompleted(IStockAnalysisResults args)
        {
            if (args.IsSuccess)
                // gether the results
                Results = new EnhancedObservableCollection<IGrouping<int, IStockAnalysisResult>>
                                                            (args.Results
                                                                 .GroupBy(result => result.Cluster)
                                                                 .OrderBy(group => group.Key));
            // update status
            _statusUpdater.UpdateStatus($"Stock analysis took {args.Duration}");
        } 

        #endregion
    }
}

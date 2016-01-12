using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;

namespace BigData.UI.Client.Infrastructure
{
    public enum StockAnalyzerTab
    {
        Stocks = 0,
        Results = 2
    }

    public interface ITabManager
    {
        void ChangeTab(StockAnalyzerTab tab);
        event Action<StockAnalyzerTab> SelectedTabChanged;
    }

    [Export(typeof(ITabManager))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class TabManager : ITabManager
    {
        #region Events & Delegates

        public event Action<StockAnalyzerTab> SelectedTabChanged;

        #endregion

        #region Methods

        public void ChangeTab(StockAnalyzerTab tab)
        {
            // raise tab canged event
            var handler = SelectedTabChanged;
            handler?.Invoke(tab);
        }

        #endregion


    }
}

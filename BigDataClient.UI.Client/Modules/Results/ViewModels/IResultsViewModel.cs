using System.Linq;
using BigData.UI.Client.Infrastructure;
using BigDataClient.BL.Stocks;

namespace BigData.UI.Client.Modules.Results.ViewModels
{
    public interface IResultsViewModel
    {
        EnhancedObservableCollection<IGrouping<int, IStockAnalysisResult>> Results { get; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigDataClient.BL.Stocks
{
    public interface IStockAnalyzer
    {
        bool IsAnalyzing { get; }
        IStockAnalysisResults Analyze(IEnumerable<IStock> stocks, 
                                      StockPriceType features, 
                                      int clusters, 
                                      Dictionary<string, string> settings);
    }
}

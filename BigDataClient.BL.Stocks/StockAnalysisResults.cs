using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigDataClient.BL.Stocks
{
    public class StockAnalysisResults : IStockAnalysisResults
    {
        #region Ctor

        public StockAnalysisResults()
        {
        }

        #endregion

        #region Properties

        public TimeSpan Duration { get; set; }
        public bool IsSuccess { get; set; }
        public IEnumerable<IStockAnalysisResult> Results { get; set; }

        #endregion

        #region Methods



        #endregion
    }
}

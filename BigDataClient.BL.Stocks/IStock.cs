using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigDataClient.BL.Stocks
{
    public interface IStock : IStockIdentifier
    {
        string MarketCategory { get; set; }
        string TestIssue { get; set; }
        string FinancialStatus { get; set; }
        string RoundLotSize { get; set; }
        IEnumerable<IStockTicker> Tickers { get; set; } 
    }
}

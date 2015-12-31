using System;
using System.Collections.Generic;

namespace BigDataClient.BL.Stocks
{
    public interface IStockAnalysisResults
    {
        TimeSpan Duration { get;}
        bool IsSuccess { get; }
        IEnumerable<IStockAnalysisResult> Results { get; }
    }
}
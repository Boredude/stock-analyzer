﻿using System.Collections.Generic;
using OxyPlot;

namespace BigDataClient.BL.Stocks
{
    public interface IStockAnalysisResult
    {
        int Cluster { get; set; }
        string StockSymbol { get; }
        IEnumerable<DataPoint> OpenTickers { get; }
        IEnumerable<DataPoint> HighTickers { get; }
        IEnumerable<DataPoint> LowTickers { get; }
        IEnumerable<DataPoint> CloseTickers { get; }
    }
}
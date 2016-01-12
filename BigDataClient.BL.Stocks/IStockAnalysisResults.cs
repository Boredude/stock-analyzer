﻿using System;
using System.Collections.Generic;

namespace BigDataClient.BL.Stocks
{
    public interface IStockAnalysisResults
    {
        TimeSpan Duration { get;}
        bool IsSuccess { get; }
        int EmptyClusters { get; }
        IEnumerable<IStockAnalysisResult> Results { get; }
    }
}
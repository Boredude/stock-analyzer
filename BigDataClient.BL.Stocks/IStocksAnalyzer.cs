﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigDataClient.BL.Stocks
{
    public interface IStocksAnalyzer
    {
        void Analyze(IEnumerable<IStock> stocks, StockPriceType features, int clusters);
    }
}

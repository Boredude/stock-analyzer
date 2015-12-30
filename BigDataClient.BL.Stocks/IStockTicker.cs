using System;
using System.Collections.Generic;

namespace BigDataClient.BL.Stocks
{
    public interface IStockTicker
    {
        DateTime Date { get; set; }
        double Open { get; set; }
        double High { get; set; }
        double Low { get; set; }
        double Close { get; set; }
        IEnumerable<double> ToRawValues(StockPriceType features);
    }
}
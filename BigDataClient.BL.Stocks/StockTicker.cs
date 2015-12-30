using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigDataClient.BL.Stocks
{
    [Export(typeof(IStockTicker))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class StockTicker : IStockTicker
    {
        #region Properties

        public DateTime Date { get; set; }
        public double Open { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public double Close { get; set; }

        #endregion

        #region Methods

        public IEnumerable<double> ToRawValues(StockPriceType features)
        {
            var values = new List<double>();

            if (features.HasFlag(StockPriceType.Open))
                values.Add(Open);
            if (features.HasFlag(StockPriceType.High))
                values.Add(High);
            if (features.HasFlag(StockPriceType.Low))
                values.Add(Low);
            if (features.HasFlag(StockPriceType.Close))
                values.Add(Close);

            return values;
        } 

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OxyPlot;

namespace BigDataClient.BL.Stocks
{
    [Export(typeof(IStockAnalysisResult))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class StockAnalysisResult : IStockAnalysisResult
    {
        #region Data Members

        private readonly IStock _stock;

        #endregion

        #region Ctor

        public StockAnalysisResult(IStock stock)
        {
            _stock = stock;
        }

        #endregion

        #region Properties

        public int Cluster { get; set; }

        public string StockSymbol
        {
            get { return _stock.Symbol; }
        }

        public IEnumerable<DataPoint> OpenTickers
        {
            get
            {
                int rollingIndex = 1;
                return _stock.Tickers
                             .ToDictionary(t => rollingIndex++, t => t.Open)
                             .Select(p => new DataPoint(p.Key, p.Value))
                             .ToList();
            }
        }

        public IEnumerable<DataPoint> HighTickers
        {
            get
            {
                int rollingIndex = 1;
                return _stock.Tickers
                             .ToDictionary(t => rollingIndex++, t => t.High)
                             .Select(p => new DataPoint(p.Key, p.Value))
                             .ToList();
            }
        }

        public IEnumerable<DataPoint> LowTickers
        {
            get
            {
                int rollingIndex = 1;
                return _stock.Tickers
                             .ToDictionary(t => rollingIndex++, t => t.Low)
                             .Select(p => new DataPoint(p.Key, p.Value))
                             .ToList();
            }
        }

        public IEnumerable<DataPoint> CloseTickers
        {
            get
            {
                int rollingIndex = 1;
                return _stock.Tickers
                             .ToDictionary(t => rollingIndex++, t => t.Close)
                             .Select(p => new DataPoint(p.Key, p.Value))
                             .ToList();
            }
        }

        #endregion
    }
}

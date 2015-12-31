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

        private IStock _stock;

        #endregion

        #region Ctor

        public StockAnalysisResult(IStock stock)
        {
            _stock = stock;
        }

        #endregion

        #region Properties

        public int Cluster { get; set; }

        public IEnumerable<DataPoint> OpenTickers
        {
            get
            {
                return _stock.Tickers
                             .ToDictionary(t => t.Date.Day, t => t.Open)
                             .Select(p => new DataPoint(p.Key, p.Value));
            }
        }

        public IEnumerable<DataPoint> HighTickers
        {
            get
            {
                return _stock.Tickers
                             .ToDictionary(t => t.Date.Day, t => t.High)
                             .Select(p => new DataPoint(p.Key, p.Value));
            }
        }

        public IEnumerable<DataPoint> LowTickers
        {
            get
            {
                return _stock.Tickers
                             .ToDictionary(t => t.Date.Day, t => t.Low)
                             .Select(p => new DataPoint(p.Key, p.Value));
            }
        }

        public IEnumerable<DataPoint> CloseTickers
        {
            get
            {
                return _stock.Tickers
                             .ToDictionary(t => t.Date.Day, t => t.Close)
                             .Select(p => new DataPoint(p.Key, p.Value));
            }
        }

        #endregion
    }
}

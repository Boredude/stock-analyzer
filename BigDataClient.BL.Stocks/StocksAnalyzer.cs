using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Primitives;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BigDataClient.BL.Infrastructure;
using CsvHelper;

namespace BigDataClient.BL.Stocks
{
    [Export(typeof(IStocksAnalyzer))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class StocksAnalyzer : IStocksAnalyzer
    {
        #region Constants

        private const int STOCKS_PER_FILE = 10;

        #endregion

        #region Data Members

        [Import]
        private IStatusUpdater _statusUpdater;

        private bool _canAnalyze;

        #endregion

        #region Ctor

        public StocksAnalyzer()
        {
            CanAnalyze = true;
        }

        #endregion

        #region Properties

        public bool CanAnalyze
        {
            get { return _canAnalyze; }
            set
            {
                _canAnalyze = value;
                var handler = CanAnalyzeChanged;
                handler?.Invoke(_canAnalyze);
            }
        }

        #endregion

        #region Events & Delegates

        public event Action<bool> CanAnalyzeChanged;

        #endregion

        public void Analyze(IEnumerable<IStock> stocks, StockPriceType features, int clusters)
        {
            // Indicate analyze process is in progress hence cannot be started
            CanAnalyze = false;

            // Don't start analyzing if not stocks are enabled
            if (!stocks.Any()) return;
            //Export the stocks to the input directory
            Export("input", stocks, features);

            // TODO: Continue implementation

            // Indicate analyze process has finished hence can be started
            CanAnalyze = true;
        }

        private void Export(string directory, IEnumerable<IStock> stocks, StockPriceType features)
        {
            _statusUpdater.UpdateStatus("Exporting stocks data to csv ...");

            // create the directory if needed
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            // Get the normalized data per feature, per day, per stock
            var data = from stock in stocks
                       where stock.Tickers
                                  .Any()
                       let rawFeatures = stock.Tickers
                                              .SelectMany(t => t.ToRawValues(features))
                       select new
                       {
                           Symbol = stock.Symbol,
                           NormalizedFeatures = rawFeatures.Normalize()
                       };

            // project an initial index
            int index = 1;

            // export each certain number of stocks to a different size
            data.Chunk(STOCKS_PER_FILE)
                .ToList()
                .ForEach(sublist =>
                {
                    var name = Path.Combine(directory, $"stocks_{index++}.csv");
                    using (var sw = new StreamWriter(name))
                    using (var writer = new CsvWriter(sw))
                    {
                        foreach (var stock in sublist)
                        {
                            writer.WriteField(stock.Symbol);

                            // write each value
                            foreach (var feature in stock.NormalizedFeatures)
                            {
                                writer.WriteField(feature);
                            }

                            //ensure you write end of record when you are using WriteField method
                            writer.NextRecord();
                        }
                    }
                });
        }
    }
}

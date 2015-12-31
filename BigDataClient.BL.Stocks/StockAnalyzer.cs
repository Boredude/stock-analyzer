using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Primitives;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using BigDataClient.BL.Infrastructure;
using CsvHelper;

namespace BigDataClient.BL.Stocks
{
    [Export(typeof(IStockAnalyzer))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class StockAnalyzer : IStockAnalyzer
    {
        #region Constants

        private const int STOCKS_PER_FILE = 10;

        #endregion

        #region Data Members

        [Import]
        private IStatusUpdater _statusUpdater;

        #endregion

        #region Ctor

        public StockAnalyzer()
        {
        }

        #endregion

        #region Properties

        public bool IsAnalyzing { get; private set; }

        #endregion

        public IStockAnalysisResults Analyze(IEnumerable<IStock> stocks, StockPriceType features, int clusters)
        {
            // Indicate analyze process is in progress hence cannot be started
            IsAnalyzing = true;
            // start stopwatch timer
            var stopwatch = Stopwatch.StartNew(); 

            // Don't start analyzing if not stocks are enabled
            if (!stocks.Any()) return new StockAnalysisResults
            {
                Duration = stopwatch.Elapsed,
                IsSuccess = false,
                Results = null
            };

            //Export the stocks to the input directory
            Export("input", stocks, features);

            // TODO: Continue implementation

            // Indicate analyze process has finished hence can be started
            IsAnalyzing = false;
            // stop stopwatch
            stopwatch.Stop();

            return new StockAnalysisResults
            {
                Duration = stopwatch.Elapsed,
                IsSuccess = true,
                Results = null
            };
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

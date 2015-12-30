using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Primitives;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BigData.UI.Client.Infrastructure;
using CsvHelper;

namespace BigDataClient.BL.Stocks
{
    [Export(typeof(IStocksAnalyzer))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class StocksAnalyzer : IStocksAnalyzer
    {
        #region Data Members

        private const int STOCKS_PER_FILE = 10;

        #endregion

        public void Analyze(IEnumerable<IStock> stocks, StockPriceType features, int clusters)
        {
            // Don't start analyzing if not stocks are enabled
            if (!stocks.Any()) return;

            // Get the normalized data per feature, per day, per stock
            var data = from stock in stocks
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
                    using (var sw = new StreamWriter($"stocks_{index++}.csv"))
                    using (var writer = new CsvWriter(sw))
                    {
                        foreach (var stock in sublist)
                        {
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

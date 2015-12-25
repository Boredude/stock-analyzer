using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using CsvHelper;
using RestSharp;

namespace BigDataClient.BL.Stocks
{
    [Export(typeof(IStocksDataManager))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class StocksDataManager : IStocksDataManager
    {
        #region Data Members

        private const string STOCKS_SYMBOLS_FILE_PATH = "BigDataClient.BL.Stocks.stockSymbols.csv";

        #endregion

        #region Methods

        public IEnumerable<IStock> GetStocks(int amount)
        {
            // wait to seconds for dramatic effect
            Task.Delay(TimeSpan.FromSeconds(2)).Wait();

            var stocks = new List<IStock>();

            using (var stream = Assembly.GetExecutingAssembly()
                                        .GetManifestResourceStream(STOCKS_SYMBOLS_FILE_PATH))
            {
                if (stream == null) return stocks;

                // read csv
                using (var reader = new StreamReader(stream))
                using (var csv = new CsvReader(reader))
                {
                    for (int i = 0; i < amount; i++)
                    {
                        // read csv record
                        csv.Read();
                        // add it to the list
                        stocks.Add(csv.GetRecord<Stock>());
                    }

                    reader.Close();
                };

                // close stream
                stream.Close();
            }

            return stocks;
        }

        #endregion
    }
}

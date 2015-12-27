using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using RestSharp;

namespace BigDataClient.BL.Stocks
{
    [Export(typeof(IStockDataManager))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class StockDataManager : IStockDataManager
    {
        #region Data Members

        private const string STOCK_DATA_BASE_URI = "http://ichart.yahoo.com/table.csv";
        private const string STOCK_DATA_SYMBOL_PARAM = "s";
        private const string STOCK_DATA_FROM_MONTH_PARAM = "a";
        private const string STOCK_DATA_FROM_DAYS_PARAM = "b";
        private const string STOCK_DATA_FROM_YEAR_PARAM = "c";
        private const string STOCK_DATA_TO_MONTH_PARAM = "d";
        private const string STOCK_DATA_TO_DAYS_PARAM = "e";
        private const string STOCK_DATA_TO_YEAR_PARAM = "f";
        private const string STOCK_DATA_INTERVAL_PARAM = "g";

        #endregion

        #region Methods

        public IEnumerable<IStockTicker> GetStockData(string symbol, int days)
        {
            try
            {
                // create the REST client
                var client = new RestClient(STOCK_DATA_BASE_URI);
                // calculate the start from date
                var fromDate = DateTime.Now.Subtract(TimeSpan.FromDays(days));
                // create the REST request
                var request = new RestRequest().AddQueryParameter(STOCK_DATA_SYMBOL_PARAM, symbol)
                                               .AddQueryParameter(STOCK_DATA_FROM_MONTH_PARAM, (fromDate.Month - 1).ToString())
                                               .AddQueryParameter(STOCK_DATA_FROM_DAYS_PARAM, fromDate.Day.ToString())
                                               .AddQueryParameter(STOCK_DATA_FROM_YEAR_PARAM, fromDate.Year.ToString())
                                               .AddQueryParameter(STOCK_DATA_TO_MONTH_PARAM, (DateTime.Now.Month - 1).ToString())
                                               .AddQueryParameter(STOCK_DATA_TO_DAYS_PARAM, DateTime.Now.Day.ToString())
                                               .AddQueryParameter(STOCK_DATA_TO_YEAR_PARAM, DateTime.Now.Year.ToString())
                                               .AddQueryParameter(STOCK_DATA_INTERVAL_PARAM, "d");
                // execute request
                var response = client.Get(request);

                using (var textReader = new StringReader(response.Content))
                using (var csvReader = new CsvReader(textReader))
                {
                    return csvReader.GetRecords<StockTicker>()
                                    .ToList();
                }
            }
            catch (Exception ex)
            {
                return new List<IStockTicker>();
            }
            
        } 

        #endregion
    }
}

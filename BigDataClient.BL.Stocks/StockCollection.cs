using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace BigDataClient.BL.Stocks
{
    [Export(typeof(IStockCollection))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class StockCollection : List<IStock>, IStockCollection
    {
        public void Set(IEnumerable<IStock> stocks)
        {
            // remove all items that exists in the current collection but not in the given one
            RemoveAll(s => !stocks.Contains(s));
            // add all the items that does not exists in the  current collection but exists in the given one
            AddRange(stocks.Except(this));
        }
    }
}
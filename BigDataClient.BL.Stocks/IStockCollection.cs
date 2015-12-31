using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BigDataClient.BL.Stocks
{
    public interface IStockCollection : IList<IStock>
    {
        void Set(IEnumerable<IStock> stocks);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigDataClient.BL.Stocks
{
    public interface IStockIdentifier
    {
        string Symbol { get; set; }
        string Name { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigDataClient.BL.Stocks
{
    [Flags]
    public enum StockPriceType
    {
        None = 0,
        Open = 1,
        High = 2,
        Low = 4,
        Close = 8
    };
}

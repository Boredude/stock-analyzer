using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigDataClient.BL.Stocks
{
    public interface IStocksDataManager
    {
        IEnumerable<IStock> GetStocks(int amount);
        Task<IEnumerable<IStock>> GetStocksAsync(int amount);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BigDataClient.BL.Stocks;
using Prism.Events;

namespace BigData.UI.Client.Events
{
    public class StockAnalysisCompletedEvent : PubSubEvent<IStockAnalysisResults>
    {
    }
}

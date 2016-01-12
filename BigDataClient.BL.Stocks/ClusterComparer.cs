using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigDataClient.BL.Stocks
{
    internal class ClusterComparer : IEqualityComparer<IStockAnalysisResult>
    {
        public bool Equals(IStockAnalysisResult x, IStockAnalysisResult y)
        {
            return x.Cluster.Equals(y.Cluster);
        }

        public int GetHashCode(IStockAnalysisResult obj)
        {
            return obj.Cluster.GetHashCode();
        }
    }
}

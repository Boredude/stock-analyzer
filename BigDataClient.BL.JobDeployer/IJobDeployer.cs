using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigDataClient.BL.JobDeployer
{
    public interface IJobDeployer
    {
        void GetOutputFromHostToLocal(string outputLocalPath, string outputHostPathFull);

        void GetOutputFromHdfsToHost(string outputHostPathRelative, string outputHdfsPath);

        void RunJob(string jarHostPath, string inputHdfsPath, string outputHdfsPath);

        void SendInputFromHostToHDFS(string inputHostPath, string inputHdfsPath);

        void SendInputFromLocalToHost(string inputLocalPath, string inputHostPath);

        void ComplieMapReduceOnHost(string sourcesHostPath);

        void PackMapReduceOnHost(string jarName, string classesHostPath);

        void SendMapReduceFromLocalToHost(string jarLocalPath, string jarHostPath);
    }
}

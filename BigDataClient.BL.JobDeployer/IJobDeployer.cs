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
        void Connect(string remoteIp, string username, string password);

        void GetOutputFromHostToLocal(string outputLocalPath, string outputHostPathFull);

        void GetOutputFromHdfsToHost(string outputHostPathRelative, string outputHdfsPath);

        void RunJob(string jarHostPath, string mainClassName, string inputHdfsPath, string outputHdfsPath, string clusters);

        void SendInputFromHostToHDFS(string inputHostPath, string inputHdfsPath);

        void SendInputFromLocalToHost(string inputLocalPath, string inputHostPath);

        void ComplieMapReduceOnHost(string sourcesHostPath);

        void PackMapReduceOnHost(string jarName, string classesHostPath);

        void SendMapReduceFromLocalToHost(string srcLocalPath, string srcHostPath);
    }
}

using System;

namespace BigData.BL.SshCommunication
{
    public interface ISshManager : IDisposable
    {
        void Connect(string remoteIp, string username, string password);
        void Connect();
        void Disconnect();
        ISshHdfsOperations HDFS { get; }
        ISshOperations Host { get; }
        ISshHadoopOperations Hadoop { get; }
    }
}

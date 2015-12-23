using System;

namespace BigData.BL.SshCommunication
{
    public interface ISshManager : IDisposable
    {
        void Connect();
        void Disconnect();
        ISshHdfsOperations HDFS { get; }
        ISshOperations Host { get; }
        ISshHadoopOperations Hadoop { get; }
    }
}

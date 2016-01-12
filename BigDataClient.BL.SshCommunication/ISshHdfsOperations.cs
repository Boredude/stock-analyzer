namespace BigData.BL.SshCommunication
{
    public interface ISshHdfsOperations
    {
        void CreateDirectory(string path);

        void GetFile(string hostPathRelative, string hostPathFull, string hdfsPath, string fileName = "", bool overwrite =true);

        void PutFile(string hostPath, string hdfsPath, string fileName = "", bool overwrite = true);
    }
}
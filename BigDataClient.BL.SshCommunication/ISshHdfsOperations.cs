namespace BigData.BL.SshCommunication
{
    public interface ISshHdfsOperations
    {
        void CreateDirectory(string path);

        void GetFile(string hostPath, string hdfsPath, string fileName = "");

        void PutFile(string hostPath, string hdfsPath, string fileName = "", bool overwrite = true);
    }
}
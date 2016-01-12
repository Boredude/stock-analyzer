namespace BigData.BL.SshCommunication
{
    public interface ISshOperations
    {
        void CreateDirectory(string path);

        void GetFile(string localPath, string targetPath, string fileName);

        void GetDirectory(string localPath, string targetPath, bool overwrite = true);

        void PutFile(string localPath, string targetPath, string fileName);

        void PutDirectory(string localPath, string targetPath, bool overwrite = true);
    }
}

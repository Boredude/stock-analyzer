namespace BigData.BL.SshCommunication
{
    public interface ISshHadoopOperations
    {
        string RunJob(string jarFilePath, string hdfsInputPath, string hdfsOutputPath);
        void Compile(string sourcesHostPath);
        void CreateJar(string jarName, string classesHostPath);
    }
}
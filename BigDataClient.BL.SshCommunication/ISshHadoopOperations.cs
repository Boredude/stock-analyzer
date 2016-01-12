namespace BigData.BL.SshCommunication
{
    public interface ISshHadoopOperations
    {
        string RunJob(string jarFilePath, 
                      string mainClassName, 
                      string hdfsInputPath, 
                      string hdfsOutputPath, 
                      string clusters,
                      bool overwriteOutput = true);
        void Compile(string sourcesHostPath);
        void CreateJar(string jarName, string classesHostPath);
    }
}
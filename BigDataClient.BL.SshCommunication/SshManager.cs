using System;
using System.IO;
using Renci.SshNet;

namespace BigData.BL.SshCommunication
{
    public class SshManager : ISshManager, ISshOperations, ISshHdfsOperations, ISshHadoopOperations
    {
        #region Data Members

        private readonly SshClient sshClient;
        private readonly ScpClient scpClient;

        #endregion

        #region Ctor

        public SshManager(string remoteIp, string username, string password, bool autoConnect = false)
        {
            sshClient = new SshClient(remoteIp, username, password);
            scpClient = new ScpClient(remoteIp, username, password);

            if (autoConnect)
                Connect();
        }

        #endregion

        #region Properties

        public ISshHdfsOperations HDFS
        {
            get { return this; }
        }

        public ISshOperations Host
        {
            get { return this; }
        }

        public ISshHadoopOperations Hadoop
        {
            get { return this; }
        }

        #endregion

        #region Methods

        #region Host

        void ISshOperations.CreateDirectory(string path)
        {
            try
            {
                // connect ssh and scp client
                Connect();
                var command = sshClient.RunCommand(string.Format("mkdir {0}", path));
                // if any error occured
                if (!string.IsNullOrWhiteSpace(command.Error))
                    throw new Exception(command.Error);
            }
            catch (Exception ex)
            {
                throw new Exception("Could not create directory", ex);
            }
        }

        void ISshOperations.GetFile(string localPath, string targetPath, string fileName)
        {
            try
            {
                // connect ssh and scp client
                Connect();
                // create new local file
                var localFile = Path.Combine(localPath, fileName);
                using (var stream = new FileStream(localFile, FileMode.Create))
                {
                    // download file
                    var targetFile = string.Format(@"{0}/{1}", targetPath, fileName);
                    scpClient.Download(targetFile, stream);

                    // close stream
                    stream.Close();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Could not get file from remote machine", ex);
            }
        }

        void ISshOperations.GetDirectory(string localPath, string targetPath)
        {
            try
            {
                // connect ssh and scp client
                Connect();


                DirectoryInfo dirInfo = new DirectoryInfo(localPath);
                // check if directory exists
                if (dirInfo.Exists)
                    throw new InvalidOperationException("local path directory already exists: " + localPath);
                
                // download directory
                scpClient.Download(targetPath, dirInfo);
            }
            catch (Exception ex)
            {
                throw new Exception("Could not get directory from remote machine", ex);
            }
        }

        void ISshOperations.PutFile(string localPath, string targetPath, string fileName)
        {
            try
            {
                // connect ssh and scp client
                Connect();

                // create new local file
                var localFile = Path.Combine(localPath, fileName);
                // if file doesnt exists
                if (!File.Exists(localFile))
                    throw new Exception
                        (string.Format("Cannot find file named {0} in {1}", localPath, fileName));

                using (var stream = new FileStream(localFile, FileMode.Open))
                {
                    // upload file
                    var targetFile = string.Format(@"{0}/{1}", targetPath, fileName);
                    scpClient.Upload(stream, targetPath);

                    // close stream
                    stream.Close();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Could not put file on remote machine", ex);
            }
        }

        void ISshOperations.PutDirectory(string localPath, string targetPath)
        {
            try
            {
                // connect ssh and scp client
                Connect();

                DirectoryInfo dirInfo = new DirectoryInfo(localPath);
                // check if directory exists
                if (!dirInfo.Exists)
                    throw new InvalidOperationException("local path directory doesn't exists: " + localPath);

                // download directory
                scpClient.Upload(dirInfo, targetPath);
            }
            catch (Exception ex)
            {
                throw new Exception("Could not get directory from remote machine", ex);
            }
        }

        #endregion

        #region HDFS

        void ISshHdfsOperations.CreateDirectory(string hdfsPath)
        {
            try
            {
                // connect ssh and scp client
                Connect();
                var command = sshClient.RunCommand(string.Format("hadoop fs -mkdir {0}", hdfsPath));
                // if any error occured
                if (!string.IsNullOrWhiteSpace(command.Error))
                    throw new Exception(command.Error);
            }
            catch (Exception ex)
            {
                throw new Exception("Could not create directory", ex);
            }
        }

        void ISshHdfsOperations.GetFile(string hostPath, string hdfsPath, string fileName)
        {
            try
            {
                // connect ssh and scp client
                Connect();
                var command = sshClient.RunCommand(string.Format("hadoop fs -get {0} {1}", 
                                                                 Path.Combine(hdfsPath, fileName), 
                                                                 hostPath));
                // if any error occured
                if (command.ExitStatus != 0)
                    throw new Exception(command.Error);
            }
            catch (Exception ex)
            {
                throw new Exception("Could not create directory", ex);
            }
        }

        void ISshHdfsOperations.PutFile(string hostPath, string hdfsPath, string fileName)
        {
            try
            {
                // connect ssh and scp client
                Connect();
                var command = sshClient.RunCommand(string.Format("hadoop fs -put {0} {1}",
                                                                 Path.Combine(hostPath, fileName),
                                                                 hdfsPath));
                // if any error occured
                if (command.ExitStatus != 0)
                    throw new Exception(command.Error);
            }
            catch (Exception ex)
            {
                throw new Exception("Could not create directory", ex);
            }
        }

        #endregion

        #region Hadoop

        string ISshHadoopOperations.RunJob(string jarFilePath, string hdfsInputPath, string hdfsOutputPath)
        {
            try
            {
                // connect ssh and scp client
                Connect();
                var command = sshClient.CreateCommand(string.Format("hadoop jar {0} {1} {2}",
                                                                 jarFilePath,
                                                                 hdfsInputPath,
                                                                 hdfsOutputPath));

                // TODO: Convert to async
                command.Execute();

                // if any error occured
                if (command.ExitStatus != 0)
                    throw new Exception(command.Error);

                return command.Result;
            }
            catch (Exception ex)
            {
                throw new Exception("Could not create directory", ex);
            }
        }

        public void Compile(string mapReduceHostPath)
        {
            try
            {
                // connect ssh and scp client
                Connect();

                var command = sshClient.CreateCommand(string.Format("javac -classpath `hadoop classpath` {0}/*.java",
                                                                 mapReduceHostPath));

                command.Execute();

                // if any error occured
                if (command.ExitStatus != 0)
                    throw new Exception(command.Error);
            }
            catch (Exception ex)
            {
                throw new Exception("Could not create directory", ex);
            }
        }

        public void CreateJar(string jarName, string classesHostPath)
        {
            try
            {
                // connect ssh and scp client
                Connect();

                if (!jarName.EndsWith(".jar"))
                    jarName += ".jar";

                var command = sshClient.CreateCommand(string.Format("jar cvf {0} {1}/*.class",
                                                                 jarName, classesHostPath));

                command.Execute();

                // if any error occured
                if (command.ExitStatus != 0)
                    throw new Exception(command.Error);
            }
            catch (Exception ex)
            {
                throw new Exception("Could not create directory", ex);
            }
        }

        #endregion

        public void Dispose()
        {
            Disconnect();
            sshClient.Dispose();
            scpClient.Dispose();
        }

        public void Connect()
        {
            try
            {
                if (!sshClient.IsConnected)
                    sshClient.Connect();

                if (!scpClient.IsConnected)
                    scpClient.Connect();
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to connect to remote peer via ssh or scp", ex);
            }
        }

        public void Disconnect()
        {
            try
            {
                if (sshClient.IsConnected)
                    sshClient.Disconnect();

                if (scpClient.IsConnected)
                    scpClient.Disconnect();
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to disconnect from remote peer via ssh or scp", ex);
            }
        }

        #endregion
    }
}

﻿using System;
using System.ComponentModel.Composition;
using System.Dynamic;
using System.IO;
using Renci.SshNet;

namespace BigData.BL.SshCommunication
{
    [Export(typeof(ISshManager))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class SshManager : ISshManager, ISshOperations, ISshHdfsOperations, ISshHadoopOperations
    {
        #region Data Members

        private SshClient sshClient;
        private ScpClient scpClient;

        #endregion

        #region Ctor

        [ImportingConstructor]
        public SshManager()
        {
        }

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

        void ISshOperations.GetDirectory(string localPath, string targetPath, bool overwrite)
        {
            try
            {
                // connect ssh and scp client
                Connect();

                DirectoryInfo dirInfo = new DirectoryInfo(localPath);
                // check if directory exists
                if (dirInfo.Exists)
                {
                    if (overwrite)
                        Directory.Delete(localPath, true);
                    else
                        throw new InvalidOperationException("local path directory already exists: " + localPath);
                }

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

        void ISshOperations.PutDirectory(string localPath, string targetPath, bool overwrite)
        {
            try
            {
                // connect ssh and scp client
                Connect();

                DirectoryInfo dirInfo = new DirectoryInfo(localPath);
                // check if directory exists
                if (!dirInfo.Exists)
                    throw new InvalidOperationException("local path directory doesn't exists: " + localPath);

                if (overwrite)
                    // delete directory if exists
                    sshClient.RunCommand(string.Format("rm -rf {0}", targetPath));
                
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

        void ISshHdfsOperations.GetFile(string hostPathRelative, 
                                        string hostPathFull, 
                                        string hdfsPath, 
                                        string fileName,
                                        bool overwrite)
        {
            try
            {
                // connect ssh and scp client
                Connect();

                if (overwrite)
                    // delete directory if exists
                    sshClient.RunCommand(string.Format("rm -rf {0}", hostPathFull));

                // run get command
                var command = sshClient.RunCommand(string.Format("hadoop fs -get {0} {1}", 
                                                                 Path.Combine(hdfsPath, fileName), 
                                                                 hostPathRelative));
                // if any error occured
                if (command.ExitStatus != 0)
                    throw new Exception(command.Error);
            }
            catch (Exception ex)
            {
                throw new Exception("Could not create directory", ex);
            }
        }

        void ISshHdfsOperations.PutFile(string hostPath, string hdfsPath, string fileName, bool overwrite)
        {
            try
            {
                // connect ssh and scp client
                Connect();

                if (overwrite)
                    // remove files if exists
                    sshClient.RunCommand(string.Format("hadoop fs -rm -r {0}", hdfsPath));
                // put files on hdfs
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

        string ISshHadoopOperations.RunJob(string jarFilePath, 
                                           string mainClassName,
                                           string hdfsInputPath, 
                                           string hdfsOutputPath,
                                           string clusters, 
                                           bool overwriteOutput)
        {
            try
            {
                // connect ssh and scp client
                Connect();

                // delete output folder if needed
                if (overwriteOutput)
                    sshClient.CreateCommand(string.Format("hadoop fs -rm -r {0}", hdfsOutputPath));

                // run job
                var command = sshClient.CreateCommand(string.Format("hadoop jar {0} {1} {2} {3} {4}",
                                                                 jarFilePath,
                                                                 mainClassName,
                                                                 hdfsInputPath,
                                                                 hdfsOutputPath,
                                                                 clusters));

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

                // create directory for output classes
                sshClient.RunCommand(string.Format("mkdir {0}/classes", mapReduceHostPath));
                // run complie command
                var command = sshClient.CreateCommand(string.Format("javac -classpath `hadoop classpath` -d {0}/classes {0}/*.java",
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

                var command = sshClient.CreateCommand(string.Format("jar -cvf {1}/{0} -C {1}/classes/ .",
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

        public void Connect(string remoteIp, string username, string password)
        {
            sshClient = new SshClient(remoteIp, username, password);
            scpClient = new ScpClient(remoteIp, username, password);

            Connect();
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

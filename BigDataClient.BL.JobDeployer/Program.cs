using System;
using System.Diagnostics;
using System.IO;
using BigData.BL.SshCommunication;

namespace BigDataClient.BL.JobDeployer
{
    class Program
    {
        static void Main(string[] args)
        {
            using (ISshManager manager = new SshManager(Settings.Default.HostIP,
                                                        Settings.Default.HostUserName,
                                                        Settings.Default.HostPassword,
                                                        autoConnect: true))
            {
                SendJarToHost(manager);
                SendInputToHost(manager);
                SendInputToHDFS(manager);
                RunJob(manager);
                GetOutputFromHDFS(manager);
                GetOutputFromHost(manager);
                ShowResults();
            }
        }

        private static void ShowResults()
        {
            Console.Write("Opening results file ... ");

            // Opening results file
            Process.Start(Path.Combine(Environment.CurrentDirectory, 
                                       Settings.Default.OutputLocalPath,
                                       "part-r-00000"));

            Console.WriteLine("Done!");
        }

        private static void GetOutputFromHost(ISshManager manager)
        {
            Console.Write("Importing results from host machine to local machine ... ");

            // get output from HDFS to host machine
            manager.Host
                   .GetDirectory(Settings.Default.OutputLocalPath, Settings.Default.OutputHostPathFull);

            Console.WriteLine("Done!");
        }

        private static void GetOutputFromHDFS(ISshManager manager)
        {
            Console.Write("Gathering results from HDFS to host machine ... ");

            // get output from HDFS to host machine
            manager.HDFS.GetFile(Settings.Default.OutputHostPathRelative, Settings.Default.OutputHdfsPath);

            Console.WriteLine("Done!");
        }

        private static void RunJob(ISshManager manager)
        {
            Console.Write("Running job on hadoop ... ");

            // send jar file to host machine
            // put jar file on remote machine
            string result = manager.Hadoop
                                   .RunJob(Settings.Default.JarHostPath, Settings.Default.InputHdfsPath, Settings.Default.OutputHdfsPath);

            // print result
            Console.WriteLine("-------------------------------------------------------");
            Console.WriteLine();
            Console.WriteLine(result);
            Console.WriteLine();
            Console.WriteLine("-------------------------------------------------------");

            Console.WriteLine("Done!");
        }

        private static void SendInputToHDFS(ISshManager manager)
        {
            Console.Write("Upload input directory from host machine to HDFS ... ");

            // send jar file to host machine
            // put jar file on remote machine
            manager.HDFS.PutFile(Settings.Default.InputHostPath, Settings.Default.InputHdfsPath);

            Console.WriteLine("Done!");
        }

        private static void SendInputToHost(ISshManager manager)
        {
            Console.Write("Send local input files to host machine ... ");

            // check if directory exists
            if (!Directory.Exists(Settings.Default.InputLocalPath))
                throw new DirectoryNotFoundException("Local input path was not found.");

            // upload directory to remote host
            manager.Host.PutDirectory(Settings.Default.InputLocalPath, Settings.Default.InputHostPath);

            Console.WriteLine("Done!");
        }

        private static void SendJarToHost(ISshManager manager)
        {
            Console.Write("Send jar file to host machine ... ");

            // send jar file to host machine
            var jarFileInfo = new FileInfo(Settings.Default.JarLocalPath);
            if (!jarFileInfo.Exists)
                throw new FileNotFoundException("Jar file was not found");

            // put jar file on remote machine
            manager.Host.PutFile(jarFileInfo.DirectoryName, Settings.Default.JarHostPath, jarFileInfo.Name);

            Console.WriteLine("Done!");
        }
    }
}

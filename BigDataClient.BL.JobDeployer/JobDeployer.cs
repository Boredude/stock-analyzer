using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BigData.BL.SshCommunication;

namespace BigDataClient.BL.JobDeployer
{
    public class JobDeployer : IJobDeployer
    {
        #region Data Members

        private readonly ISshManager _sshManager;

        #endregion

        #region Ctor

        public JobDeployer(ISshManager sshManager)
        {
            _sshManager = sshManager;
        }

        #endregion

        public void GetOutputFromHostToLocal(string outputLocalPath, string outputHostPathFull)
        {
            Console.Write("Importing results from host machine to local machine ... ");

            // get output from HDFS to host machine
            _sshManager.Host
                       .GetDirectory(outputLocalPath, outputHostPathFull);

            Console.WriteLine("Done!");
        }

        public void GetOutputFromHdfsToHost(string outputHostPathRelative, string outputHdfsPath)
        {
            Console.Write("Gathering results from HDFS to host machine ... ");

            // get output from HDFS to host machine
            _sshManager.HDFS
                       .GetFile(outputHostPathRelative, outputHdfsPath);

            Console.WriteLine("Done!");
        }

        public void RunJob(string jarHostPath, string inputHdfsPath, string outputHdfsPath)
        {
            Console.Write("Running job on hadoop ... ");

            // send jar file to host machine
            // put jar file on remote machine
            string result = _sshManager.Hadoop
                                       .RunJob(jarHostPath, inputHdfsPath, outputHdfsPath);

            // print result
            Console.WriteLine("-------------------------------------------------------");
            Console.WriteLine();
            Console.WriteLine(result);
            Console.WriteLine();
            Console.WriteLine("-------------------------------------------------------");

            Console.WriteLine("Done!");
        }

        public void SendInputFromHostToHDFS(string inputHostPath, string inputHdfsPath)
        {
            Console.Write("Upload input directory from host machine to HDFS ... ");

            // send jar file to host machine
            // put jar file on remote machine
            _sshManager.HDFS.PutFile(inputHostPath, inputHdfsPath);

            Console.WriteLine("Done!");
        }

        public void SendInputFromLocalToHost(string inputLocalPath, string inputHostPath)
        {
            Console.Write("Send local input files to host machine ... ");

            // check if directory exists
            if (!Directory.Exists(inputLocalPath))
                throw new DirectoryNotFoundException("Local input path was not found.");

            // upload directory to remote host
            _sshManager.Host.PutDirectory(inputLocalPath, inputHostPath);

            Console.WriteLine("Done!");
        }

        public void PackMapReduceOnHost(string jarName, string classesHostPath)
        {
            Console.Write("Pack mapReduce to jar on host machine ... ");

            // send jar file to host machine
            // put jar file on remote machine
            _sshManager.Hadoop
                       .CreateJar(jarName, classesHostPath);

            // print result
            Console.WriteLine("Done!");
        }

        public void ComplieMapReduceOnHost(string sourcesHostPath)
        {
            Console.Write("Compliling mapReduce on host machine ... ");

            // send jar file to host machine
            // put jar file on remote machine
            _sshManager.Hadoop
                       .Compile(sourcesHostPath);

            // print result
            Console.WriteLine("Done!");
        }

        public void SendMapReduceFromLocalToHost(string jarLocalPath, string jarHostPath)
        {
            Console.Write("Send jar file to host machine ... ");

            // send jar file to host machine
            var jarFileInfo = new FileInfo(jarLocalPath);
            if (!jarFileInfo.Exists)
                throw new FileNotFoundException("Jar file was not found");

            // put jar file on remote machine
            _sshManager.Host.PutFile(jarFileInfo.DirectoryName, jarHostPath, jarFileInfo.Name);

            Console.WriteLine("Done!");
        }
    }
}

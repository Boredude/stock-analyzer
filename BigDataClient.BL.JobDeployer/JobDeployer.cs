using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BigData.BL.SshCommunication;

namespace BigDataClient.BL.JobDeployer
{
    [Export(typeof(IJobDeployer))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class JobDeployer : IJobDeployer
    {
        #region Data Members

        [Import]
        private ISshManager _sshManager;

        #endregion

        #region Ctor

        public JobDeployer()
        {
        }

        #endregion

        public void Connect(string remoteIp, string username, string password)
        {
            _sshManager.Connect(remoteIp, username, password);
        }

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

        public void RunJob(string jarHostPath, string mainClassName, string inputHdfsPath, string outputHdfsPath, string clusters)
        {
            Console.Write("Running job on hadoop ... ");

            // send jar file to host machine
            // put jar file on remote machine
            string result = _sshManager.Hadoop
                                       .RunJob(jarHostPath, mainClassName, inputHdfsPath,  outputHdfsPath, clusters);

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

            // send input folder to hdfs
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

        public void SendMapReduceFromLocalToHost(string srcLocalPath, string srcHostPath)
        {
            Console.Write("Send map reduce source files to host machine ... ");

            // put source file on remote machine
            _sshManager.Host.PutDirectory(srcLocalPath, srcHostPath);

            Console.WriteLine("Done!");
        }
    }
}

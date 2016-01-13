using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Primitives;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using BigDataClient.BL.Infrastructure;
using BigDataClient.BL.JobDeployer;
using CsvHelper;

namespace BigDataClient.BL.Stocks
{
    [Export(typeof(IStockAnalyzer))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class StockAnalyzer : IStockAnalyzer
    {
        #region Constants

        private const int STOCKS_PER_FILE = 10;

        #endregion

        #region Data Members

        [Import]
        private IStatusUpdater _statusUpdater;

        [Import]
        private IJobDeployer _jobDeployer;

        #endregion

        #region Ctor

        public StockAnalyzer()
        {
        }

        #endregion

        #region Properties

        public bool IsAnalyzing { get; private set; }

        #endregion

        #region Methods

        public void Prepare()
        {
            // prepare by setting indicator
            IsAnalyzing = true;
        }

        public IStockAnalysisResults Analyze(IEnumerable<IStock> stocks,
                                     StockPriceType features,
                                     int clusters,
                                     Dictionary<string, string> settings)
        {
            // Don't start analyzing if not stocks are enabled
            if (!stocks.Any())
                return new StockAnalysisResults
                {
                    Duration = new TimeSpan(),
                    IsSuccess = false,
                    Error = "No stocks to analyze",
                    Results = null
                };

            // Indicate analyze process is in progress hence cannot be started
            IsAnalyzing = true;

            // start stopwatch timer
            var stopwatch = Stopwatch.StartNew();

            // begin alayzing process
            bool isSuccess = true;
            string error = string.Empty;
            IEnumerable<IStockAnalysisResult> results = default(IEnumerable<IStockAnalysisResult>);

            try
            {
                //Export the stocks to the input directory
                Export("input", stocks, features);
                // initate map reduce process
                var resultsFile = LaunchMapReduce(settings, clusters);
                // get clustering results
                var clusteringResults = GetClusteringResults(resultsFile);
                // set results
                results = stocks.Where(stock => clusteringResults.ContainsKey(stock.Symbol))
                                .Select(stock => new StockAnalysisResult(stock)
                                {
                                    Cluster = clusteringResults[stock.Symbol]
                                });

            }
            catch (Exception ex)
            {
                isSuccess = false;
                error = ex?.InnerException?.Message ?? ex?.Message;
            }
            finally
            {
                // Indicate analyze process has finished hence can be started
                IsAnalyzing = false;
                // stop stopwatch
                stopwatch.Stop();
            }

            return new StockAnalysisResults
            {
                Duration = stopwatch.Elapsed,
                IsSuccess = isSuccess,
                Error = error,
                Results = results,
                EmptyClusters = clusters - results?.Distinct(new ClusterComparer()).Count() ?? clusters
            };
        }

        private void Export(string directory, IEnumerable<IStock> stocks, StockPriceType features)
        {
            _statusUpdater.UpdateStatus("Exporting stocks data to csv ...");

            // create the directory if needed
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            // Get the normalized data per feature, per day, per stock
            var data = from stock in stocks
                       where stock.Tickers
                                  .Any()
                       let rawFeatures = stock.Tickers
                                              .SelectMany(t => t.ToRawValues(features))
                       select new
                       {
                           Symbol = stock.Symbol,
                           NormalizedFeatures = rawFeatures.Normalize()
                       };

            // project an initial index
            int index = 1;

            // export each certain number of stocks to a different size
            data.Chunk(STOCKS_PER_FILE)
                .ToList()
                .ForEach(sublist =>
                {
                    var name = Path.Combine(directory, $"stocks_{index++}.csv");
                    using (var sw = new StreamWriter(name))
                    using (var writer = new CsvWriter(sw))
                    {
                        foreach (var stock in sublist)
                        {
                            writer.WriteField(stock.Symbol);

                            // write each value
                            foreach (var feature in stock.NormalizedFeatures)
                            {
                                writer.WriteField(feature);
                            }

                            //ensure you write end of record when you are using WriteField method
                            writer.NextRecord();
                        }
                    }
                });
        }

        private string LaunchMapReduce(Dictionary<string, string> settings, int clusters)
        {
            _statusUpdater.UpdateStatus("Connecting to host machine ...");
            // connect to remote host
            _jobDeployer.Connect(settings[SettingsKeys.HostIP],
                                 settings[SettingsKeys.HostUsername], 
                                 settings[SettingsKeys.HostPassword]);

            _statusUpdater.UpdateStatus("Uploading MapReduce source files to host machine ...");
            // send map reduce source files to host mahine
            _jobDeployer.SendMapReduceFromLocalToHost(settings[SettingsKeys.SrcLocalPath],
                                                      settings[SettingsKeys.SrcHostPath]);

            _statusUpdater.UpdateStatus("Compiling MapReduce source files ...");
            // complie map reduce job
            _jobDeployer.ComplieMapReduceOnHost(settings[SettingsKeys.SrcHostPath]);

            _statusUpdater.UpdateStatus("Packing (jaring) MapReduce source files ...");
            // pack (jar) the map reduce classes
            _jobDeployer.PackMapReduceOnHost(settings[SettingsKeys.JarName], 
                                             settings[SettingsKeys.SrcHostPath]);

            _statusUpdater.UpdateStatus("Uploading input files to host machine ...");
            // send input files to host machine
            _jobDeployer.SendInputFromLocalToHost(settings[SettingsKeys.InputLocalPath], 
                                                  settings[SettingsKeys.InputHostPath]);

            _statusUpdater.UpdateStatus("Uploading input files to HDFS ...");
            // upload input from host machine to HDFS
            _jobDeployer.SendInputFromHostToHDFS(settings[SettingsKeys.InputHostPath], 
                                                 settings[SettingsKeys.InputHdfsPath]);

            _statusUpdater.UpdateStatus("Running MapReduce job. This could take a while ...");
            // run job
            _jobDeployer.RunJob(string.Format("{0}/{1}", 
                                              settings[SettingsKeys.SrcHostPath],
                                              settings[SettingsKeys.JarName]),
                                settings[SettingsKeys.MainClassName], 
                                settings[SettingsKeys.InputHdfsPath],
                                settings[SettingsKeys.OutputHdfsPath],
                                clusters.ToString());

            _statusUpdater.UpdateStatus("Gathering outputs from HDFS ...");
            // gather output from hdfs to host machine
            _jobDeployer.GetOutputFromHdfsToHost(settings[SettingsKeys.OutputHostPathRelative],
                                                 settings[SettingsKeys.OutputHostPathFull],
                                                 settings[SettingsKeys.OutputHdfsPath]);

            _statusUpdater.UpdateStatus("Downloading outputs from Host machine ...");
            // gather output from hdfs to local machine
            _jobDeployer.GetOutputFromHostToLocal(settings[SettingsKeys.OutputLocalPath],
                                                  settings[SettingsKeys.OutputHostPathFull]);

            _statusUpdater.UpdateStatus("Analyzing results. Hold tight ...");
            // get the results files
            var resultsFiles = Directory.GetFiles(settings[SettingsKeys.OutputLocalPath],
                                                  "part*",
                                                  SearchOption.TopDirectoryOnly);

            return resultsFiles.FirstOrDefault();
        }

        private Dictionary<string, int> GetClusteringResults(string resultFilePath)
        {
            // if file doesn't exist
            if (!File.Exists(resultFilePath))
                throw new FileNotFoundException("Results file not found. " + resultFilePath);

            // read original mappings from file
            var mappings = (from line in File.ReadAllLines(resultFilePath, Encoding.UTF8)
                            let mapping = line.Split(new[] { '\t' }, StringSplitOptions.RemoveEmptyEntries)
                            select new
                            {
                                Symbol = mapping.FirstOrDefault(),
                                Cluster = mapping.LastOrDefault()
                            })
                           .ToList();

            // convert the mappings to cluster indices (for example 0_0 to 1, 0_1 to 2 and so on ...)
            var nameMappings =
                from mapping in mappings
                let clusterNameMappings = mappings.Select(m => m.Cluster)
                                                  .Distinct()
                                                  .Select((cluster, index) => new
                                                  {
                                                      ClusterName = cluster,
                                                      ClusterIndex = index + 1
                                                  })
                                                  .ToDictionary(k => k.ClusterName, v => v.ClusterIndex)
                select new
                {
                    Symbol = mapping.Symbol,
                    Cluster = clusterNameMappings[mapping.Cluster]
                };

            return nameMappings.ToDictionary(k => k.Symbol,
                                             v => v.Cluster);
        } 

        #endregion
    }
}

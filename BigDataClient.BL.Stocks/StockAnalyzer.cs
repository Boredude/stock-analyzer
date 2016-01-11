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

        public IStockAnalysisResults Analyze(IEnumerable<IStock> stocks, 
                                             StockPriceType features, 
                                             int clusters,
                                             Dictionary<string, string> settings)
        {
            // Indicate analyze process is in progress hence cannot be started
            IsAnalyzing = true;
            
            // start stopwatch timer
            var stopwatch = Stopwatch.StartNew(); 

            // Don't start analyzing if not stocks are enabled
            if (!stocks.Any()) return new StockAnalysisResults
            {
                Duration = stopwatch.Elapsed,
                IsSuccess = false,
                Results = null
            };

            //Export the stocks to the input directory
            Export("input", stocks, features);

            // initate map reduce process
            LaunchMapReduce(settings, clusters);

            // get clustering results
            // TODO: read this from SSH
            var clusteringResults = GetClusteringResults(@"C:\Users\Omri\Desktop\part-r-00000");

            // Indicate analyze process has finished hence can be started
            IsAnalyzing = false;
            // stop stopwatch
            stopwatch.Stop();

            return new StockAnalysisResults
            {
                Duration = stopwatch.Elapsed,
                IsSuccess = true,
                Results = stocks.Where(stock => clusteringResults.ContainsKey(stock.Symbol))
                                .Select(stock => new StockAnalysisResult(stock)
                                {
                                    Cluster = clusteringResults[stock.Symbol]
                                })
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

        private void LaunchMapReduce(Dictionary<string, string> settings, int clusters)
        {
            //// connect to remote host
            //_jobDeployer.Connect(settings[SettingsKeys.HostIP],
            //                     settings[SettingsKeys.HostUsername], 
            //                     settings[SettingsKeys.HostPassword]);

            //// send map reduce source files to host mahine
            //_jobDeployer.SendMapReduceFromLocalToHost(settings[SettingsKeys.SrcLocalPath],
            //                                          settings[SettingsKeys.SrcHostPath]);


        }

        private Dictionary<string, int> GetClusteringResults(string resultFilePath)
        {
            // if file doesn't exist
            if (!File.Exists(resultFilePath))
                throw new FileNotFoundException("Results file not found. " + resultFilePath);

            // read original mappings from file
            var mappings = (from line in File.ReadAllLines(resultFilePath, Encoding.UTF8)
                            let mapping = line.Split(new [] { '\t' }, StringSplitOptions.RemoveEmptyEntries)
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
    }
}

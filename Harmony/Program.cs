using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using Documents;
using MFilesLib;
using MFilesAPI;
using TreatiesService;

namespace Harmony
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Run();
            Console.WriteLine("Press any key...");
            Console.ReadKey();
        }

        private static void Run()
        {
            var config = new AppJsonConfiguration();
            var secret = new PrivateJsonConfiguration();

            var listener = new ConsoleTraceListener()
            {
                //TraceOutputOptions = TraceOptions.ThreadId
            };
            Trace.Listeners.Add(listener);

            Trace.TraceInformation($"Harmony version {config.AppVersion}");
            ProcessVaults.Run(secret.MFiles.ServerName, secret.MFiles.UserName, secret.MFiles.Password,
                config.Vaults.Select(v => v.Name).ToArray(), config.View, config.StartDate,
                config.Vaults.ToDictionary(vault => vault.Name, vault => vault.ListProperties?.ToArray() ?? new string[] {}),
                new MainProcessor(secret.ConnectionString, config.Vaults.ToDictionary(cfg => cfg.Name, cfg => cfg),
                    config.ThumbnailsUrlPattern, new CountriesClient(config.TreatiesServiceUrl),
                    config.DeleteNotProcessed)
                );
        }
    }
}
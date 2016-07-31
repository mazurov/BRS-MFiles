using System;
using System.Diagnostics;
using System.Linq;
using Essential.Diagnostics;
using MFilesLib;
using NLog;
using TreatiesService;

namespace Harmony
{
    internal class Program
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();
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
            _logger.Info($"Harmony version {config.AppVersion}");
            ProcessVaults.Run(secret.MFiles.ServerName, secret.MFiles.UserName, secret.MFiles.Password,
                config.Vaults.Select(v => v.Name).ToArray(), config.View, config.StartDate,
                config.Vaults.Where(v => v.Crm !=null).ToDictionary(vault => vault.Name, vault => from p in vault.Crm select new PropertyListType(vault.Name, p.Type, p.Property)),
                new MainProcessor(secret.ConnectionString, config.Vaults.ToDictionary(cfg => cfg.Name, cfg => cfg),
                    config.ThumbnailsUrlPattern, config.BrsTermsUrlPattern, config.LeoTermsUrlPattern, new CountriesClient(config.TreatiesServiceUrl), new ConferencesClient(config.ConferencesServiceUrl),
                    config.DeleteNotProcessed)
                );
        }
    }
}
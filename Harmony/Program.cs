using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Timers;
using Essential.Diagnostics;
using MFilesLib;
using NLog;
using TreatiesService;
using Topshelf;

namespace Harmony
{
    internal class HarmonyService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly Timer _timer;
        public HarmonyService()
        {
            _timer = new Timer
            {
                AutoReset = true,
                Enabled = false
                
            };
            _timer.Elapsed += ProcessOnTimer;
        }

        public void ProcessOnTimer(object source, ElapsedEventArgs args)
        {
            Run();
        }

        public void Start()
        {
            Logger.Info("Start service");
            Run();

        }
        public void Stop()
        {
            Logger.Info("Stop service");
            _timer.Stop();
        }

        private  void Run()
        {
            var config = new AppJsonConfiguration();
            var secret = new PrivateJsonConfiguration();
            Logger.Info($"Harmony version {config.AppVersion}");
            _timer.Enabled = false;
            try
            {
                ProcessVaults.Run(secret.MFiles.ServerName, secret.MFiles.UserName, secret.MFiles.Password,
                    config.Vaults.Select(v => v.Name).ToArray(), config.View, config.StartDate,
                    config.Vaults.Where(v => v.Crm != null)
                        .ToDictionary(vault => vault.Name,
                            vault => from p in vault.Crm select new PropertyListType(vault.Name, p.Type, p.Property)),
                    new MainProcessor(secret.ConnectionString, config.Vaults.ToDictionary(cfg => cfg.Name, cfg => cfg),
                        config.ThumbnailsUrlPattern, config.BrsTermsUrlPattern, config.LeoTermsUrlPattern,
                        new CountriesClient(config.TreatiesServiceUrl),
                        new ConferencesClient(config.ConferencesServiceUrl),
                        config.DeleteNotProcessed)
                    );
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Service exception");
            }
            _timer.Interval = config.ServiceInterval * 60 * 1000;
            _timer.Enabled = true;
            Logger.Info($"Next harmonization in {config.ServiceInterval} minutes");
        }
    }

    internal class Program
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static void PressKey()
        {
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        private static void Main(string[] args)
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            HostFactory.Run(x =>                                 //1
            {
                x.Service<HarmonyService>(s =>                        //2
                {
                    s.ConstructUsing(name => new HarmonyService());     //3
                    s.WhenStarted(tc => tc.Start());              //4
                    s.WhenStopped(tc => tc.Stop());               //5
                });
                x.DependsOnEventLog();
                
                x.RunAsLocalSystem();                            //6

                x.SetDescription("M-Files to database syncronization tool");        //7
                x.SetDisplayName("HarmonyApp");                       //8
                x.SetServiceName("HarmonyApp");                       //9
                
                x.AfterInstall(PressKey);
                x.AfterUninstall(PressKey);

            });
        }

        private static void Run()
        {
            var config = new AppJsonConfiguration();
            var secret = new PrivateJsonConfiguration();
            Logger.Info($"Harmony version {config.AppVersion}");
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
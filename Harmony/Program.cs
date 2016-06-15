using System;
using System.Diagnostics;
using System.Linq;

using MFilesLib;
using MFilesAPI;

namespace Harmony
{
    internal class Processor : IProcessor
    {
        public void ProcessObject(ObjectVersion obj, string vaultName, Vault vault)
        {
            Trace.TraceInformation($"Result {obj.Title} from {vaultName}");
        }
    }
    internal class Program
    {
        static void Main(string[] args)
        {
            var config = new AppJsonConfiguration();
            var secret = new PrivateJsonConfiguration();


            Console.Out.WriteLine(config.AppVersion);
            ProcessVaults.Run(secret.MFiles.ServerName, secret.MFiles.UserName, secret.MFiles.Password,
                config.Vaults.Select(v => v.Name).ToArray(), config.View, config.StartDate, new Processor());
            Trace.TraceInformation("Finished");
            Console.ReadKey();
        }
    }
}
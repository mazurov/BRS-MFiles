using System;
using System.Diagnostics;
using System.Linq;

namespace Harmony
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var config = new AppJsonConfiguration();
            var secret = new PrivateJsonConfiguration();


            Console.Out.WriteLine(config.AppVersion);
            MFilesLib.ProcessVaults.Run(secret.MFiles.ServerName, secret.MFiles.UserName, secret.MFiles.Password,
                config.Vaults.Select(v => v.Name).ToArray(), config.View, config.StartDate);
            Trace.TraceInformation("Finished");
            Console.ReadKey();
        }
    }
}
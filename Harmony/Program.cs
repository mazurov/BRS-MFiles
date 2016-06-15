using System;

namespace Harmony
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var config = new AppJsonConfiguration();
  
            Console.Out.WriteLine(config.AppVersion);
            foreach (var vault in config.Vaults)
            {
                Console.Out.WriteLine(vault.Name);
            }

            Console.ReadKey();
        }
    }
}

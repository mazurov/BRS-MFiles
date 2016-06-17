using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FX.Configuration;

namespace Harmony
{
    public class MFilesDetails
    {
        public string ServerName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    class PrivateJsonConfiguration : JsonConfiguration
    {
        public PrivateJsonConfiguration()
            : base("private.json")
        {
        }

        public MFilesDetails MFiles { get; set; }
        public string ConnectionString { get; set; }
    }
}
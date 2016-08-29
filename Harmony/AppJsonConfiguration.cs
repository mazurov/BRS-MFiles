using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FX.Configuration;

namespace Harmony
{
    public class VaultDetails
    {
        public string Name { get; set; }
        public string NameInDb { get; set; }
        public string Url { get; set; }
        public bool Enabled { get; set; }
        public List<CrmDetail> Crm { get; set; }
    }

    public class CrmDetail
    {
        public string Type { get; set; }
        public string Property { get; set; }
    }

    public class AppJsonConfiguration : JsonConfiguration
    {
        public AppJsonConfiguration()
        : base("config.json")
    {
        }
        /// <summary>
        /// Gets the application version
        /// </summary>
        public string AppVersion { get; set; }

        /// <summary>
        /// Gets the integer values
        /// </summary>
        public List<VaultDetails> Vaults { get; set; }

        public string View { get; set; }
        public DateTime StartDate { get; set; }
        public string TreatiesServiceUrl { get; set; }
        public string ConferencesServiceUrl { get; set; }
        public string ThumbnailsUrlPattern { get; set; }
        public string BrsTermsUrlPattern { get; set; }
        public string LeoTermsUrlPattern { get; set; }

        public bool DeleteNotProcessed { get; set; }

        public int ServiceInterval { get; set; }

    }
}

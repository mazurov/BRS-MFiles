using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Runtime.Serialization;
using MFilesLib;
using Documents;
using TreatiesService;

namespace Harmony
{

    class MainProcessorContext : IProcessorContext
    {
        private readonly DocumentsContext _ctx;
        private readonly MainProcessor _parent;
        public MainProcessorContext(MainProcessor parent)
        {
            _ctx = new DocumentsContext(parent.ConnectionString);
            _parent = parent;

            _ctx.Database.Connection.Open();

            Trace.TraceInformation($"Connection string {_ctx.Database.Connection.ConnectionString}");

            if (_ctx.Database.Connection.State != ConnectionState.Open)
            {
                Trace.TraceError("Could not connect to database");
                return;
            }
            Trace.TraceInformation("Connection open");
        }

        public void ProcessObject(ObjectVersionWrapper obj)
        {
            Trace.TraceInformation($"Process {obj.Title} from {obj.VaultName} with UN-Number {obj.UnNumber}");
            _parent.ProcessedMFilesGuids.Add(obj.Guid);
            var doc = Logic.FindDocument(_ctx, obj.Guid);
            var master = Logic.FindMaster(_ctx,  obj.UnNumber ?? obj.Name );
            var masterByGuid = Logic.FindMasterById(_ctx, obj.Guid);

            if (doc != null && (master == null || master.Guid != masterByGuid.Guid)) // Changed UnNumber
            {
                Logic.Delete(_ctx, doc);
                doc = null;
            }

            // Special case for Basel convention
            if (master != null && (obj.VaultName != "basel" && master.Convention == "basel"))
            {
                Logic.Delete(_ctx, doc);
                return;
            }

            if (doc == null)
            {
                if (master == null)
                {
                   master = Logic.CreateMaster(_ctx, obj, _parent.VaultDetails, _parent.Countries);
                }

                if (master != null)
                {
                    if (master.Guid != obj.Guid)
                    {
                        Trace.TraceInformation($"Create slave document {obj.File.Name}.{obj.File.Extension}");
                    }
                    Logic.CreateSlave(_ctx, master, obj, _parent.VaultDetails, _parent.ThumbnailsUrlPattern);
                }

            } else if (doc.ModifiedDate != obj.ModifiedDate)
            {
               

                if (master.Guid == doc.Guid)
                {
                    Trace.TraceInformation($"Update master document {obj.File.Name}.{obj.File.Extension}");
                    Logic.UpdateMaster(_ctx, master, obj, _parent.VaultDetails, _parent.Countries);
                }
                else
                {
                    Trace.TraceInformation($"Update slave document {obj.File.Name}.{obj.File.Extension}");
                }
                Logic.UpdateSlave(_ctx, master, doc, obj, _parent.VaultDetails, _parent.ThumbnailsUrlPattern);
            }
            
        }
    }

    internal class MainProcessor : IProcessor
    {
        private DocumentsContext _ctx;
        public MainProcessor(string connectionString, IDictionary<string, VaultDetails> vaultDetails, string thumbnailsUrlPattern, CountriesClient countries, bool deleteNotInList)
        {
            ConnectionString = connectionString;
            VaultDetails = vaultDetails;
            Countries = countries;
            ThumbnailsUrlPattern = thumbnailsUrlPattern;
            DeleteNotInList = deleteNotInList;

            _ctx = new DocumentsContext(connectionString);
            _ctx.Database.CreateIfNotExists();
        }

        public IDictionary<string, VaultDetails> VaultDetails { get; }
        public CountriesClient Countries { get; }
        public string ThumbnailsUrlPattern { get; }
        public bool DeleteNotInList { get; }

        public string ConnectionString { get;}

        public List<Guid> ProcessedMFilesGuids { get; set; }

        public IProcessorContext CreateContext()
        {
            return new MainProcessorContext(this);
        }

        public void AfterProcessing()
        {
            if (DeleteNotInList)
            {
                Logic.DeleteNotInList(_ctx, ProcessedMFilesGuids);
            }
        }



    }
}
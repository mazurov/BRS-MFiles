using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using MFilesAPI;
using MFilesLib;
using Documents;

namespace Harmony
{

    class MainProcessorContext : IProcessorContext
    {
        private DocumentsContext _ctx;
        private MainProcessor _parent;
        public MainProcessorContext(MainProcessor parent)
        {
            _ctx = new DocumentsContext(parent.ConnectionString);
            try
            {
                _ctx.Database.CreateIfNotExists();
            }
            catch (SqlException ex)
            {
                Trace.TraceError(ex.Message);
                return;
            }
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
            Trace.TraceInformation($"Result {obj.Title} from {obj.VaultName}\n{obj.UnNumber} {obj.Description}");

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
                    Logic.CreateMaster(_ctx, obj, _parent.VaultToConventionMap);
                }
            }

            //Trace.TraceInformation($"UnNumber={obj.UnNumber}");
            //var unNumber = string.IsNullOrEmpty(doc.UnNumber) ? doc.Name : doc.UnNumber;
            //return _ctx.MFilesDocuments.FirstOrDefault(x => x.Document.UnNumber == unNumber);
        }
    }

    internal class MainProcessor : IProcessor
    {


        public MainProcessor(string connectionString, IDictionary<string, string> vaultToConventionMap)
        {
            ConnectionString = connectionString;
            VaultToConventionMap = vaultToConventionMap;
        }

        public IDictionary<string, string> VaultToConventionMap { get; set; }

        public string ConnectionString { get; set; }

        public IProcessorContext CreateContext()
        {
            return new MainProcessorContext(this);
        }


        
    }
}
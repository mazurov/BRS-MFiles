using System;
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
        public MainProcessorContext(string connectionString)
        {
            _ctx = new DocumentsContext(connectionString);
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
            Trace.TraceInformation($"Result {obj.Title} from {obj.VaultName} {obj.UnNumber}");

            var dbDocument = _ctx.MFilesDocuments.FirstOrDefault(d => d.Guid == obj.Guid);

            //Trace.TraceInformation($"UnNumber={obj.UnNumber}");
            //var unNumber = string.IsNullOrEmpty(doc.UnNumber) ? doc.Name : doc.UnNumber;
            //return _ctx.MFilesDocuments.FirstOrDefault(x => x.Document.UnNumber == unNumber);
        }
    }

    internal class MainProcessor : IProcessor
    {
        private readonly string _connectionString;

        public MainProcessor(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IProcessorContext CreateContext()
        {
            return new MainProcessorContext(_connectionString);
        }


        
    }
}
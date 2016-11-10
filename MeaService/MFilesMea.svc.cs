//------------------------------------------------------------------------------
// <copyright file="WebDataService.svc.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Data.Services;
using System.Data.Services.Common;
using System.Data.Services.Providers;
using System.Linq;

using System.ServiceModel;
using System.ServiceModel.Web;
using System.Web;
using MeaDocuments;

namespace MeaService
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class MFilesMea : EntityFrameworkDataService<DocumentsContext> 
    {
        // This method is called only once to initialize service-wide policies.
        public static void InitializeService(DataServiceConfiguration config)
        {
            // TODO: set rules to indicate which entity sets and service operations are visible, updatable, etc.
            // Examples:
            config.SetEntitySetAccessRule("*", EntitySetRights.AllRead);
            config.SetServiceOperationAccessRule("*", ServiceOperationRights.All);
            config.DataServiceBehavior.MaxProtocolVersion = DataServiceProtocolVersion.V3;
        }

        protected override void HandleException(HandleExceptionArgs args)
        {
            System.Diagnostics.Debug.WriteLine(args.Exception.Message);
            base.HandleException(args);
        }

        protected override DocumentsContext CreateDataSource()
        {
            var result = base.CreateDataSource();
            result.Database.CommandTimeout = 60;
            // result.Database.Log = message => System.Diagnostics.Debug.Write(message);
            return result;
        }
    }
}

//------------------------------------------------------------------------------
// <copyright file="WebDataService.svc.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Data.Services;
using System.Data.Services.Common;
using System.Data.Services.Providers;
using System.ServiceModel;
using System.Web.Configuration;
using Documents;

namespace MFilesService
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class MFiles : EntityFrameworkDataService<DocumentsContext>
    {
        // This method is called only once to initialize service-wide policies.
        public static void InitializeService(DataServiceConfiguration config)
        {
            config.SetEntitySetAccessRule("*", EntitySetRights.AllRead);
            config.SetServiceOperationAccessRule("*", ServiceOperationRights.AllRead);
            config.DataServiceBehavior.MaxProtocolVersion = DataServiceProtocolVersion.V3;
            config.UseVerboseErrors = true;
            config.SetEntitySetPageSize("*",
                Convert.ToInt32(WebConfigurationManager.AppSettings["AllEntitiesPageSize"])
                );
        }
        protected override void HandleException(HandleExceptionArgs args)
        {
            System.Diagnostics.Debug.WriteLine(args.Exception.Message);
            base.HandleException(args);
        }
    }
}
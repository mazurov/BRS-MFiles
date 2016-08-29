using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Transactions;
using MFilesAPI;
using NLog;

namespace MFilesLib
{
    public static class ProcessVaults
    {
        private static MFilesServerApplication _mfilesServer;
        private static readonly Logger ClassLogger = LogManager.GetCurrentClassLogger();

        private static void ProcessData(string vaultName, Vault vault, IView view, DateTime startDate,
            IProcessor processor)
        {
            Thread.CurrentThread.Name = $"Thread-{vaultName}";


            var conditions = view.SearchConditions;
            var dfDate = new DataFunctionCall();
            dfDate.SetDataDate();

            var search = new SearchCondition();
            var expression = new Expression();
            var value = new TypedValue();

            expression.SetPropertyValueExpression((int) MFBuiltInPropertyDef.MFBuiltInPropertyDefLastModified,
                MFParentChildBehavior.MFParentChildBehaviorNone, dfDate);
            search.Set(expression, MFConditionType.MFConditionTypeGreaterThanOrEqual, value);

            conditions.Add(-1, search);

            search = new SearchCondition();
            expression = new Expression();
            value = new TypedValue();
            expression.SetPropertyValueExpression((int) MFBuiltInPropertyDef.MFBuiltInPropertyDefLastModified,
                MFParentChildBehavior.MFParentChildBehaviorNone, dfDate);
            search.Set(expression, MFConditionType.MFConditionTypeLessThan, value);

            conditions.Add(-1, search);


            var currentDateTime = startDate;

            var processorContext = processor.CreateContext();
            while (currentDateTime < DateTime.Now)
            {
                conditions[conditions.Count - 1].TypedValue.SetValue(MFDataType.MFDatatypeDate, currentDateTime);
                currentDateTime = currentDateTime.AddMonths(1);
                conditions[conditions.Count].TypedValue.SetValue(MFDataType.MFDatatypeDate, currentDateTime);


                ObjectSearchResults objects = vault.ObjectSearchOperations.SearchForObjectsByConditionsEx(conditions,
                    MFSearchFlags.MFSearchFlagReturnLatestVisibleVersion, false, 0);

                foreach (ObjectVersion objVer in objects)
                {
                    processorContext.ProcessObject(new ObjectVersionWrapper(objVer, vault, vaultName));
                }
                //internalDocuments.AddRange(from ObjectVersion obj in objects
                //                            select new MFilesInternalDocument(internalVault, obj));
            }
        }

        public static void Run(string serverName, string userName, string password, string[] vaultNames, string viewName,
            DateTime startDate, IDictionary<string, IEnumerable<PropertyListType>> listProperties, IProcessor processor)
        {
            _mfilesServer = new MFilesServerApplication();
            MFServerConnection result;
            try
            {
                result = _mfilesServer.Connect(MFAuthType.MFAuthTypeSpecificMFilesUser, userName, password,
                    NetworkAddress: serverName);
            }
            catch (COMException ex)
            {
                ClassLogger.Error(ex, $"Could not connect to M-Files server");
                return;
            }
            if (result != MFServerConnection.MFServerConnectionAuthenticated)
            {
                ClassLogger.Error("Could not connect to M-Files server");
                return;
            }

            var topContext = processor.CreateContext();

 
            var vaultsOnServer = _mfilesServer.GetVaults();

            IList<Tuple<string, Vault, IView>> data = new List<Tuple<string, Vault, IView>>();
            foreach (var vaultName in vaultNames)
            {
                IVaultOnServer vaultOnServer;
                try
                {
                    vaultOnServer = vaultsOnServer.GetVaultByName(vaultName);
                }
                catch (COMException)
                {
                    ClassLogger.Error($"Could not find vault '{vaultName}'");
                    continue;
                }

                Vault vault = vaultOnServer.LogIn();
                if (!vault.LoggedIn)
                {
                    ClassLogger.Error($"Could not logging to vault '{vaultName}'");
                    continue;
                }

                if (listProperties.ContainsKey(vaultName))
                {

                    foreach (var listProperty in listProperties[vaultName])
                    {
                        var def =
                            vault.PropertyDefOperations.GetPropertyDefs()
                                .Cast<PropertyDef>()
                                .SingleOrDefault(x => x.Name == listProperty.PropertyName);
                        
                        if (def == null || !def.BasedOnValueList)
                        {
                            ClassLogger.Error($"Property {listProperty.PropertyName} in {vaultName} is not a list");
                            continue;
                        }

                        ValueListItems items = vault.ValueListItemOperations.GetValueListItems(def.ValueList,true,MFExternalDBRefreshType.MFExternalDBRefreshTypeQuick);
                        
                        foreach (ValueListItem item in items)
                        {
                            Guid guid;
                            if (!Guid.TryParse(item.DisplayID, out guid))
                            {
                                guid = Guid.NewGuid();
                            }

                            listProperty.Items.Add(new PropertyListItem(guid, item.Name));
                        
                        }

                        topContext.ProcessListProperty(listProperty);

                    }
                }

                IView view = vault.ViewOperations.GetViews().Cast<IView>().FirstOrDefault(v => v.Name == viewName);
                if (view == null)
                {
                    ClassLogger.Warn($"Could not find view '{viewName}' in vault  '{vaultName}'");
                    continue;
                }
                data.Add(Tuple.Create(vaultName, vault, view));
            }

            IList<Task> tasks = new List<Task>();

            foreach (var taskData in data)
            {
                var task =
                    new Task(() => ProcessData(taskData.Item1, taskData.Item2, taskData.Item3, startDate, processor));
                tasks.Add(task);
                task.Start();
            }

            foreach (var task in tasks)
            {
                task.Wait();
            }
            processor.AfterProcessing();
        }
    }
}
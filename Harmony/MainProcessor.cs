using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Documents;
using MFilesLib;
using NLog;
using TreatiesService;
using TreatiesService.Conferences;
using ListProperty = Documents.ListProperty;

namespace Harmony
{
    public class ListPropertyTypesNames
    {
        public static string Chemical = "chemical";
        public static string Term = "term";
        public static string Tag = "tag";
        public static string Program = "programme";
        public static string Type = "type";
        public static string Meeting = "meeting";
        public static string MeetingType = "meetingtype";

        public static string[] Names = {Chemical, Term, Tag, Program, Type, Meeting, MeetingType};
    }

    internal class MainProcessorContext : IProcessorContext
    {
        private static readonly Logger ClassLogger = LogManager.GetCurrentClassLogger();
        private readonly DocumentsContext _ctx;
        private readonly MainProcessor _parent;


        public MainProcessorContext(MainProcessor parent)
        {
            _ctx = new DocumentsContext(parent.ConnectionString);
            _parent = parent;

            _ctx.Database.Connection.Open();

            ClassLogger.Info($"Connection string {_ctx.Database.Connection.ConnectionString}");

            if (_ctx.Database.Connection.State != ConnectionState.Open)
            {
                ClassLogger.Error("Could not connect to database");
                return;
            }
            ClassLogger.Info("Connection open");
        }


        public void ProcessObject(ObjectVersionWrapper obj)
        {
            ClassLogger.Info($"Process {obj.Title} from {obj.VaultName} with UN-Number {obj.UnNumber}");
            _parent.ProcessedMFilesGuids.Add(obj.Guid);
            var doc = Logic.FindDocument(_ctx, obj.Guid);
            var master = Logic.FindMaster(_ctx, string.IsNullOrEmpty(obj.UnNumber) ? obj.Name : obj.UnNumber);
            var masterByGuid = Logic.FindMasterById(_ctx, obj.Guid);

            if (doc != null && (master == null || master.Guid != masterByGuid.Guid)) // Changed UnNumber
            {
                Logic.Delete(_ctx, doc);
                doc = null;
            }

            // Special case for Basel convention
            if (master != null && obj.VaultName != "Basel" && master.Convention == "basel")
            {
                Logic.Delete(_ctx, doc);
                return;
            }

            if (doc == null)
            {
                if (master == null)
                {
                    ClassLogger.Info($"Create master document '{obj.File.Name}.{obj.File.Extension}' {obj.ModifiedDate}");
                    master = Logic.CreateMaster(_ctx, obj, _parent.VaultDetails, _parent.Countries);
                }

                if (master != null)
                {
                    if (master.Guid != obj.Guid)
                    {
                        ClassLogger.Info(
                            $"Create slave document '{obj.File.Name}.{obj.File.Extension}' {obj.ModifiedDate}");
                    }
                    Logic.CreateSlave(_ctx, master, obj, _parent.VaultDetails, _parent.ThumbnailsUrlPattern);
                }
            }
            else if (doc.ModifiedDate != obj.ModifiedDate)
            {
                if (master.Guid == doc.Guid)
                {
                    ClassLogger.Info($"Update master document '{obj.File.Name}.{obj.File.Extension}' {obj.ModifiedDate}");
                    Logic.UpdateMaster(_ctx, master, obj, _parent.VaultDetails, _parent.Countries);
                }
                else
                {
                    ClassLogger.Info($"Update slave document '{obj.File.Name}.{obj.File.Extension}' {obj.ModifiedDate}");
                }
                Logic.UpdateSlave(_ctx, master, doc, obj, _parent.VaultDetails, _parent.ThumbnailsUrlPattern);
            }
        }

        public void ProcessListProperty(PropertyListType lst)
        {
            if (lst.Type == "meeting")
            {
                ProcessCrmMeetings(lst);
            }
            else if (lst.Type == "term")
            {
                ProcessCrmTerms(lst);
            }
            else
            {
                ProcessList(lst);
            }


            using (var trans = _ctx.Database.BeginTransaction())
            {
                try
                {
                    _ctx.SaveChanges();
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();

                    ClassLogger.Error($"ProcessListProperty {ex.Message} {lst.Type}");
                    throw;
                }
            }
        }

        private void ProcessCrmMeetings(PropertyListType lst)
        {
            var meetingsType = _ctx.ValueTypes.FirstOrDefault(x => x.Name == lst.Type);

            var serviceMeetings = _parent.Conferences.Meetings;
            foreach (var source in lst.Items)
            {
                Meetings meetingInService;
                serviceMeetings.TryGetValue(source.Guid, out meetingInService);
                if (meetingInService == null)
                {
                    ClassLogger.Warn(
                        $"Not found {source.Guid} ({source.Value}) in the service {_parent.Conferences.ServiceUri}");
                }

                var url = meetingInService != null? $"{_parent.Conferences.ServiceUri}/Meetings(guid'{source.Guid}')":null ;
                var target = _ctx.Values.FirstOrDefault(t => t.ListPropertyId == source.Guid);
                if (target == null)
                {
                    target = new ListProperty
                    {
                        ListPropertyId = source.Guid,
                        Value = source.Value,
                        Url = url,
                        IsFromCrm = true
                    };
                    _ctx.Values.Add(target);
                }
                else
                {
                    target.Value = source.Value;
                    target.Url = url;
                }

                target.Types.Add(meetingsType);
            }
        }

        private void ProcessList(PropertyListType lst)
        {
            var listType = _ctx.ValueTypes.First(x => x.Name == lst.Type);

            foreach (var source in lst.Items)
            {
                var target = _ctx.Values.OfType<ListProperty>().FirstOrDefault(t => t.ListPropertyId == source.Guid);
                if (target == null)
                {
                    target = new ListProperty
                    {
                        ListPropertyId = source.Guid,
                        Value = source.Value,
                        IsFromCrm = true
                    };
                    _ctx.Values.Add(target);
                }
                else
                {
                    target.Value = source.Value;
                }
                target.Types.Add(listType);
            }
        }

        private static string TermToUri(string term)
        {
            return term != null ? Uri.EscapeUriString(term.Trim().Replace(" ", "-")) : null;
        }

        private void UpdateUrlsFromService(ListProperty target, PropertyListItem source)
        {
            var serviceTerms = _parent.Conferences.Terms;
            Terms termsInService;
            serviceTerms.TryGetValue(source.Guid, out termsInService);

            if (termsInService == null)
            {
                ClassLogger.Warn(
                    $"Not found {source.Guid} ({source.Value}) in the service {_parent.Conferences.ServiceUri}");
            }

            var urlBrs = termsInService != null
                ? _parent.BrsTermsUrlPattern.Replace("{term}", TermToUri(termsInService.name))
                : null;
            target.Url = urlBrs;
            target.LeoTerms.Clear();

            if (!string.IsNullOrEmpty(termsInService?.informeaTerm))
            {
                var leoTerms = termsInService.informeaTerm.Split(',');
                foreach (var lterm in leoTerms)
                {
                    var leoName = lterm.Trim();
                    var leoUrl = _parent.LeoTermsUrlPattern.Replace("{term}", TermToUri(leoName));

                    Uri leoTryUrl;
                    if (Uri.TryCreate(leoName, UriKind.Absolute, out leoTryUrl))
                    {
                        leoUrl = leoTryUrl.AbsoluteUri;
                        leoName = leoUrl.Split('/').Last().Replace("-", " ");
                    }

                    var leoTerm = _ctx.LeoTerms.FirstOrDefault(t => t.Name == leoName);
                    if (leoTerm == null)
                    {
                        leoTerm = new LeoTerm {LeoTermId = Guid.NewGuid(), Name = leoName, Url = leoUrl};
                        _ctx.LeoTerms.Add(leoTerm);
                    }
                    target.LeoTerms.Add(leoTerm);
                }
            }
        }

        private void ProcessCrmTerms(PropertyListType lst)
        {
            var termsType = _ctx.ValueTypes.First(x => x.Name == lst.Type);
            foreach (var source in lst.Items)
            {
                var target = _ctx.Values.FirstOrDefault(t => t.ListPropertyId == source.Guid);
                if (target == null)
                {
                    target = new ListProperty
                    {
                        ListPropertyId = source.Guid,
                        Value = source.Value,
                        IsFromCrm = true
                    };
                    _ctx.Values.Add(target);
                }
                else
                {
                    target.Value = source.Value;
                }
                target.Types.Add(termsType);

                UpdateUrlsFromService(target, source);
            }
        }
    }

    internal class MainProcessor : IProcessor
    {
        private readonly DocumentsContext _ctx;

        public MainProcessor(string connectionString, IDictionary<string, VaultDetails> vaultDetails,
            string thumbnailsUrlPattern, string brsTermsUrlPattern, string leoTermsUrlPattern, CountriesClient countries,
            ConferencesClient conferences, bool deleteNotInList)
        {
            ConnectionString = connectionString;
            VaultDetails = vaultDetails;
            Countries = countries;
            Conferences = conferences;
            ThumbnailsUrlPattern = thumbnailsUrlPattern;
            BrsTermsUrlPattern = brsTermsUrlPattern;
            LeoTermsUrlPattern = leoTermsUrlPattern;
            DeleteNotInList = deleteNotInList;

            _ctx = new DocumentsContext(connectionString);
            _ctx.Database.CreateIfNotExists();

            foreach (var type in ListPropertyTypesNames.Names)
            {
                if (_ctx.ValueTypes.FirstOrDefault(x => x.Name == type) == null)
                {
                    _ctx.ValueTypes.Add(new ListPropertyType {ListPropertyTypeId = Guid.NewGuid(), Name = type});
                }
            }
            _ctx.SaveChanges();
        }

        public IDictionary<string, VaultDetails> VaultDetails { get; }
        public CountriesClient Countries { get; }
        public ConferencesClient Conferences { get; }
        public string ThumbnailsUrlPattern { get; }
        public string BrsTermsUrlPattern { get; }
        public string LeoTermsUrlPattern { get; }
        public bool DeleteNotInList { get; }

        public string ConnectionString { get; }

        public List<Guid> ProcessedMFilesGuids { get; set; } = new List<Guid>();

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
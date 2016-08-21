using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Documents;
using MFilesLib;
using MimeSharp;
using NLog;
using TreatiesService;

namespace Harmony
{
    public class Logic
    {
        private static readonly Mime Mime = new Mime();
        private static readonly Logger ClassLogger = LogManager.GetCurrentClassLogger();

        public static MFilesDocument FindDocument(DocumentsContext ctx, Guid docGuid)
        {
            return ctx.MFilesDocuments.FirstOrDefault(d => d.Guid == docGuid);
        }

        public static MFilesDocument FindMaster(DocumentsContext ctx, string unNumber)
        {
            return ctx.MFilesDocuments.FirstOrDefault(x => x.Document.UnNumber == unNumber);
        }

        public static MFilesDocument FindMasterById(DocumentsContext ctx, Guid guid)
        {
            var file = ctx.Files.FirstOrDefault(f => f.FileId == guid);
            return file?.Document.MFilesDocument;
        }

        public static Tuple<string, bool> GetAuthor(ObjectVersionWrapper obj, CountriesClient countries)
        {
            
           
            var author = obj.Author;
            var cauthor = obj.CorporateAuthor;
            var player = obj.Player;

            if (!string.IsNullOrWhiteSpace(author))
            {
                return new Tuple<string, bool>(author, false);
            }

            if (!string.IsNullOrEmpty(cauthor))
            {
                return new Tuple<string, bool>(cauthor, true);
            }

            /*if (!string.IsNullOrEmpty(player))
            {
                var country = countries.GetCountryIsoCode2(obj.Player);
                if (country != null)
                {
                    return new Tuple<string, bool>(player, true);
                }
            }*/

            return null;
    }



        public static bool UpdateMaster(DocumentsContext ctx, MFilesDocument targetDoc, ObjectVersionWrapper  sourceDoc,
            IDictionary<string,VaultDetails> vaultDetails, CountriesClient countries)
        {
 
            targetDoc.Guid = sourceDoc.Guid;
            targetDoc.CreatedDate = sourceDoc.CreatedDate;
            targetDoc.ModifiedDate = sourceDoc.ModifiedDate;

            var masterDoc = targetDoc.Document;
            masterDoc.MFilesDocument = targetDoc;
            masterDoc.UnNumber = string.IsNullOrEmpty(sourceDoc.UnNumber)?sourceDoc.Name:sourceDoc.UnNumber;    
            masterDoc.Convention = vaultDetails[sourceDoc.VaultName].NameInDb ?? sourceDoc.VaultName.ToLower();
            var authorAndType = GetAuthor(sourceDoc, countries);
            if (authorAndType != null)
            {
                masterDoc.Author = authorAndType.Item1;
                masterDoc.AuthorType = authorAndType.Item2 ? "organization" : "person";
            }
            masterDoc.CountryFull = countries.GetCountryIsoCode2(sourceDoc.Country) != null ? sourceDoc.Country: null;
            masterDoc.Country = countries.GetCountryIsoCode2(masterDoc.CountryFull);
            masterDoc.Copyright = sourceDoc.Copyright;
            var period = sourceDoc.GetPeriod();
            if (period != null)
            {
                masterDoc.PeriodStartDate = period.Item1;
                masterDoc.PeriodEndDate = period.Item2;
            }
            masterDoc.PublicationDate = sourceDoc.PublicationDate;

            lock ("Process") { 
                ProcessDocumentTypes(ctx, masterDoc, sourceDoc);
                ProcessMeetings(ctx, masterDoc, sourceDoc);
                ProcessMeetingTypes(ctx, masterDoc, sourceDoc);
                ProcessChemicals(ctx, masterDoc, sourceDoc);
                ProcessPrograms(ctx, masterDoc, sourceDoc);
                ProcessTerms(ctx, masterDoc, sourceDoc);
                ProcessTags(ctx, masterDoc, sourceDoc);
         
                using (var trans = ctx.Database.BeginTransaction())
                {
                    try
                    {
                        ctx.SaveChanges();
                        trans.Commit();
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        ClassLogger.Error("SQL exception "  + ex.Message);
                        throw;
                    }
                }
            }
            return true;
        }

        private static void ProcessDocumentTypes(DocumentsContext ctx, Document masterDoc, ObjectVersionWrapper sourceDoc)
        {
            masterDoc.Types.Clear();
            var propertyType = ctx.ValueTypes.First(x => x.Name == ListPropertyTypesNames.Type);
            foreach (var type in sourceDoc.Types)
            {
                var targetType = ctx.Values.FirstOrDefault(t => t.Value == type.Value);
                if (targetType == null)
                {
                    targetType = new Documents.ListProperty
                    {
                        ListPropertyId = Guid.NewGuid(),
                        Value = type.Value
                    };
                    ctx.Values.Add(targetType);
                }
                targetType.Types.Add(propertyType);
                masterDoc.Types.Add(targetType);
            }
        }

        private static void ProcessTerms(DocumentsContext ctx, Document masterDoc, ObjectVersionWrapper sourceDoc)
        {
            masterDoc.Terms.Clear();
            var propertyType = ctx.ValueTypes.First(x => x.Name == ListPropertyTypesNames.Term);
            foreach (var source in sourceDoc.Terms)
            {
                var values = (source.IsStringProperty
                    ? source.Value.Split(',').Select(x => x.Trim()).ToArray()
                    : new[] { source.Value });
                foreach (var value in values)
                {
                    var target = ctx.Values.FirstOrDefault(t => t.Value == value);
                    if (target == null)
                    {
                        var listPropertyId = Guid.NewGuid();
                        if (!source.IsStringProperty && source.Guid.HasValue)
                        {
                            listPropertyId = source.Guid.Value;
                        }
                        target = new Documents.ListProperty
                        {
                            ListPropertyId = listPropertyId,
                            Value = value.Trim()
                        };
                        ctx.Values.Add(target);
                    }
                    target.Types.Add(propertyType);
                    masterDoc.Terms.Add(target);
                }
            }
        }

        private static void ProcessTags(DocumentsContext ctx, Document masterDoc, ObjectVersionWrapper sourceDoc)
        {
            masterDoc.Tags.Clear();
            var propertyType = ctx.ValueTypes.First(x => x.Name == ListPropertyTypesNames.Tag);
            foreach (var source in sourceDoc.Tags)
            {
                var values = (source.IsStringProperty
                    ? source.Value.Split(',').Select(x => x.Trim().ToLower()).Where(x => !String.IsNullOrEmpty(x)).ToArray()
                    : new[] { source.Value.ToLower() });
                foreach (var value in values)
                {
                    var target = ctx.Values.FirstOrDefault(t => t.Value == value);
                    if (target == null)
                    {
                        var listPropertyId = Guid.NewGuid();
                        if (!source.IsStringProperty && source.Guid.HasValue)
                        {
                            listPropertyId = source.Guid.Value;
                        }
                        target = new Documents.ListProperty
                        {
                            ListPropertyId = listPropertyId,
                            Value = value.Trim()
                        };
                        ctx.Values.Add(target);
                    }
                    target.Types.Add(propertyType);
                    masterDoc.Tags.Add(target);
                }
            }
        }

        private static void ProcessPrograms(DocumentsContext ctx, Document masterDoc, ObjectVersionWrapper sourceDoc)
        {
            masterDoc.Programs.Clear();
            var propertyType = ctx.ValueTypes.First(x => x.Name == ListPropertyTypesNames.Program);
            foreach (var source in sourceDoc.Programs)
            {
                var target = ctx.Values.FirstOrDefault(t => t.Value == source.Value);
                if (target == null)
                {
                    target = new Documents.ListProperty
                    {
                        ListPropertyId = source.Guid ?? Guid.NewGuid(),
                        Value = source.Value
                    };
                    ctx.Values.Add(target);
                }
                target.Types.Add(propertyType);
                masterDoc.Programs.Add(target);
            }
        }

        private static void ProcessChemicals(DocumentsContext ctx,  Document masterDoc, ObjectVersionWrapper sourceDoc)
        {
            masterDoc.Chemicals.Clear();
            var propertyType = ctx.ValueTypes.First(x => x.Name == ListPropertyTypesNames.Chemical);
            foreach (var source in sourceDoc.Chemicals)
            {
                var target = ctx.Values.FirstOrDefault(t => t.Value == source.Value);
                if (target == null)
                {
                    target = new Documents.ListProperty
                    {
                        ListPropertyId = source.Guid ?? Guid.NewGuid(),
                        Value = source.Value
                    };
                    ctx.Values.Add(target);
                }
                target.Types.Add(propertyType);
                masterDoc.Chemicals.Add(target);
            }
        }

        private static void ProcessMeetings(DocumentsContext ctx, Document masterDoc, ObjectVersionWrapper sourceDoc)
        {
            masterDoc.Meetings.Clear();
            var propertyType = ctx.ValueTypes.First(x => x.Name == ListPropertyTypesNames.Meeting);
            foreach (var source in sourceDoc.Meetings)
            {
                var target = ctx.Values.FirstOrDefault(t => t.Value == source.Value);
                if (target == null)
                {
                    target = new Documents.ListProperty
                    {
                        ListPropertyId = source.Guid ?? Guid.NewGuid(),
                        Value = source.Value
                    };
                    ctx.Values.Add(target);
                }
                target.Types.Add(propertyType);
                masterDoc.Meetings.Add(target);
            }
        }

        private static void ProcessMeetingTypes(DocumentsContext ctx, Document masterDoc, ObjectVersionWrapper sourceDoc)
        {
            masterDoc.MeetingsTypes.Clear();
            var propertyType = ctx.ValueTypes.First(x => x.Name == ListPropertyTypesNames.MeetingType);
            foreach (var source in sourceDoc.MeetingsTypes)
            {
                var target = ctx.Values.FirstOrDefault(t => t.Value == source.Value);
                if (target == null)
                {
                    target = new Documents.ListProperty
                    {
                        ListPropertyId = source.Guid ?? Guid.NewGuid(),
                        Value = source.Value
                    };
                    ctx.Values.Add(target);
                }
                target.Types.Add(propertyType);
                masterDoc.MeetingsTypes.Add(target);
            }
        }

        public static MFilesDocument CreateMaster(DocumentsContext ctx, ObjectVersionWrapper sourceDoc, IDictionary<string, VaultDetails> vaultDetails, CountriesClient coutries)
        {
            var targetDoc = ctx.MFilesDocuments.Create();
            ctx.MFilesDocuments.Add(targetDoc);

            var masterDoc = new Document {MFilesDocument = targetDoc};
            ctx.Documents.Add(masterDoc);

            UpdateMaster(ctx, targetDoc, sourceDoc, vaultDetails, coutries);
            return targetDoc;
        }

        public static void Delete(DocumentsContext ctx, MFilesDocument targetDocument)
        {
            if (targetDocument == null)
            {
                return;
            }
            var doc = targetDocument as MFilesDocument;
            Debug.Assert(doc != null);
            if (doc.Title != null)
            {
                ClassLogger.Warn($"Delete document '{doc.Title.Value}'");
            }
            else
            {
                ClassLogger.Warn($"Delete document {doc.Guid}");
            }

            var documents = from x in ctx.Documents where x.DocumentId == doc.Guid select x;
            ctx.Documents.RemoveRange(documents.ToList());

            var titles = from x in ctx.Titles where x.TitleId == doc.Guid select x;
            ctx.Titles.RemoveRange(titles.ToList());

            var descriptions = (from x in ctx.Descriptions where x.DescriptionId == doc.Guid select x).ToList();
            ctx.Descriptions.RemoveRange(descriptions.ToList());

            var files = (from x in ctx.Files where x.FileId == doc.Guid select x).ToList();
            ctx.Files.RemoveRange(files.ToList());

            ctx.MFilesDocuments.Remove(doc);

            using (var trans = ctx.Database.BeginTransaction())
            {
                try
                {
                    ctx.SaveChanges();
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    ClassLogger.Error($"Delete document {ex.Message}");
                    throw;
                }
            }
        }

        public static void DeleteNotInList(DocumentsContext ctx, ICollection<Guid> guids)
        {
            ClassLogger.Info("Find documents for removing...");
            //ctx.Database.CommandTimeout = 600;
            var delGuids = (from mfdoc in ctx.MFilesDocuments select mfdoc.Guid).ToList().Except(guids);
            var toDelete =
                (from mfdoc in ctx.MFilesDocuments where delGuids.Contains(mfdoc.Guid) select mfdoc).ToList();
            //ctx.Database.CommandTimeout = 60;
            ClassLogger.Info($"Number of files to remove = {toDelete.Count()}");
            foreach (var doc in toDelete)
            {
                Delete(ctx, doc);
            }
        }

        public static MFilesDocument CreateSlave(DocumentsContext ctx, MFilesDocument master, ObjectVersionWrapper sourceDoc, IDictionary<string, VaultDetails> vaultDetails, string thumbnailsUrlPattern)
        {
            MFilesDocument targetDoc = null;
            if (sourceDoc.Guid != master.Guid)
            {
                targetDoc = ctx.MFilesDocuments.Create();
            }
            else
            {
                targetDoc = master;
            }
            return UpdateSlave(ctx, master, targetDoc, sourceDoc, vaultDetails, thumbnailsUrlPattern);
        }

        public static  MFilesDocument UpdateSlave(DocumentsContext ctx, MFilesDocument masterDoc, MFilesDocument targetDoc, ObjectVersionWrapper sourceDoc, IDictionary<string, VaultDetails> vaultDetails, string thumbnailsUrlPattern)
        {
  
            if (sourceDoc.Guid != masterDoc.Guid)
            {
                targetDoc.Guid = sourceDoc.Guid;
                targetDoc.ModifiedDate = sourceDoc.ModifiedDate;
                targetDoc.CreatedDate = sourceDoc.CreatedDate;
            }
            else
            {
                targetDoc = masterDoc;
            }

            var doc = masterDoc.Document;
            Debug.Assert(doc != null);

            var languageCode = CultureUtils.GetLangTwoLetterCode(sourceDoc.Language);

            if (languageCode == null)
            {
                ClassLogger.Warn("Could not find language code for  {0} (Document {1})",
                    sourceDoc.Language,
                    sourceDoc.UnNumber);
                return null;
            }

            var title = doc.Titles.FirstOrDefault(t => t.Language == languageCode && t.Document == doc);
            if (title == null || title.MFilesDocument == targetDoc)
            {
                if (title == null)
                {
                    title = new Title {MFilesDocument = targetDoc};
                    doc.Titles.Add(title);
                }

                title.Document = doc;
                title.Language = languageCode;
                title.LanguageFull = sourceDoc.Language;
                title.MFilesDocument = targetDoc;
                title.Value = sourceDoc.Title;
            }

            var descirpiton = doc.Descriptions.FirstOrDefault(t => t.Language == languageCode && t.Document == doc);
            if (descirpiton == null || descirpiton.MFilesDocument == targetDoc)
            {
                if (descirpiton == null)
                {
                    descirpiton = new Description();
                    doc.Descriptions.Add(descirpiton);
                }
                descirpiton.Document = doc;
                descirpiton.Language = languageCode;
                descirpiton.LanguageFull = sourceDoc.Language;
                descirpiton.MFilesDocument = targetDoc;
                descirpiton.Value = sourceDoc.Title;
            }


            var file = sourceDoc.File;
            var repositoryUrl = vaultDetails[sourceDoc.VaultName].Url??"";

           
            var targetFile = ctx.Files.FirstOrDefault(f => f.FileId == targetDoc.Guid);
            if (targetFile == null)
            {
                targetFile = new File();
                doc.Files.Add(targetFile);
            }
            targetFile.Document = doc;
            targetFile.MFilesDocument = targetDoc;
            targetFile.Language = languageCode;
            targetFile.LanguageFull = sourceDoc.Language;
            targetFile.Name = file.Name;
            targetFile.Extension = file.Extension;
            targetFile.Size = file.Size;
            targetFile.MimeType = Mime.Lookup(file.Name + "." + file.Extension);
            targetFile.Url = file.GetUrl(repositoryUrl);
            targetFile.ThumbnailUrl = thumbnailsUrlPattern.Replace("{vault}", sourceDoc.VaultName)
                .Replace("{file}", $"{targetFile.Name}.{targetFile.Extension}");


            using (var trans = ctx.Database.BeginTransaction())
            {
                try
                {
                    ctx.SaveChanges();
                    trans.Commit();
                }
                catch (Exception ex)
                {

                    trans.Rollback();
                    ClassLogger.Error(ex);
                    throw;
                }
            }

            return targetDoc;
        }
    }
}

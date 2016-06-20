using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Documents;
using MFilesLib;

namespace Harmony
{
    public class Logic
    {
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

        public static string GetAuthor(ObjectVersionWrapper obj)
        {
           
                var author = string.IsNullOrWhiteSpace(obj.Author) ? null : obj.Author;
                if (string.IsNullOrWhiteSpace(obj.Player))
                {
                    return author;

                }
                var country = CultureUtils.GetCountryTwoLetterCode(obj.Player);
                if (country == null && author == null)
                {
                    return obj.Player;
                }
                else
                {
                    return author;
                }
    }



        public static void UpdateMaster(DocumentsContext ctx, MFilesDocument targetDoc, ObjectVersionWrapper  sourceDoc,
            IDictionary<string,string> vaultToConventionMap)
        {
 
            targetDoc.Guid = sourceDoc.Guid;
            targetDoc.CreatedDate = sourceDoc.CreatedDate;
            targetDoc.ModifiedDate = sourceDoc.ModifiedDate;

            var masterDoc = targetDoc.Document;
            masterDoc.MFilesDocument = targetDoc;
            masterDoc.UnNumber = sourceDoc.UnNumber??sourceDoc.Name;    
            masterDoc.Convention = vaultToConventionMap.ContainsKey(sourceDoc.VaultName)? vaultToConventionMap[sourceDoc.VaultName]: sourceDoc.VaultName.ToLower();
            masterDoc.Author = GetAuthor(sourceDoc);
            masterDoc.CountryFull = sourceDoc.Country;
            masterDoc.Country = _countries.GetCountryIsoCode2(masterDoc.CountryFull);
            masterDoc.Copyright = sourceDoc.Copyright;
            var period = sourceDoc.GetPeriod();
            if (period != null)
            {
                masterDoc.PeriodStartDate = period.Item1;
                masterDoc.PeriodEndDate = period.Item2;
            }
            masterDoc.PublicationDate = sourceDoc.PublicationDate;

            ProcessDocumentTypes(masterDoc, sourceDoc);
            ProcessMeetings(masterDoc, sourceDoc);
            ProcessMeetingTypes(masterDoc, sourceDoc);
            ProcessChemicals(masterDoc, sourceDoc);
            ProcessPrograms(masterDoc, sourceDoc);
            ProcessTerms(masterDoc, sourceDoc);
            ProcessTags(masterDoc, sourceDoc);

            var status = true;
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
                    Logger.ErrorException("SQL exception", ex);
                    status = false;
                }
            }
            return status ? targetDoc : null;
        }

        public static MFilesDocument CreateMaster(DocumentsContext ctx, ObjectVersionWrapper sourceDoc, IDictionary<string, string> vaultToConventionMap)
        {
            var targetDoc = ctx.MFilesDocuments.Create();
            ctx.MFilesDocuments.Add(targetDoc);

            var masterDoc = new Document();
            masterDoc.MFilesDocument = targetDoc;
            ctx.Documents.Add(masterDoc);

            UpdateMaster(ctx, targetDoc, sourceDoc, vaultToConventionMap);
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
                Trace.TraceInformation($"Delete document '{doc.Title.Value}'");
            }
            else
            {
                Trace.TraceInformation($"Delete document {doc.Guid}");
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
                    Trace.TraceError($"Delete document {ex.Message}");
                }
            }
        }
    }
}

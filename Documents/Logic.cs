using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MFilesLib;

namespace Documents
{
    class Logic
    {
        public static MFilesDocument FindDocument(DocumentsContext ctx, ObjectVersionWrapper doc)
        {
            return ctx.MFilesDocuments.FirstOrDefault(d => d.Guid == doc.Guid);
        }

        public MFilesDocument FindMaster(DocumentsContext ctx, ObjectVersionWrapper doc)
        {
            var unNumber = string.IsNullOrEmpty(doc.UnNumber) ? doc.Name : doc.UnNumber;
            return ctx.MFilesDocuments.FirstOrDefault(x => x.Document.UnNumber == unNumber);
        }

        public MFilesDocument FindMasterById(DocumentsContext ctx, Guid guid)
        {
            var file = ctx.Files.FirstOrDefault(f => f.FileId == guid);
            return file?.Document.MFilesDocument;
        }
    }
}

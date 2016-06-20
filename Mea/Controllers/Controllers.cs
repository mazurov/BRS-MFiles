using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Linq;
using System.Web.Http;
using System.Web.OData;
using Documents;


namespace Mea.Controllers
{
    [EnableQuery]
    public class DocumentsController : ODataController
    {
        private DocumentsContext _ctx = new DocumentsContext();

        private IEnumerable<Models.Document> GetDocuments()
        {
            return from doc in _ctx.Documents
                select new Models.Document
                {
                    schemaVersion = "1.0",
                    id = doc.DocumentId.ToString(),
                    published = doc.MFilesDocument.CreatedDate,
                    updated = doc.MFilesDocument.ModifiedDate,
                    treaty = doc.Convention,
                    types =
                        from t in doc.Types select new Models.Type {id = t.ListPropertyId.ToString(), value = t.Value},
                    authors =
                        new List<Models.Author>
                        {
                            new Models.Author {id = Guid.NewGuid().ToString(), key = "organization", value = doc.Author}
                        },
                    keywords = from t in doc.Terms
                        select
                            new Models.Keyword()
                            {
                                id = t.ListPropertyId.ToString(),
                                termURI = "http://terms.com/" + t.Value,
                                scope = "brs",
                                termValueInEnglish = t.Value
                            },
                    titles =
                        from t in doc.Titles
                        select
                            new Models.LangString() {id = t.TitleId.ToString(), language = t.Language, value = t.Value},
                    descriptions = from t in doc.Descriptions
                        select
                            new Models.LangString()
                            {
                                id = t.DescriptionId.ToString(),
                                language = t.Language,
                                value = t.Value
                            },
                    identifiers =
                        new List<Models.Identifier>
                        {
                            new Models.Identifier()
                            {
                                id = doc.DocumentId.ToString(),
                                identifierName = "UN-Number",
                                identifierValue = doc.UnNumber
                            }
                        },
                    copyright = doc.Copyright,
                    files = from f in doc.Files
                        select
                            new Models.File()
                            {
                                id = f.FileId.ToString(),
                                name = f.Name,
                                extension = f.Extension,
                                language = f.Language,
                                mimetype = f.MimeType,
                                size = f.Size,
                                url = f.Url
                            },
                    thumbnailUrl = "",
                    tags = from t in doc.Chemicals
                        select
                            new Models.Tag()
                            {
                                id = t.ListPropertyId.ToString(),
                                language = "en",
                                scope = "chemical",
                                value = t.Value
                            },
                    country = doc.Country
                };
        }

        public IEnumerable<Models.Document> Get()
        {
            return GetDocuments();
        }

        public Models.Document GetDocument(string key)
        {
            var guid = new Guid();
            Guid.TryParse(key.Replace("'",""), out guid);
            return (from doc in _ctx.Documents where doc.DocumentId==guid
                    select new Models.Document
                   {
                       schemaVersion = "1.0",
                       id = doc.DocumentId.ToString(),
                       published = doc.MFilesDocument.CreatedDate,
                       updated = doc.MFilesDocument.ModifiedDate,
                       treaty = doc.Convention,
                       types =
                           from t in doc.Types select new Models.Type { id = t.ListPropertyId.ToString(), value = t.Value },
                       authors =
                           new List<Models.Author>
                           {
                            new Models.Author {id = Guid.NewGuid().ToString(), key = "organization", value = doc.Author}
                           },
                       keywords = from t in doc.Terms
                                  select
                                      new Models.Keyword()
                                      {
                                          id = t.ListPropertyId.ToString(),
                                          termURI = "http://terms.com/" + t.Value,
                                          scope = "brs",
                                          termValueInEnglish = t.Value
                                      },
                       titles =
                           from t in doc.Titles
                           select
                               new Models.LangString() { id = t.TitleId.ToString(), language = t.Language, value = t.Value },
                       descriptions = from t in doc.Descriptions
                                      select
                                          new Models.LangString()
                                          {
                                              id = t.DescriptionId.ToString(),
                                              language = t.Language,
                                              value = t.Value
                                          },
                       identifiers =
                           new List<Models.Identifier>
                           {
                            new Models.Identifier()
                            {
                                id = doc.DocumentId.ToString(),
                                identifierName = "UN-Number",
                                identifierValue = doc.UnNumber
                            }
                           },
                       copyright = doc.Copyright,
                       files = from f in doc.Files
                               select
                                   new Models.File()
                                   {
                                       id = f.FileId.ToString(),
                                       name = f.Name,
                                       extension = f.Extension,
                                       language = f.Language,
                                       mimetype = f.MimeType,
                                       size = f.Size,
                                       url = f.Url
                                   },
                       thumbnailUrl = "",
                       tags = from t in doc.Chemicals
                              select
                                  new Models.Tag()
                                  {
                                      id = t.ListPropertyId.ToString(),
                                      language = "en",
                                      scope = "chemical",
                                      value = t.Value
                                  },
                       country = doc.Country
                   }).SingleOrDefault();
        }
    }
}
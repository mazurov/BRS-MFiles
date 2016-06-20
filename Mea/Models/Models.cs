using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Mea.Models
{
    public class Document
    {
        public string schemaVersion { get; set; }

        [Key]
        public string id { get; set; }

        public DateTime published { get; set; }
        public DateTime updated { get; set; }
        public string treaty { get; set; }

        public IEnumerable<Type> types { get; set; }
        public IEnumerable<Author> authors { get; set; }
        public IEnumerable<Keyword> keywords { get; set; }

        public IEnumerable<LangString> titles { get; set; }
        public IEnumerable<LangString> descriptions { get; set; }
        public IEnumerable<Identifier> identifiers { get; set; }

        public string copyright { get; set; }

        public IEnumerable<File> files { get; set; }

        public string thumbnailUrl { get; set; }

        public IEnumerable<Tag> tags { get; set; }
        public string country { get; set; }
        public IEnumerable<ReferenceToEntity> referenceToEntities { get; set; }
        public int displayOrder { get; set; }



    }

    public class Type
    {
        [Key]
        public string id { get; set; }

        public string value { get; set; }
    }

    public class Author
    {
        [Key]
        public string id { get; set; }

        public string key { get; set; }
        public string value { get; set; }

    }

    public class Keyword
    {
        [Key]
        public string id { get; set; }

        public string termURI { get; set; }
        public string scope { get; set; }
        public string termValueInEnglish { get; set; }
    }

    public class Tag
    {
        [Key]
        public string id { get; set; }

        public string language { get; set; }
        public string scope { get; set; }
        public string value { get; set; }
        public string comment { get; set; }
    }

    public class LangString
    {
        [Key]
        public string id { get; set; }

        public string language { get; set; }
        public string value { get; set; }
    }

    public class Identifier
    {
        [Key]
        public string id { get; set; }

        public string identifierName { get; set; }
        public string identifierValue { get; set; }
    }

    public class File
    {
        [Key]
        public string id { get; set; }

        [Required]
        [StringLength(3)]
        public string language { get; set; }

        [Required]
        [StringLength(512)]
        public string name { get; set; }

        [Required]
        [StringLength(10)]
        public string extension { get; set; }

        [Required]
        [StringLength(1024)]
        public string url { get; set; }

        [Required]
        [StringLength(255)]
        public string mimetype { get; set; }

        [Required]
        public long size { get; set; }
    }

    public class ReferenceToEntity
    {
        [Key]
        public string id { get; set; }

        public string entityType { get; set; }
        public string refURI { get; set; }
    }
}
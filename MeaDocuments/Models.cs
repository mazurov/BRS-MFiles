using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace MeaDocuments
{
    public class DocumentsContext : DbContext
    {
        public DbSet<Document> Documents { get; set; }
        public DbSet<Type> Types { get; set; }

        public DocumentsContext() : base("DocumentsContext")
        {
            Database.Log = message => System.Diagnostics.Debug.WriteLine(message);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Document>().HasMany(d => d.types).WithMany(t => t.documents).
                Map(m =>
                {
                    m.ToTable("mea_TypeDocuments");
                    m.MapLeftKey("documentId");
                    m.MapRightKey("typeId");
                });

            modelBuilder.Entity<Document>().HasMany(d => d.authors).WithMany(t => t.documents).
                Map(m =>
                {
                    m.ToTable("mea_AuthorDocuments");
                    m.MapLeftKey("documentId");
                    m.MapRightKey("authorId");
                });
            modelBuilder.Entity<Document>().HasMany(d => d.keywords).WithMany(t => t.documents).
                Map(m =>
                {
                    m.ToTable("mea_KeywordDocuments");
                    m.MapLeftKey("documentId");
                    m.MapRightKey("keywordId");
                });
            modelBuilder.Entity<Document>().HasMany(d => d.tags).WithMany(t => t.documents).
                Map(m =>
                {
                    m.ToTable("mea_TagDocuments");
                    m.MapLeftKey("documentId");
                    m.MapRightKey("tagId");
                });
            modelBuilder.Entity<Document>().HasMany(d => d.referenceToEntities).WithMany(t => t.documents).
                Map(m =>
                {
                    m.ToTable("mea_ReferenceToEntityDocuments");
                    m.MapLeftKey("documentId");
                    m.MapRightKey("referenceToEntityId");
                });

        }
    }

    [Table("mea_Documents")]
    public class Document
    {
        [Key]
        public string id { get; set; }
        public string schemaVersion { get; set; }



        public DateTime published { get; set; }
        public DateTime updated { get; set; }
        public string treaty { get; set; }


        public string copyright { get; set; }


        public string thumbnailUrl { get; set; }

        public string country { get; set; }
        public int displayOrder { get; set; }

        public virtual ICollection<Type> types { get; set; }
        public virtual ICollection<Author> authors { get; set; }
        public virtual ICollection<Keyword> keywords { get; set; }

        public virtual ICollection<Titles> titles { get; set; }
        public virtual ICollection<Descriptions> descriptions { get; set; }
        public virtual ICollection<Identifier> identifiers { get; set; }
        public virtual ICollection<Tag> tags { get; set; }
        public virtual ICollection<ReferenceToEntity> referenceToEntities { get; set; }
        public virtual ICollection<File> files { get; set; }
    }

    [Table("mea_Types")]
    public class Type
    {
        [Key]
        public string id { get; set; }
        public string value { get; set; }
        public ICollection<Document> documents { get; set; }
    }

    [Table("mea_Authors")]
    public class Author
    {
        [Key]
        public string id { get; set; }

        public string type { get; set; }
        public string value { get; set; }

        public virtual ICollection<Document> documents { get; set; }
    }

    [Table("mea_Keywords")]
    public class Keyword
    {
        [Key]
        public string id { get; set; }

        public string termURI { get; set; }
        public string scope { get; set; }
        public string termValueInEnglish { get; set; }

        public virtual ICollection<Document> documents { get; set; }
    }

    [Table("mea_Tags")]
    public class Tag
    {
        [Key]
        public string id { get; set; }

        public string language { get; set; }
        public string scope { get; set; }
        public string value { get; set; }
        public string comment { get; set; }

        public virtual ICollection<Document> documents { get; set; }
    }

    [Table("mea_Titles")]
    public class Titles
    {
        [Key]
        public string id { get; set; }

        public string language { get; set; }
        public string value { get; set; }
        public string documentId { get; set; }

        [ForeignKey("documentId")]
        public virtual Document document { get; set; }
    }

    [Table("mea_Descriptions")]
    public class Descriptions
    {
        [Key]
        public string id { get; set; }

        public string language { get; set; }
        public string value { get; set; }
        public string documentId { get; set; }

        [ForeignKey("documentId")]
        public virtual Document document { get; set; }
    }

    [Table("mea_Identifiers")]
    public class Identifier
    {
        [Key]
        public string id { get; set; }

        public string identifierName { get; set; }
        public string identifierValue { get; set; }

        public string documentId { get; set; }
        [ForeignKey("documentId")]
        public virtual Document document { get; set; }
    }

    [Table("mea_Files")]
    public class File
    {
        [Key]
        public string id { get; set; }

        [Required]
        public string language { get; set; }

        [Required]
        public string name { get; set; }

        [Required]
        public string extension { get; set; }

        [Required]
        public string url { get; set; }

        [Required]
        public string mimetype { get; set; }

        [Required]
        public long size { get; set; }

        public string documentId { get; set; }

        [ForeignKey("documentId")]
        public virtual Document document { get; set; }
    }

    [Table("mea_ReferenceToEntity")]
    public class ReferenceToEntity
    {
        [Key]
        public string id { get; set; }

        public string entityType { get; set; }
        public string refURI { get; set; }

        public virtual ICollection<Document> documents { get; set; }
    }
}
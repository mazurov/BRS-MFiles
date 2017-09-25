using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace Documents
{
    public class DocumentsContext : DbContext
    {
        public DocumentsContext(string connectionString) : base(connectionString)
        {
            // TODO(amazurov): workaround if need CreateDatabaseIfItsNotExists 
            Database.SetInitializer<DocumentsContext>(null);
        }


        public DocumentsContext() : base("DocumentsContext")
        {
            

        }

        public DbSet<Document> Documents { get; set; }
        public DbSet<ListPropertyType> ValueTypes { get; set; }
        public DbSet<ListProperty> Values { get; set; }
        public DbSet<LeoTerm> LeoTerms { get; set; }
        public DbSet<Title> Titles { get; set; }
        public DbSet<Description> Descriptions { get; set; }
        public DbSet<File> Files { get; set; }

        public DbSet<MFilesDocument> MFilesDocuments { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Document>()
                .HasMany(t => t.Chemicals)
                .WithMany()
                .Map(m =>
                {
                    m.ToTable("DocumentsChemicals");
                    m.MapLeftKey("DocumentId");
                    m.MapRightKey("PropertyId");
                });

            modelBuilder.Entity<Document>()
                .HasMany(t => t.Meetings)
                .WithMany()
                .Map(m =>
                {
                    m.ToTable("DocumentsMeetings");
                    m.MapLeftKey("DocumentId");
                    m.MapRightKey("PropertyId");
                });

            modelBuilder.Entity<Document>()
                .HasMany(t => t.MeetingsTypes)
                .WithMany()
                .Map(m =>
                {
                    m.ToTable("DocumentsMeetingsTypes");
                    m.MapLeftKey("DocumentId");
                    m.MapRightKey("PropertyId");
                });
            modelBuilder.Entity<Document>()
                .HasMany(t => t.Tags)
                .WithMany()
                .Map(m =>
                {
                    m.ToTable("DocumentsTags");
                    m.MapLeftKey("DocumentId");
                    m.MapRightKey("PropertyId");
                });
            modelBuilder.Entity<Document>()
                .HasMany(t => t.Terms)
                .WithMany()
                .Map(m =>
                {
                    m.ToTable("DocumentsTerms");
                    m.MapLeftKey("DocumentId");
                    m.MapRightKey("PropertyId");
                });

            modelBuilder.Entity<Document>()
               .HasMany(t => t.Types)
               .WithMany()
               .Map(m =>
               {
                   m.ToTable("DocumentsTypes");
                   m.MapLeftKey("DocumentId");
                   m.MapRightKey("PropertyId");
               });

            modelBuilder.Entity<Document>()
               .HasMany(t => t.Programs)
               .WithMany()
               .Map(m =>
               {
                   m.ToTable("DocumentsPrograms");
                   m.MapLeftKey("DocumentId");
                   m.MapRightKey("PropertyId");
               });

        }
    }

    public class MFilesDocument

    {
        [Key]
        public Guid Guid { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; }


        [Required]
        public DateTime ModifiedDate { get; set; }

        public virtual Document Document { get; set; }

        public virtual Title Title { get; set; }
        public virtual Description Description { get; set; }

        [NotMapped]
        public string Convention => Document?.Convention;
    }

    public class Document
    {
        public Document()
        {
            // ReSharper disable  DoNotCallOverridableMethodsInConstructor
            Titles = new HashSet<Title>();
            Descriptions = new HashSet<Description>();
            Chemicals = new HashSet<ListProperty>();
            Terms = new HashSet<ListProperty>();
            Tags = new HashSet<ListProperty>();
            Programs = new HashSet<ListProperty>();
            Types = new HashSet<ListProperty>();
            Meetings = new HashSet<ListProperty>();
            MeetingsTypes = new HashSet<ListProperty>();
            Files = new HashSet<File>();
        }

        [Key, ForeignKey("MFilesDocument")]
        public Guid DocumentId { get; set; }

        [Required]
        [StringLength(255)]
        public string Convention { get; set; }

        [Required]
        [StringLength(255)]
        [Index(IsUnique = true)]
        public string UnNumber { get; set; }

        public string Copyright { get; set; }

        public string Author { get; set; }


        public string AuthorType { get; set; }


        [StringLength(3)]
        public string Country { get; set; }

        [StringLength(255)]
        public string CountryFull { get; set; }


        public DateTime PublicationDate { get; set; }


        public DateTime? PeriodStartDate { get; set; }

        public DateTime? PeriodEndDate { get; set; }


        public virtual ICollection<Title> Titles { get; set; }
        public virtual ICollection<Description> Descriptions { get; set; }
        public virtual ICollection<ListProperty> Chemicals { get; set; }
        public virtual ICollection<ListProperty> Terms { get; set; }
        public virtual ICollection<ListProperty> Tags { get; set; }
        public virtual ICollection<ListProperty> Programs { get; set; }
        public virtual ICollection<ListProperty> Types { get; set; }
        public virtual ICollection<ListProperty> Meetings { get; set; }
        public virtual ICollection<ListProperty> MeetingsTypes { get; set; }

        public virtual ICollection<File> Files { get; set; }


        public virtual MFilesDocument MFilesDocument { get; set; }
    }


    public class File
    {
        [Key, ForeignKey("MFilesDocument")]
        public Guid FileId { get; set; }

        [Required]
        public virtual Document Document { get; set; }

        [Required]
        [StringLength(3)]
        public string Language { get; set; }

        [Required]
        [StringLength(255)]
        public string LanguageFull { get; set; }

        [Required]
        [StringLength(512)]
        public string Name { get; set; }

        [Required]
        [StringLength(10)]
        public string Extension { get; set; }

        [Required]
        [StringLength(1024)]
        public string Url { get; set; }

        [Required]
        [StringLength(255)]
        public string MimeType { get; set; }

        [Required]
        public long Size { get; set; }

        [Required]
        [StringLength(1024)]
        public string ThumbnailUrl { get; set; }

        public virtual MFilesDocument MFilesDocument { get; set; }
    }

    public class ListPropertyType
    {
        public Guid ListPropertyTypeId { get; set; }
        public string Name { get; set; }

        public virtual ICollection<ListProperty> Properties { get; set; }
    }

    public class ListProperty
    {
        public ListProperty()
        {
            LeoTerms = new HashSet<LeoTerm>();
            Types = new HashSet<ListPropertyType>();
        }

        public Guid ListPropertyId { get; set; }


        [Required(AllowEmptyStrings = true)]
        [DefaultValue("")]
        public string Value { get; set; }

        public string Url { get; set; }
        [DefaultValue(false)]
        public bool IsFromCrm { get; set; }


        public virtual ICollection<ListPropertyType> Types { get; set; }
        //public virtual ICollection<Document> Documents { get; set; }
        public virtual ICollection<LeoTerm> LeoTerms { get; set; }
    }

    public class Title
    {
        [Key, ForeignKey("MFilesDocument")]
        public Guid TitleId { get; set; }

        public MFilesDocument MFilesDocument { get; set; }

        [Required]
        [InverseProperty("Titles")]
        public Document Document { get; set; }

        [Required]
        [StringLength(3)]
        public string Language { get; set; }

        [Required]
        [StringLength(255)]
        public string LanguageFull { get; set; }

        [Required(AllowEmptyStrings = true)]
        [DefaultValue("")]
        public string Value { get; set; }
    }


    public class Description
    {
        [Key, ForeignKey("MFilesDocument")]
        public Guid DescriptionId { get; set; }

        public MFilesDocument MFilesDocument { get; set; }

        [Required]
        [InverseProperty("Descriptions")]
        public Document Document { get; set; }

        [Required]
        [StringLength(3)]
        public string Language { get; set; }

        [Required]
        [StringLength(255)]
        public string LanguageFull { get; set; }

        [Required(AllowEmptyStrings = true)]
        [DefaultValue("")]
        public string Value { get; set; }
    }


    public class LeoTerm
    {
        [Key]
        public Guid LeoTermId { get; set; }

        public string Name { get; set; }
        public string Url { get; set; }

        public virtual ICollection<ListProperty> Properties { get; set; }
    }
}
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace MultivendorWebViewer.Models
{
    public partial class MultivendorModel : DbContext
    {
        public MultivendorModel()
            : base("name=MultivendorModelContext")
        {
        }

        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<CategoryImage> CategoryImages { get; set; }
        public virtual DbSet<CategoryNode> CategoryNodes { get; set; }
        public virtual DbSet<Image> Images { get; set; }
        public virtual DbSet<NodeImage> NodeImages { get; set; }
        public virtual DbSet<Node> Nodes { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<PriceAvailability> PriceAvailabilities { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<ProductImage> ProductImages { get; set; }
        public virtual DbSet<ProductNode> ProductNodes { get; set; }
        public virtual DbSet<ProductSpecification> ProductSpecifications { get; set; }
        public virtual DbSet<Specification> Specifications { get; set; }
        public virtual DbSet<SpecificationType> SpecificationTypes { get; set; }
        public virtual DbSet<SubNode> SubNodes { get; set; }
        public virtual DbSet<Text> Texts { get; set; }
        public virtual DbSet<TextTranslation> TextTranslations { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

            InitializeCategory(modelBuilder);
            InitializeCategoryImage(modelBuilder);
            InitializeCategoryNode(modelBuilder);
        }

        protected virtual void InitializeCategory(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().ToTable("Category");
            modelBuilder.Entity<Category>().HasMany<CategoryNode>(i => i.CategoryNodes);
            
            modelBuilder.Entity<Category>().HasMany<CategoryImage>(i => i.CategoryImages);
            modelBuilder.Entity<Category>().HasOptional<Text>(i => i.Name);
            modelBuilder.Entity<Category>().HasOptional<Text>(i => i.Description);


            modelBuilder.Entity<Category>().HasKey<int>(i => i.Id).Property(i => i.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
        }


        protected virtual void InitializeCategoryImage(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CategoryImage>().ToTable("CategoryImage");
            modelBuilder.Entity<CategoryImage>().HasRequired<Category>(i => i.Category);
            modelBuilder.Entity<CategoryImage>().HasRequired<Image>(i => i.Image);


            modelBuilder.Entity<Category>().HasKey<int>(i => i.Id).Property(i => i.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
        }

        protected virtual void InitializeCategoryNode(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CategoryNode>().ToTable("CategoryNode");
            modelBuilder.Entity<CategoryNode>().HasRequired<Category>(i => i.Category);
            modelBuilder.Entity<CategoryNode>().HasRequired<Node>(i => i.Node);


            modelBuilder.Entity<Category>().HasKey<int>(i => i.Id).Property(i => i.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
        }

    }

}

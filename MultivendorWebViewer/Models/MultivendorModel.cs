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
            InitializeImage(modelBuilder);
            InitializeNode(modelBuilder);
            InitializeNodeImage(modelBuilder);
            InitializeOrder(modelBuilder);
            InitializePriceAvailability(modelBuilder);
            InitializeProduct(modelBuilder);
            InitializeProductImage(modelBuilder);
            InitializeProductNode(modelBuilder);
            InitializeProductSpecification(modelBuilder);
            InitializeSpecification(modelBuilder);
            InitializeSpecificationType(modelBuilder);
            InitializeSubNode(modelBuilder);
            InitializeText(modelBuilder);
            InitializeTextTranslation(modelBuilder);
            InitializeText(modelBuilder);

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

        protected virtual void InitializeImage(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Image>().ToTable("Image");

            modelBuilder.Entity<Category>().HasKey<int>(i => i.Id).Property(i => i.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
        }

      
        protected virtual void InitializeNode(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Node>().ToTable("Node");
            modelBuilder.Entity<Node>().HasRequired<Text>(i => i.Name);
            modelBuilder.Entity<Node>().HasRequired<Text>(i => i.Description);


            modelBuilder.Entity<Category>().HasKey<int>(i => i.Id).Property(i => i.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
        }

        protected virtual void InitializeNodeImage(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<NodeImage>().ToTable("NodeImage");
            modelBuilder.Entity<NodeImage>().HasRequired<Image>(i => i.Image);
            modelBuilder.Entity<NodeImage>().HasRequired<Node>(i => i.Node);


            modelBuilder.Entity<Category>().HasKey<int>(i => i.Id).Property(i => i.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
        }

        protected virtual void InitializeOrder(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>().ToTable("Order");
            modelBuilder.Entity<Order>().HasRequired<Product>(i => i.Product);
            modelBuilder.Entity<Order>().HasRequired<User>(i => i.User);


            modelBuilder.Entity<Category>().HasKey<int>(i => i.Id).Property(i => i.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
        }

        protected virtual void InitializePriceAvailability(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PriceAvailability>().ToTable("PriceAvailability");
            modelBuilder.Entity<PriceAvailability>().HasRequired<Product>(i => i.Product);
            modelBuilder.Entity<PriceAvailability>().HasRequired<User>(i => i.User);


            modelBuilder.Entity<Category>().HasKey<int>(i => i.Id).Property(i => i.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

        }

        protected virtual void InitializeProduct(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>().ToTable("Product");
            modelBuilder.Entity<Product>().HasRequired<Text>(i => i.Name);
            modelBuilder.Entity<Product>().HasRequired<Text>(i => i.Description);


            modelBuilder.Entity<Category>().HasKey<int>(i => i.Id).Property(i => i.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

        }

        protected virtual void InitializeProductImage(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductImage>().ToTable("ProductImage");
            modelBuilder.Entity<ProductImage>().HasRequired<Image>(i => i.Image);
            modelBuilder.Entity<ProductImage>().HasRequired<Product>(i => i.Product);


            modelBuilder.Entity<Category>().HasKey<int>(i => i.Id).Property(i => i.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

        }

        protected virtual void InitializeProductNode(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductNode>().ToTable("ProductNode");
            modelBuilder.Entity<ProductNode>().HasRequired<Node>(i => i.Node);
            modelBuilder.Entity<ProductNode>().HasRequired<Product>(i => i.Product);


            modelBuilder.Entity<Category>().HasKey<int>(i => i.Id).Property(i => i.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

        }

        protected virtual void InitializeProductSpecification(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductSpecification>().ToTable("ProductSpecification");
            modelBuilder.Entity<ProductSpecification>().HasRequired<Product>(i => i.Product);
            modelBuilder.Entity<ProductSpecification>().HasRequired<Specification>(i => i.Specification);


            modelBuilder.Entity<Category>().HasKey<int>(i => i.Id).Property(i => i.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

        }

        protected virtual void InitializeSpecification(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Specification>().ToTable("Specification");
            modelBuilder.Entity<Specification>().HasRequired<SpecificationType>(i => i.SpecificationType);
            modelBuilder.Entity<Specification>().HasRequired<Text>(i => i.SpecificationText);


            modelBuilder.Entity<Category>().HasKey<int>(i => i.Id).Property(i => i.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

        }

        protected virtual void InitializeSpecificationType(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SpecificationType>().ToTable("SpecificationType");
            modelBuilder.Entity<SpecificationType>().HasRequired<Text>(i => i.SpecificationTypeText);


            modelBuilder.Entity<Category>().HasKey<int>(i => i.Id).Property(i => i.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

        }

        protected virtual void InitializeSubNode(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SubNode>().ToTable("SubNode");
            modelBuilder.Entity<SubNode>().HasRequired<Node>(i => i.Node);
            modelBuilder.Entity<SubNode>().HasRequired<Node>(i => i.SubNodeItem);

            modelBuilder.Entity<Category>().HasKey<int>(i => i.Id).Property(i => i.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

        }

        protected virtual void InitializeText(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Text>().ToTable("Text");

            modelBuilder.Entity<Category>().HasKey<int>(i => i.Id).Property(i => i.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

        }

        protected virtual void InitializeTextTranslation(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TextTranslation>().ToTable("TextTranslation");
            modelBuilder.Entity<TextTranslation>().HasRequired<Text>(i => i.Text);

            modelBuilder.Entity<Category>().HasKey<int>(i => i.Id).Property(i => i.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

        }

        protected virtual void InitializeUser(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("User");
           
            modelBuilder.Entity<Category>().HasKey<int>(i => i.Id).Property(i => i.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

        }


    }

} 

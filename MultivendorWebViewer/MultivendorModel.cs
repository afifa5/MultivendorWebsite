using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using MultivendorWebViewer.Models;

namespace MultivendorWebViewer
{
    public partial class MultivendorModel : DbContext
    {
        public MultivendorModel()
            : base("name=MultivendorModelContext")
        {
            this.Configuration.LazyLoadingEnabled =false;
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
        public virtual DbSet<OrderLine> OrderLines { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //InitializeCategory(modelBuilder);
            //InitializeCategoryImage(modelBuilder);
            //InitializeCategoryNode(modelBuilder);
            //InitializeImage(modelBuilder);
            //InitializeNode(modelBuilder);
            //InitializeNodeImage(modelBuilder);
            //InitializeOrder(modelBuilder);
            //InitializePriceAvailability(modelBuilder);
            //InitializeProduct(modelBuilder);
            //InitializeProductImage(modelBuilder);
            //InitializeProductNode(modelBuilder);
            //InitializeProductSpecification(modelBuilder);
            //InitializeSpecification(modelBuilder);
            //InitializeSpecificationType(modelBuilder);
            //InitializeSubNode(modelBuilder);
            //InitializeText(modelBuilder);
            //InitializeTextTranslation(modelBuilder);
            //InitializeOrderLine(modelBuilder);
            //InitializeCustomer(modelBuilder);

        }

        protected virtual void InitializeCategory(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().ToTable("Category");
            modelBuilder.Entity<Category>().HasMany<CategoryNode>(i => i.CategoryNodes).WithMany();

            modelBuilder.Entity<Category>().HasMany<CategoryImage>(i => i.CategoryImages);
            modelBuilder.Entity<Category>().HasOptional<Text>(i => i.Name);
            modelBuilder.Entity<Category>().HasOptional<Text>(i => i.Description);


            modelBuilder.Entity<Category>().HasKey<int>(i => i.Id).Property(i => i.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
        }


        protected virtual void InitializeCategoryImage(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CategoryImage>().ToTable("CategoryImage");
            //modelBuilder.Entity<CategoryImage>().HasRequired<Category>(i => i.Category);
            modelBuilder.Entity<CategoryImage>().HasRequired<Image>(i => i.Image);


            modelBuilder.Entity<CategoryImage>().HasKey<int>(i => i.Id).Property(i => i.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
        }

        protected virtual void InitializeCategoryNode(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CategoryNode>().ToTable("CategoryNode");
            //modelBuilder.Entity<CategoryNode>().HasRequired<Category>(i => i.Category);
            modelBuilder.Entity<CategoryNode>().HasRequired<Node>(i => i.Node);


            modelBuilder.Entity<CategoryNode>().HasKey<int>(i => i.Id).Property(i => i.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
        }

        protected virtual void InitializeImage(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Image>().ToTable("Image");

            modelBuilder.Entity<Image>().HasKey<int>(i => i.Id).Property(i => i.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
        }

      
        protected virtual void InitializeNode(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Node>().ToTable("Node");
            modelBuilder.Entity<Node>().HasOptional<Text>(i => i.Name);
            modelBuilder.Entity<Node>().HasOptional<Text>(i => i.Description);


            modelBuilder.Entity<Node>().HasKey<int>(i => i.Id).Property(i => i.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
        }

        protected virtual void InitializeNodeImage(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<NodeImage>().ToTable("NodeImage");
            modelBuilder.Entity<NodeImage>().HasRequired<Image>(i => i.Image);
            //modelBuilder.Entity<NodeImage>().HasRequired<Node>(i => i.Node);


            modelBuilder.Entity<NodeImage>().HasKey<int>(i => i.Id).Property(i => i.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
        }

        protected virtual void InitializeOrder(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>().ToTable("Order");
            modelBuilder.Entity<Order>().HasRequired<Customer>(i => i.Customer);
            modelBuilder.Entity<Order>().HasKey<int>(i => i.Id).Property(i => i.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
        }
        protected virtual void InitializeOrderLine(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrderLine>().ToTable("OrderLine");

            modelBuilder.Entity<OrderLine>().HasRequired<Product>(i => i.Product);
            //modelBuilder.Entity<OrderLine>().HasRequired<User>(i => i.User);

            modelBuilder.Entity<OrderLine>().HasKey<int>(i => i.Id).Property(i => i.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
        }
        protected virtual void InitializePriceAvailability(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PriceAvailability>().ToTable("PriceAvailability");
            //modelBuilder.Entity<PriceAvailability>().HasRequired<Product>(i => i.Product);
            //modelBuilder.Entity<PriceAvailability>().HasRequired<User>(i => i.User);


            modelBuilder.Entity<PriceAvailability>().HasKey<int>(i => i.Id).Property(i => i.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

        }

        protected virtual void InitializeProduct(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>().ToTable("Product");
            modelBuilder.Entity<Product>().HasOptional<Text>(i => i.Name);
            modelBuilder.Entity<Product>().HasOptional<Text>(i => i.Description);
            modelBuilder.Entity<Product>().HasKey<int>(i => i.Id).Property(i => i.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

        }

        protected virtual void InitializeProductImage(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductImage>().ToTable("ProductImage");
            modelBuilder.Entity<ProductImage>().HasRequired<Image>(i => i.Image);
            //modelBuilder.Entity<ProductImage>().HasRequired<Product>(i => i.Product);


            modelBuilder.Entity<ProductImage>().HasKey<int>(i => i.Id).Property(i => i.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

        }

        protected virtual void InitializeProductNode(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductNode>().ToTable("ProductNode");
            //modelBuilder.Entity<ProductNode>().HasRequired<Node>(i => i.Node);
            modelBuilder.Entity<ProductNode>().HasRequired<Product>(i => i.Product);


            modelBuilder.Entity<ProductNode>().HasKey<int>(i => i.Id).Property(i => i.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

        }

        protected virtual void InitializeProductSpecification(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductSpecification>().ToTable("ProductSpecification");
            //modelBuilder.Entity<ProductSpecification>().HasRequired<Product>(i => i.Product);
            modelBuilder.Entity<ProductSpecification>().HasRequired<Specification>(i => i.Specification);


            modelBuilder.Entity<ProductSpecification>().HasKey<int>(i => i.Id).Property(i => i.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

        }

        protected virtual void InitializeSpecification(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Specification>().ToTable("Specification");
            modelBuilder.Entity<Specification>().HasRequired<SpecificationType>(i => i.SpecificationType);
            modelBuilder.Entity<Specification>().HasOptional<Text>(i => i.SpecificationText);


            modelBuilder.Entity<Specification>().HasKey<int>(i => i.Id).Property(i => i.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

        }

        protected virtual void InitializeSpecificationType(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SpecificationType>().ToTable("SpecificationType");
            modelBuilder.Entity<SpecificationType>().HasOptional<Text>(i => i.SpecificationTypeText);


            modelBuilder.Entity<SpecificationType>().HasKey<int>(i => i.Id).Property(i => i.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

        }

        protected virtual void InitializeSubNode(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SubNode>().ToTable("SubNode");
            //modelBuilder.Entity<SubNode>().HasRequired<Node>(i => i.Node);
            modelBuilder.Entity<SubNode>().HasRequired<Node>(i => i.SubNodeItem);

            modelBuilder.Entity<SubNode>().HasKey<int>(i => i.Id).Property(i => i.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

        }

        protected virtual void InitializeText(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Text>().ToTable("Text");
            modelBuilder.Entity<Text>().HasMany<TextTranslation>(i =>i.TextTranslations );

            modelBuilder.Entity<Text>().HasKey<int>(i => i.Id).Property(i => i.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

        }
        protected virtual void InitializeCustomer(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>().ToTable("Customer");
            modelBuilder.Entity<Customer>().HasKey<int>(i => i.Id).Property(i => i.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

        }
        protected virtual void InitializeTextTranslation(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TextTranslation>().ToTable("TextTranslation");
            modelBuilder.Entity<TextTranslation>().HasRequired<Text>(i => i.Text);

            modelBuilder.Entity<TextTranslation>().HasKey<int>(i => i.Id).Property(i => i.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

        }

        protected virtual void InitializeUser(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("User");
           
            modelBuilder.Entity<User>().HasKey<int>(i => i.Id).Property(i => i.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

        }


    }

} 

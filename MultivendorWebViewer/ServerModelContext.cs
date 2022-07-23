using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using MultivendorWebViewer.DbMigrations;
using MultivendorWebViewer.Server.Models;

namespace MultivendorWebViewer
{
    public partial class ServerModelContext : DbContext
    {
        public ServerModelContext()
        {
            SetupContext();
            Database.SetInitializer<ServerModelContext>(new ServerDBInitializer());
        }
        public ServerModelContext(string connectionstring)
            : base(connectionstring/*"name=MultivendorModelContext"*/)
        {
            SetupContext();
            Database.SetInitializer<ServerModelContext>(new ServerDBInitializer());
            //this.Configuration.LazyLoadingEnabled =false;
        }
        public static ServerModelContext Create()
        {
            return Instance.Create<ServerModelContext>();
        }


        public ServerModelContext(DbConnection existingConnection)
            : base(existingConnection, false)
        {
            SetupContext();
            Database.SetInitializer<ServerModelContext>(new ServerDBInitializer());
        }

        public ServerModelContext(DbConnection existingConnection, DbCompiledModel model)
            : base(existingConnection, model, false)
        {
            Database.SetInitializer<ServerModelContext>(new ServerDBInitializer());
        }



        public static ServerModelContext Create(DbConnection existingConnection)
        {
            return Instance.Create<DbConnection, ServerModelContext>(existingConnection);
        }

        public static ServerModelContext Create(DbConnection existingConnection, DbCompiledModel model)
        {
            return Instance.Create<DbConnection, DbCompiledModel, ServerModelContext>(existingConnection, model);
        }
        public virtual void SetupContext()
        {
            Configuration.LazyLoadingEnabled = false;
            Configuration.ProxyCreationEnabled = false;

            // DANGEROUS! 
#if DEBUG
            Configuration.AutoDetectChangesEnabled = true;
            Configuration.ValidateOnSaveEnabled = true;
#else
            Configuration.AutoDetectChangesEnabled = false;
            Configuration.ValidateOnSaveEnabled = false;
#endif
        }


        public virtual DbSet<Order> Orders { get; set; }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<OrderLine> OrderLines { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<KnownProperty> KnownProperties { get; set; }
        public virtual DbSet<Payment> Payments { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }


    }

} 

using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using MultivendorWebViewer.Server.Models;

namespace MultivendorWebViewer
{
    public partial class ServerModelContext : DbContext
    {
        public ServerModelContext(string connectionstring)
            : base(connectionstring/*"name=MultivendorModelContext"*/)
        {
            //this.Configuration.LazyLoadingEnabled =false;
        }
        public static ServerModelContext Create()
        {
            return Instance.Create<ServerModelContext>();
        }

        public static ServerModelContext Create(DbConnection existingConnection)
        {
            return Instance.Create<DbConnection, ServerModelContext>(existingConnection);
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

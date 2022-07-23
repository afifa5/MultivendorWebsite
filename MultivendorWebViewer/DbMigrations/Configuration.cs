namespace MultivendorWebViewer.DbMigrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<MultivendorWebViewer.ServerModelContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            ContextKey = "MultivendorWebViewer.ServerModelContext";
            this.CommandTimeout = 1800;
        }

    }
}

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace MultivendorWebViewer.DbMigrations
{
    public class ServerDBInitializer : CreateDatabaseIfNotExists<ServerModelContext>
    {
        protected override void Seed(ServerModelContext context)
        {
            base.Seed(context);
        }
    }
}

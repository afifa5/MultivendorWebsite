using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace MultivendorWebViewer.DbMigrations
{
    public class SiteDBInitializer : CreateDatabaseIfNotExists<MultivendorModelContext>
    {
        protected override void Seed(MultivendorModelContext context)
        {
            base.Seed(context);
        }
    }
}

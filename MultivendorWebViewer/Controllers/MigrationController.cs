using Microsoft.AspNet.Identity;
using MultivendorWebViewer.Common;
using MultivendorWebViewer.Configuration;
using MultivendorWebViewer.DbMigrations;
using MultivendorWebViewer.Helpers;
using MultivendorWebViewer.Models;
using MultivendorWebViewer.Server.Models;
using MultivendorWebViewer.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MultivendorWebViewer.Controllers
{
    public class MigrationController : BaseController
    {
        // GET: WebAdmin
        public override ActionResult Index()
        {
            return View();
        }

        #region DatabaseMigrations
        [HttpGet]
        public ActionResult DatabaseMigrationView()
        {
            ServerDatabaseMigrationDetail migrationdetail = new ServerDatabaseMigrationDetail();

            var dbconnection = ServerModelDatabaseContextManager.Default.GetConnection();
            var connectionstring = dbconnection.ConnectionString;

            ServerDatabaseMigrator serverdatabasemigratior = new ServerDatabaseMigrator(connectionstring);

            var configuration = serverdatabasemigratior.Configuration;

            var migrator = new DbMigrator(configuration);

            var appliedmigration = migrator.GetDatabaseMigrations();

            DefaultInitialCreate initialcreate = new DefaultInitialCreate();
            //if (appliedmigration.Count() == 1)
            //{
            //    serverdatabasemigratior.ResetInitialCreate(dbconnection, initialcreate);
            //}
            var default_initialcreate = initialcreate.MigrationId.Substring(2);

            var latestinitialcreate = default_initialcreate.Substring(0, default_initialcreate.Length - 1);

            var validdatabasewithmigration = appliedmigration.Contains(latestinitialcreate);

            migrationdetail.EnableMigarationForDatabase = validdatabasewithmigration == true ? true : false;

            foreach (var item in appliedmigration.Reverse())
            {
                migrationdetail.AppliedMigrationNames.Add(item);
            }

            var pendingmigraiton = migrator.GetPendingMigrations();

            foreach (var item in pendingmigraiton)
            {
                migrationdetail.PendingMigraitonNames.Add(item);
                migrationdetail.PendingMigrationDetails.Add(PendingMigrationInformation(item));
            }

            var comptiable = serverdatabasemigratior.IsCurrentServerDatabaseCompatibleWithModel(dbconnection);

            migrationdetail.CurrentServerDatabaseCompatibleWithCurrentModel = comptiable;

            return View("_DatabaseMigrationView", migrationdetail);
        }

        [HttpPost]
        public ActionResult UpdateDatabaseMigration()
        {
            var dbconnection = ServerModelDatabaseContextManager.Default.GetConnection();
            var connectionstring = dbconnection.ConnectionString;

            ServerDatabaseMigrator serverdatabasemigratior = new ServerDatabaseMigrator(connectionstring);

            var configuration = serverdatabasemigratior.Configuration;

            var migrator = new DbMigrator(configuration);

            var pendingmigraiton = migrator.GetPendingMigrations();

            string runningmigration = string.Empty;

            try
            {
                foreach (var item in pendingmigraiton)
                {
                    runningmigration = item;
                    migrator.Update(item);
                }
                return Json(new { result = "true", message = string.Format("Migration done.") });
            }
            catch (Exception e)
            {
                return Json(new { result = "false", message = string.Format("Migration failed at {0}. <br/> Exception Message : {1} <br/> InnerException : {2}", runningmigration, e.Message, e.InnerException) });
            }

        }
        public PendingMigratioDetail PendingMigrationInformation(string migrationname)
        {
            PendingMigratioDetail detail = new PendingMigratioDetail();

            switch (migrationname)
            {
                case "202007211138512_mig_refactoring2_column_tables_32":

                    List<string> information = new List<string>();

                    information.Add("Users attached to non-existing price group will have their price group set to NULL.");
                    information.Add("Organizations attached to non-existing price group will have their price group set to NULL.");

                    detail.PendingMigrationInformation = information;
                    detail.PendingMigrationName = migrationname;

                    break;

            }


            return detail;

        }
        public bool IsServerDatabaseValid(string serverDatabaseName)
        {
            try
            {
                using (var context = ServerModelDatabaseContextManager.Default.Context())
                {
                    var user = context.Users.FirstOrDefault();
                    if (user == null)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        #endregion


    }
}

using System;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Diagnostics;


namespace MultivendorWebViewer.Common
{
    public class ContextManager : SingletonBase<ContextManager>
    {
        private static bool databaseExistenceChecked = false;

        public ContextManager()
        {
            DbConfiguration.SetConfiguration(MultivendorDbConfiguration.Create());
        }

        public string GetConnectionString(string instance = null, string databaseName = null)
        {
            ServerConfiguration serverConfiguration = ServerConfigurationProvider.Default.Configuration;
            if (serverConfiguration.SiteDatabase == null || serverConfiguration.SiteDatabase.Enabled == false) return null;

            SiteDatabase SiteDatabase = serverConfiguration.SiteDatabase;
            instance = instance ?? SiteDatabase.DataBaseServer;
            databaseName = databaseName ?? SiteDatabase.DataBaseName;

            if (string.IsNullOrEmpty(instance) == true) throw new ArgumentNullException("DataBaseServer");
            if (string.IsNullOrEmpty(databaseName) == true) throw new ArgumentNullException("DatabaseName");

            string connectionString = String.Format("Data Source={0};Initial Catalog={1};MultipleActiveResultSets=True;Connection Timeout={2};", instance, databaseName, serverConfiguration.AttachDatabaseTimeout);

            if (SiteDatabase.UseSqlServerUser)
                connectionString += String.Format("User Id='{0}'; Password='{1}';Persist Security Info=True;", SiteDatabase.SqlServerUser, SiteDatabase.SqlServerPassword);
            else
                connectionString += "Integrated Security=true;";

            return connectionString;
        }

        public virtual DbConnection GetConnection(string instance = null, string databaseName = null)
        {
            // Get sqlserver connect string
            string serverConnectString = GetConnectionString(instance, databaseName);
            if (serverConnectString == null) return null;

            IDbConnectionFactory factory = new SqlConnectionFactory();
            DbConnection connection = factory.CreateConnection(serverConnectString);

            if (databaseExistenceChecked == true)
            {
                databaseExistenceChecked = false; // bool is atomic. Worst case we can multiple calls to below code
                if (Database.Exists(connection) == false)
                {
                    // Create non-existing database
                    MultivendorModelContext context = MultivendorModelContext.Create(connection);
                    context.Database.Initialize(true);

                    if (Database.Exists(connection) == false)
                    {
                        throw new Exception("Database not found for: " + serverConnectString);
                    }
                }
            }

            return connection;
        }

        public virtual MultivendorModelContext Context(string instance = null, string databaseName = null)
        {
            try
            {
                DbConnection connection = GetConnection(instance, databaseName);
                MultivendorModelContext context = MultivendorModelContext.Create(connection);
                return context;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }

    public class SiteModelDatabaseContextManager : SingletonBase<SiteModelDatabaseContextManager>
    {
        private static bool databaseExistenceChecked = false;

        public SiteModelDatabaseContextManager()
        {
            DbConfiguration.SetConfiguration(MultivendorDbConfiguration.Create());
        }

        public string GetConnectionString(string instance = null, string databaseName = null)
        {
            ServerConfiguration serverConfiguration = ServerConfigurationProvider.Default.Configuration;
            if (serverConfiguration.SiteDatabase == null || serverConfiguration.SiteDatabase.Enabled == false) return null;

            SiteDatabase SiteDatabase = serverConfiguration.SiteDatabase;
            instance = instance ?? SiteDatabase.DataBaseServer;
            databaseName = databaseName ?? SiteDatabase.DataBaseName;

            if (string.IsNullOrEmpty(instance) == true) throw new ArgumentNullException("DataBaseServer");
            if (string.IsNullOrEmpty(databaseName) == true) throw new ArgumentNullException("DatabaseName");

            string connectionString = String.Format("Data Source={0};Initial Catalog={1};MultipleActiveResultSets=True;Connection Timeout={2};", instance, databaseName, serverConfiguration.AttachDatabaseTimeout);

            if (SiteDatabase.UseSqlServerUser)
                connectionString += String.Format("User Id='{0}'; Password='{1}';Persist Security Info=True;", SiteDatabase.SqlServerUser, SiteDatabase.SqlServerPassword);
            else
                connectionString += "Integrated Security=true;";

            return connectionString;
        }

        public virtual SqlConnection GetConnection(string instance = null, string databaseName = null)
        {
            // Get sqlserver connect string
            string serverConnectString = GetConnectionString(instance, databaseName);
            if (serverConnectString == null) return null;

            // Create and open connection
            SqlConnection connection = new SqlConnection(serverConnectString);
            connection.Open();

            if (databaseExistenceChecked == true)
            {
                databaseExistenceChecked = false; // bool is atomic. Worst case we can multiple calls to below code
                if (Database.Exists(connection) == false)
                {
                    // Create non-existing database
                    MultivendorModelContext context = MultivendorModelContext.Create(connection);
                    context.Database.Initialize(true);

                    if (Database.Exists(connection) == false)
                    {
                        throw new Exception("Database not found for: " + serverConnectString);
                    }
                }
            }

            return connection;
        }

        public virtual MultivendorModelContext Context(string instance = null, string databaseName = null)
        {
            try
            {
                SqlConnection connection = GetConnection(instance, databaseName);
                if (connection == null) return null;

                return MultivendorModelContext.Create(connection);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }

    public class ServerContextManager : SingletonBase<ContextManager>
    {
        private static bool databaseExistenceChecked = false;

        public ServerContextManager()
        {
            DbConfiguration.SetConfiguration(MultivendorDbConfiguration.Create());
        }

        public string GetConnectionString(string instance = null, string databaseName = null)
        {
            ServerConfiguration serverConfiguration = ServerConfigurationProvider.Default.Configuration;
            if (serverConfiguration.ServerDatabase == null || serverConfiguration.ServerDatabase.Enabled == false) return null;

            ServerDatabase serverDatabase = serverConfiguration.ServerDatabase;
            instance = instance ?? serverDatabase.DataBaseServer;
            databaseName = databaseName ?? serverDatabase.DataBaseName;

            if (string.IsNullOrEmpty(instance) == true) throw new ArgumentNullException("DataBaseServer");
            if (string.IsNullOrEmpty(databaseName) == true) throw new ArgumentNullException("DatabaseName");

            string connectionString = String.Format("Data Source={0};Initial Catalog={1};MultipleActiveResultSets=True;Connection Timeout={2};", instance, databaseName, serverConfiguration.AttachDatabaseTimeout);

            if (serverDatabase.UseSqlServerUser)
                connectionString += String.Format("User Id='{0}'; Password='{1}';Persist Security Info=True;", serverDatabase.SqlServerUser, serverDatabase.SqlServerPassword);
            else
                connectionString += "Integrated Security=true;";

            return connectionString;
        }

        public virtual DbConnection GetConnection(string instance = null, string databaseName = null)
        {
            // Get sqlserver connect string
            string serverConnectString = GetConnectionString(instance, databaseName);
            if (serverConnectString == null) return null;

            IDbConnectionFactory factory = new SqlConnectionFactory();
            DbConnection connection = factory.CreateConnection(serverConnectString);

            if (databaseExistenceChecked == true)
            {
                databaseExistenceChecked = false; // bool is atomic. Worst case we can multiple calls to below code
                if (Database.Exists(connection) == false)
                {
                    // Create non-existing database
                    ServerModelContext context = ServerModelContext.Create(connection);
                    context.Database.Initialize(true);

                    if (Database.Exists(connection) == false)
                    {
                        throw new Exception("Database not found for: " + serverConnectString);
                    }
                }
            }

            return connection;
        }

        public virtual ServerModelContext Context(string instance = null, string databaseName = null)
        {
            try
            {
                DbConnection connection = GetConnection(instance, databaseName);
                ServerModelContext context = ServerModelContext.Create(connection);
                return context;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }

    public class ServerModelDatabaseContextManager : SingletonBase<ServerModelDatabaseContextManager>
    {
        private static bool databaseExistenceChecked = false;

        public ServerModelDatabaseContextManager()
        {
            DbConfiguration.SetConfiguration(MultivendorDbConfiguration.Create());
        }

        public string GetConnectionString(string instance = null, string databaseName = null)
        {
            ServerConfiguration serverConfiguration = ServerConfigurationProvider.Default.Configuration;
            if (serverConfiguration.ServerDatabase == null || serverConfiguration.ServerDatabase.Enabled == false) return null;

            ServerDatabase serverDatabase = serverConfiguration.ServerDatabase;
            instance = instance ?? serverDatabase.DataBaseServer;
            databaseName = databaseName ?? serverDatabase.DataBaseName;

            if (string.IsNullOrEmpty(instance) == true) throw new ArgumentNullException("DataBaseServer");
            if (string.IsNullOrEmpty(databaseName) == true) throw new ArgumentNullException("DatabaseName");

            string connectionString = String.Format("Data Source={0};Initial Catalog={1};MultipleActiveResultSets=True;Connection Timeout={2};", instance, databaseName, serverConfiguration.AttachDatabaseTimeout);

            if (serverDatabase.UseSqlServerUser)
                connectionString += String.Format("User Id='{0}'; Password='{1}';Persist Security Info=True;", serverDatabase.SqlServerUser, serverDatabase.SqlServerPassword);
            else
                connectionString += "Integrated Security=true;";

            return connectionString;
        }

        public virtual SqlConnection GetConnection(string instance = null, string databaseName = null)
        {
            // Get sqlserver connect string
            string serverConnectString = GetConnectionString(instance, databaseName);
            if (serverConnectString == null) return null;

            // Create and open connection
            SqlConnection connection = new SqlConnection(serverConnectString);
            connection.Open();

            if (databaseExistenceChecked == true)
            {
                databaseExistenceChecked = false; // bool is atomic. Worst case we can multiple calls to below code
                if (Database.Exists(connection) == false)
                {
                    // Create non-existing database
                    ServerModelContext context = ServerModelContext.Create(connection);
                    context.Database.Initialize(true);

                    if (Database.Exists(connection) == false)
                    {
                        throw new Exception("Database not found for: " + serverConnectString);
                    }
                }
            }

            return connection;
        }

        public virtual ServerModelContext Context(string instance = null, string databaseName = null)
        {
            try
            {
                SqlConnection connection = GetConnection(instance, databaseName);
                if (connection == null) return null;

                return ServerModelContext.Create(connection);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }

}


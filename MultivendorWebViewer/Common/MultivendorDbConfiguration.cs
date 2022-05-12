using System;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Infrastructure.Interception;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Runtime.Remoting.Messaging;

#if NET452
using System.Runtime.Remoting.Messaging;
using System.Collections.Generic;
using System.Collections.Concurrent;
#else
//using System.Runtime.Remoting.Messaging;
#endif
#if NET5
using System.Collections.Concurrent;
using System.Threading;
#endif

namespace MultivendorWebViewer.Common
{
    public class MultivendorDbConfiguration : DbConfiguration
    {
        public enum ConnectType { None, SqlServer, LocalDb, MultivendorSite };

        public static ConnectType? ConnectionType
        {
            get { return CallContext.LogicalGetData("ConnectionType") as ConnectType?; }
            set { CallContext.LogicalSetData("ConnectionType", value); }
        }

        public static string SqlServerConnectionString
        {
            get { return CallContext.LogicalGetData("SqlServerConnectionString") as string; }
            set { CallContext.LogicalSetData("SqlServerConnectionString", value); }
        }

        public static string LocalDbPath
        {
            get { return CallContext.LogicalGetData("LocalDbPath") as string; }
            set { CallContext.LogicalSetData("LocalDbPath", value); }
        }

        public static bool SuspendExecutionStrategy
        {
            get { return (bool?)CallContext.LogicalGetData("SuspendExecutionStrategy") ?? false; }
            set { CallContext.LogicalSetData("SuspendExecutionStrategy", value); }
        }

        public MultivendorDbConfiguration()
            : base()
        {
            AddInterceptor(new TempTableCommandInterceptor());

            AddInterceptor(new TempTableCommandTreeInterceptor());

            this.SetExecutionStrategy("System.Data.SqlClient", () =>
                SuspendExecutionStrategy ? (IDbExecutionStrategy)new DefaultExecutionStrategy() : new SqlAzureExecutionStrategy());

            if (ConnectionType == null || ConnectionType == ConnectType.None) return;

            if (ConnectionType == ConnectType.SqlServer)
            {
                if (String.IsNullOrEmpty(SqlServerConnectionString)) throw new ArgumentNullException("SqlServerConnectionString");

                SqlConnectionFactory factory = new SqlConnectionFactory(SqlServerConnectionString);
                SetDefaultConnectionFactory(factory);

                SetProviderServices(SqlProviderServices.ProviderInvariantName, SqlProviderServices.Instance);
            }
            else if (ConnectionType == ConnectType.LocalDb)
            {
                if (String.IsNullOrEmpty(LocalDbPath)) throw new ArgumentNullException("LocalDbPath");

                string dbServer = "(localdb)";
                string dbInstance = "V11.0";
                string connectString = String.Format("Server={0}\\{1};Integrated Security=true;AttachDbFileName={2};", dbServer, dbInstance, LocalDbPath);
                LocalDbConnectionFactory factory = new LocalDbConnectionFactory(dbInstance, connectString);
                SetDefaultConnectionFactory(factory);
            }
        }

        public static MultivendorDbConfiguration Create()
        {
            return Instance.Create<MultivendorDbConfiguration>();
        }
    }

#if NET5
    public static class CallContext
    {
        static ConcurrentDictionary<string, AsyncLocal<object>> state = new ConcurrentDictionary<string, AsyncLocal<object>>();
        /// Stores a given object and associates it with the specified name.
        public static void LogicalSetData(string name, object data) =>
            state.GetOrAdd(name, _ => new AsyncLocal<object>()).Value = data;
        /// Retrieves an object with the specified name from the <see cref="CallContext"/>.
        public static object LogicalGetData(string name) =>
            state.TryGetValue(name, out AsyncLocal<object> data) ? data.Value : null;
    }
#endif
    public class TempTableCommandInterceptor : IDbCommandInterceptor
    {
        public void NonQueryExecuted(DbCommand command, DbCommandInterceptionContext<int> interceptionContext)
        {

        }

        public void NonQueryExecuting(DbCommand command, DbCommandInterceptionContext<int> interceptionContext)
        {

        }

        public void ReaderExecuted(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext)
        {

        }

        public void ReaderExecuting(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext)
        {

        }

        public void ScalarExecuted(DbCommand command, DbCommandInterceptionContext<object> interceptionContext)
        {

        }

        public void ScalarExecuting(DbCommand command, DbCommandInterceptionContext<object> interceptionContext)
        {

        }
    }

    public class TempTableCommandTreeInterceptor : IDbCommandTreeInterceptor
    {
        public void TreeCreated(DbCommandTreeInterceptionContext interceptionContext)
        {
        }
    }
}
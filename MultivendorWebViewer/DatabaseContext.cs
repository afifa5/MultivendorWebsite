using MultivendorWebViewer.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Interception;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace MultivendorWebViewer
{
    public class CommandLog
    {
        [Flags]
        public enum LogMode
        {
            None = 0x0,
            NonQueryExecuting = 0x1,
            NonQueryExecuted = 0x2,
            ReaderExecuting = 0x4,
            ReaderExecuted = 0x8,
            ScalarExecuting = 0x10,
            ScalarExecuted = 0x20,
            BulkExecuting = 0x40,
            BulkExecuted = 0x80,
            AllExecuting = NonQueryExecuting | ReaderExecuting | ScalarExecuting | BulkExecuting,
            AllExecuted = NonQueryExecuted | ReaderExecuted | ScalarExecuted | BulkExecuted,
            All = AllExecuting | AllExecuted,
        }

        [Flags]
        public enum LogInformation
        {
            None = 0x0,
            Name = 0x1,
            ExcecutionTime = 0x2,
            CommandText = 0x4,
            ConnectionString = 0x8,
            BulkDestinationTable = 0x100,
            BulkItemCount = 0x200,
            BulkColumnMapping = 0x400,
            BulkTopItems = 0x800,
            RecordsAffected = 0x1000,
            Default = Name | ExcecutionTime | CommandText | RecordsAffected | BulkDestinationTable | BulkItemCount | BulkColumnMapping,
            All = Default | ConnectionString | BulkTopItems,
        }

        public class Entry
        {
            public Entry(object source)
            {
                Source = source;
            }

            public object Source { get; private set; }

            public LogMode Mode { get; set; }

            public string Tag { get; set; }

            public string Name { get; set; }

            public long? ExcecutionTime { get; set; }

            public string CommandText { get; set; }

            public string ConnectionString { get; set; }

            public int? RecordsAffected { get; set; }

            public string BulkDestinationTable { get; set; }

            public int? BulkItemCount { get; set; }

            public string BulkColumnMapping { get; set; }

            public IList BulkTopItems { get; set; }
        }

        public class LogEventArgs : CancelEventArgs
        {
            public LogEventArgs(Entry entry, string text)
            {
                Entry = entry;
                Text = text;
            }

            public Entry Entry { get; private set; }

            public string Text { get; set; }
        }

        public CommandLog(Action<string> logger, LogMode mode = LogMode.AllExecuted, LogInformation information = LogInformation.Default, EventHandler<LogEventArgs> logTextFormattingHandler = null)
        {
            Logger = logger;
            Mode = mode;
            Information = information;
            if (logTextFormattingHandler != null)
            {
                LogTextFormatting += logTextFormattingHandler;
            }
        }

        public event EventHandler<LogEventArgs> LogTextFormatting;


        protected Action<string> Logger { get; private set; }

        public LogMode Mode { get; private set; }

        public LogInformation Information { get; private set; }

        private static readonly double ElapsedTickToMicroSecondsMultiplier = 1000000.0 / Stopwatch.Frequency;

        protected virtual void Log(string text)
        {
            Logger(text);
        }

        protected virtual void OnLogTextFormatting(LogEventArgs e)
        {
            var logTextFormatting = LogTextFormatting;
            if (logTextFormatting != null)
            {
                logTextFormatting(this, e);
            }
        }

        public void LogText(string text)
        {
            Log(text);
        }

        public virtual void LogEntry(Entry entry)
        {
            var info = new StringBuilder();
            if (entry.Name != null)
            {
                info.AppendLine(entry.Name);
            }

            if (entry.ConnectionString != null)
            {
                info.AppendLine(entry.ConnectionString);
            }

            if (entry.CommandText != null)
            {
                info.AppendLine(entry.CommandText);
            }

            if (entry.RecordsAffected.HasValue == true)
            {
                info.AppendLine("Records affected: " + entry.RecordsAffected.Value);
            }

            if (entry.BulkDestinationTable != null)
            {
                info.AppendLine("Insert into table: " + entry.BulkDestinationTable);
            }

            if (entry.BulkColumnMapping != null)
            {
                info.AppendLine("Column mappings: " + entry.BulkColumnMapping);
            }

            if (entry.BulkItemCount.HasValue == true)
            {
                info.AppendLine("Item count: " + entry.BulkItemCount.Value);
            }

            if (entry.BulkTopItems != null)
            {
                info.AppendLine("Top items: " + string.Join(", ", entry.BulkTopItems));
            }

            if (entry.ExcecutionTime.HasValue == true)
            {
                info.AppendLine(string.Format("Execution time in {0:f3} ms", entry.ExcecutionTime.Value / 1000.0));
            }

            var e = new LogEventArgs(entry, info.ToString());

            OnLogTextFormatting(e);

            if (e.Cancel == false)
            {
                Log(e.Text);
            }
        }

        protected void LogExecuting(LogMode modeFlag, object source, Func<object, Entry> entryFactory)
        {
            if (Information != LogInformation.None)
            {
                if (Mode.HasFlag(modeFlag) == true)
                {
                    var entry = entryFactory(source);
                    if (entry != null)
                    {
                        entry.Mode = modeFlag;
                        LogEntry(entry);
                    }
                }

                if (Information.HasFlag(LogInformation.ExcecutionTime) == true && (Mode & LogMode.AllExecuted) != 0)
                {
                    long startTime = Stopwatch.GetTimestamp();
                }
            }
        }

        protected void LogExecuted(LogMode modeFlag, object source, Func<object, Entry> entryFactory, int? recordsAffected = 0)
        {
            if (Information != LogInformation.None)
            {
                if (Mode.HasFlag(modeFlag) == true)
                {
                    long? excecutionTime = null;
                    if (Information.HasFlag(LogInformation.ExcecutionTime) == true)
                    {
                        long stoptime = Stopwatch.GetTimestamp();
                    }

                    var entry = entryFactory(source);
                    if (entry != null)
                    {
                        entry.Mode = modeFlag;
                        entry.ExcecutionTime = excecutionTime;
                        entry.RecordsAffected = recordsAffected;
                        LogEntry(entry);
                    }
                }
            }
        }

        protected virtual Entry GetLogEntry(System.Data.Common.DbCommand command, string name)
        {
            var entry = new Entry(command);
            if (Information.HasFlag(LogInformation.Name) == true) entry.Name = name;
            if (Information.HasFlag(LogInformation.CommandText) == true) entry.CommandText = command.CommandText;
            if (Information.HasFlag(LogInformation.ConnectionString) == true && command.Connection != null) entry.ConnectionString = command.Connection.ConnectionString;
            return entry;
        }

    }

    public class DatabaseContext : IDisposable
    {

        public DatabaseContext(string connectionString)
            : this(new SqlConnection(connectionString))
        {
            Connection.Open();
            ownedConnection = true;
        }
        public DatabaseContext(string connectionString, System.Data.IsolationLevel transactionIsolationLevel, CommandLog log = null)
            : this(new SqlConnection(connectionString), transactionIsolationLevel, log)
        {
            Connection.Open();
            ownedConnection = true;
        }

        public DatabaseContext(SqlConnection connection, System.Data.IsolationLevel transactionIsolationLevel, CommandLog log = null)
            : this(connection, connection.BeginTransaction(transactionIsolationLevel), log)
        {
            ownedTransaction = true;
        }

        public DatabaseContext(SqlConnection connection, SqlTransaction transaction = null, CommandLog log = null)
        {
            Connection = connection;
            Transaction = transaction;
            Log = log;
        }

        public DatabaseContext(IDbConnection connection, SqlTransaction transaction = null, CommandLog log = null)
        {
            Connection = connection as SqlConnection;
            Transaction = transaction;
            Log = log;
        }

        //public DatabaseContext(ModelContext context, SqlTransaction transaction = null, CommandLog log = null)
        //{
        //    Connection = context.Database.Connection as SqlConnection;
        //    Transaction = transaction;
        //    Log = log;
        //}

        private bool ownedConnection;

        private bool ownedTransaction;

        public SqlConnection Connection { get; private set; }

        public SqlTransaction Transaction { get; set; }

        public CommandLog Log { get; set; }

        public SqlCommand CreateCommand(string cmdText = null)
        {
            return new SqlCommand(cmdText, Connection, Transaction) { CommandTimeout = 240 };
        }

        public int ExecuteNonQuery(string cmdText)
        {
            using (var command = CreateCommand(cmdText))
            {
                return command.ExecuteNonQuery();
            }
        }

        public object ExecuteScalar(string cmdText)
        {
            using (var command = CreateCommand(cmdText))
            {
                return command.ExecuteScalar();
            }
        }

        public SqlDataReader ExecuteReader(string cmdText)
        {
            using (var command = CreateCommand(cmdText))
            {
                return command.ExecuteReader();
            }
        }

        public IEnumerable<SqlDataReader> EnumerateReader(string cmdText)
        {
            using (var command = CreateCommand(cmdText))
            {
                var reader = command.ExecuteReader();
                if (reader.HasRows == true)
                {
                    while (reader.Read())
                    {
                        yield return reader;
                    }
                }
            }
        }

       

        public Dictionary<TKey, TItem> ExecuteReaderToDictionary<TKey, TItem>(string cmdText, Func<SqlDataReader, TKey> keySelector, Func<SqlDataReader, TItem> itemSelector, CommandLog log = null)
        {
            using (var command = CreateCommand(cmdText))
            using (var reader = command.ExecuteReader())
            {
                if (reader.HasRows == true)
                {
                    var result = new Dictionary<TKey, TItem>();
                    while (reader.Read())
                    {
                        TKey key = keySelector(reader);
                        TItem item = itemSelector(reader);
                        result.Add(key, item);
                    }
                    return result;
                }
                return new Dictionary<TKey, TItem>(0);
            }
        }

        public void Dispose()
        {
            if (ownedTransaction == true && Transaction != null)
            {
                Transaction.Commit();
                Transaction.Dispose();
            }

            if (ownedConnection == true && Connection != null)
            {
                Connection.Close();
                Connection.Dispose();
            }
        }



        public bool EnableDiagnostics { get; set; }

        // Change log handling
        public string Scope { get; set; }
        public string UserName { get; set; }
    }

}

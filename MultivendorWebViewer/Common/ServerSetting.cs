using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Xml.Serialization;
using System.ComponentModel;
using System.Reflection;
using System.Net.Mail;
using System.Net;

namespace MultivendorWebViewer.Common
{
    [Serializable]
    public class ServerConfiguration : INotifyPropertyChanged, INotifyPropertyChanging
    {


        private SqlServerDatabaseSettings sqlServerDatabaseSettings;
        public SqlServerDatabaseSettings SqlServerDatabaseSettings
        {
            get { return sqlServerDatabaseSettings; }
            set
            {
                if (sqlServerDatabaseSettings != value)
                {
                    OnPropertyChanging("SqlServerDatabaseSettings");

                    sqlServerDatabaseSettings = value;

                    OnPropertyChanged("SqlServerDatabaseSettings");
                }
            }
        }

        private ServerDatabase serverDatabase;
        public ServerDatabase ServerDatabase
        {
            get { return serverDatabase; }
            set
            {
                if (serverDatabase != value)
                {
                    OnPropertyChanging("ServerDatabase");

                    serverDatabase = value;

                    OnPropertyChanged("ServerDatabase");
                }
            }
        }

       
        private int logRequestTimeFilter;
        public int LogRequestTimeFilter
        {
            get { return logRequestTimeFilter; }
            set
            {
                if (logRequestTimeFilter != value)
                {
                    OnPropertyChanging("LogRequestTimeFilter");

                    logRequestTimeFilter = value;

                    OnPropertyChanged("LogRequestTimeFilter");
                }
            }
        }

        private bool logDebugWrites;
        public bool LogDebugWrites
        {
            get { return logDebugWrites; }
            set
            {
                if (logDebugWrites != value)
                {
                    OnPropertyChanging("LogDebugWrites");

                    logDebugWrites = value;

                    OnPropertyChanged("LogDebugWrites");
                }
            }
        }

        private bool? logResourceStatistics;
        public bool? LogResourceStatistics // DOS FIX
        {
            get { return logDebugWrites; }
            set
            {
                if (logResourceStatistics != value)
                {
                    OnPropertyChanging("LogResourceStatistics");

                    logResourceStatistics = value;

                    OnPropertyChanged("LogResourceStatistics");
                }
            }
        }

        private int? resourceStatisticsIntervall;
        public int? ResourceStatisticsIntervall // DOS FIX
        {
            get { return resourceStatisticsIntervall; }
            set
            {
                if (resourceStatisticsIntervall != value)
                {
                    OnPropertyChanging("ResourceStatisticsIntervall");

                    resourceStatisticsIntervall = value;

                    OnPropertyChanged("ResourceStatisticsIntervall");
                }
            }
        }

        private int? resourceStatisticsTimeIntervall;
        public int? ResourceStatisticsTimeIntervall // DOS FIX
        {
            get { return resourceStatisticsTimeIntervall; }
            set
            {
                if (resourceStatisticsTimeIntervall != value)
                {
                    OnPropertyChanging("ResourceStatisticsTimeIntervall");

                    resourceStatisticsTimeIntervall = value;

                    OnPropertyChanged("ResourceStatisticsTimeIntervall");
                }
            }
        }

        private bool useTruncation;
        public bool UseTruncation
        {
            get { return useTruncation; }
            set
            {
                if (useTruncation != value)
                {
                    OnPropertyChanging("UseTruncation");

                    useTruncation = value;

                    OnPropertyChanged("UseTruncation");
                }
            }
        }
        private string logFileRollOverOnMaxMBSize;
        public string LogFileRollOverOnMaxMBSize
        {
            get { return logFileRollOverOnMaxMBSize; }
            set
            {
                if (logFileRollOverOnMaxMBSize != value)
                {
                    OnPropertyChanging("LogFileRollOverOnMaxMBSize");

                    logFileRollOverOnMaxMBSize = value;

                    OnPropertyChanged("LogFileRollOverOnMaxMBSize");
                }
            }
        }
        private string deleteRollOverOldLogAfterNumberOfFiles;
        public string DeleteRollOverOldLogAfterNumberOfFiles
        {
            get { return deleteRollOverOldLogAfterNumberOfFiles; }
            set
            {
                if (deleteRollOverOldLogAfterNumberOfFiles != value)
                {
                    OnPropertyChanging("DeleteRollOverOldLogAfterNumberOfFiles");

                    deleteRollOverOldLogAfterNumberOfFiles = value;

                    OnPropertyChanged("DeleteRollOverOldLogAfterNumberOfFiles");
                }
            }
        }

        private SmtpClientSettings smtpClientSettings;
        public SmtpClientSettings SmtpClientSettings
        {
            get { return smtpClientSettings; }
            set
            {
                if (smtpClientSettings != value)
                {
                    OnPropertyChanging("SmtpClientSettings");

                    smtpClientSettings = value;

                    OnPropertyChanged("SmtpClientSettings");
                }
            }
        }


        private string clientDownloadUrl;
        public string ClientDownloadUrl
        {
            get { return clientDownloadUrl; }
            set
            {
                if (clientDownloadUrl != value)
                {
                    OnPropertyChanging("ClientDownloadUrl");

                    clientDownloadUrl = value;

                    OnPropertyChanged("ClientDownloadUrl");
                }
            }
        }

        private string remoteArea;
        public string RemoteArea
        {
            get { return remoteArea; }
            set
            {
                if (remoteArea != value)
                {
                    OnPropertyChanging("RemoteArea");

                    remoteArea = value;

                    OnPropertyChanged("RemoteArea");
                }
            }
        }

        private string feedbackMailFrom;
        public string FeedbackMailFrom
        {
            get { return feedbackMailFrom; }
            set
            {
                if (feedbackMailFrom != value)
                {
                    OnPropertyChanging("FeedbackMailFrom");

                    feedbackMailFrom = value;

                    OnPropertyChanged("FeedbackMailFrom");
                }
            }
        }

        private string feedbackMailTo;
        public string FeedbackMailTo
        {
            get { return feedbackMailTo; }
            set
            {
                if (feedbackMailTo != value)
                {
                    OnPropertyChanging("FeedbackMailTo");

                    feedbackMailTo = value;

                    OnPropertyChanged("FeedbackMailTo");
                }
            }
        }

        private string logFilePath;
        public string LogFilePath
        {
            get { return logFilePath; }
            set
            {
                if (logFilePath != value)
                {
                    OnPropertyChanging("LogFilePath");

                    logFilePath = value;

                    OnPropertyChanged("LogFilePath");
                }
            }
        }


        public string SQLScriptsPath { get; set; }

        public string PowershellScriptsPath { get; set; }

        int _repositorySearchTextBoxTypeDelayMilliSeconds = 750;
        public int RepositorySearchTextBoxTypeDelayMilliSeconds
        {
            get { return _repositorySearchTextBoxTypeDelayMilliSeconds; }
            set { _repositorySearchTextBoxTypeDelayMilliSeconds = value; }
        }

        public bool AddHashToImageFilesEnabled { get; set; }

        public ServerConfiguration()
        {
            RaisePropertyChange = true;
            ExpandEnvironmentVariables = true;

            LogFilePath = null; // Rely on default setting for the log file 

            ServerDatabase = new ServerDatabase() { Enabled = false };
            ServerDatabase.PropertyChanged += ServerDatabase_PropertyChanged;


            SmtpClientSettings = new SmtpClientSettings();
            SmtpClientSettings.PropertyChanged += SmtpClientSettings_PropertyChanged;

            SqlServerDatabaseSettings = new SqlServerDatabaseSettings();
            SqlServerDatabaseSettings.PropertyChanged += SqlServerDatabaseSettings_PropertyChanged;

            UseTruncation = false;
            LogFileRollOverOnMaxMBSize = "100";
            DeleteRollOverOldLogAfterNumberOfFiles = "90";
            SQLScriptsPath = @"C:\MultivendorWeb";
            AddHashToImageFilesEnabled = false;
        }

        public void Reset()
        {
        }

        public void RaisePropertyChanged(string propertyName)
        {
            OnPropertyChanged(propertyName);
        }

        void PublisherSettings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged("PublisherSettings");
        }

        void SqlServerDatabaseSettings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged("SqlServerDatabaseSettings");
        }

        void SmtpClientSettings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged("SmtpClientSettings");
        }

        void ServerDatabase_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged("ServerDatabase");
        }

        void StaticDatabase_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged("StaticDatabase");
        }

        void Sites_ListChanged(object sender, ListChangedEventArgs e)
        {
            OnPropertyChanged("Sites");
        }

        #region Change handling
        [field: NonSerialized]
        public event PropertyChangingEventHandler PropertyChanging;
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        [XmlIgnore]
        [DefaultValue(true)]
        public bool ExpandEnvironmentVariables { get; set; }

        [XmlIgnore]
        [DefaultValue(true)]
        public bool RaisePropertyChange { get; set; }

        [XmlIgnore]
        [DefaultValue(false)]
        public bool ChangeTracking { get; set; }

        private Dictionary<string, object> oldValues = new Dictionary<string, object>();
        private Dictionary<string, object> newValues = new Dictionary<string, object>();

        private bool isRestoring = false;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            Type type = this.GetType();
            PropertyInfo property = type.GetProperty(propertyName);

            if (property != null)
            {
                if (isRestoring == false && ChangeTracking == true)
                {
                    object value = property.GetValue(this, null);

                    if (newValues.ContainsKey(propertyName))
                    {
                        newValues.Remove(propertyName);
                    }

                    newValues.Add(propertyName, value);
                }

                if (PropertyChanged != null && RaisePropertyChange == true)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
            }
        }

        protected virtual void OnPropertyChanging(string propertyName)
        {
            Type type = this.GetType();
            PropertyInfo property = type.GetProperty(propertyName);

            if (property != null)
            {
                if (isRestoring == false && ChangeTracking == true)
                {
                    object value = property.GetValue(this, null);

                    if (!oldValues.ContainsKey(propertyName))
                    {
                        oldValues.Add(propertyName, value);
                    }
                }

                if (PropertyChanging != null && RaisePropertyChange == true)
                {
                    PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
                }
            }
        }
        #endregion
    }

    public class SqlServerDatabaseSettings : INotifyPropertyChanged, INotifyPropertyChanging
    {
        public SqlServerDatabaseSettings()
        {
            DetachDatabaseTimeout = 300;
            AttachDatabaseTimeout = 60;
            DropDatabaseTimeout = 300;
            ShrinkDatabaseTimeout = 300;
            PublisherCommandTimeout = 300;
            CommandTimeout = 30;
            MaxParallelPublisherThread = 5;
            ChunkCount = 2000;

            MountAtInstanceName = String.Format(@"{0}\SQLEXPRESS", Environment.MachineName);
            MountAtSqlServerLogin = new SqlServerLogin();
            MountAtSqlServerLogin.PropertyChanged += MountAtSqlServerLogin_PropertyChanged;

            PublishToInstanceName = String.Format(@"{0}\SQLEXPRESS", Environment.MachineName);
            PublishToSqlServerLogin = new SqlServerLogin();
            PublishToSqlServerLogin.PropertyChanged += PublishToSqlServerLogin_PropertyChanged;
        }

        void PublishToSqlServerLogin_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged("PublishToSqlServerLogin");
        }

        void MountAtSqlServerLogin_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged("MountAtSqlServerLogin");
        }

        public static SqlServerDatabaseSettings Create()
        {
            return Instance.Create<SqlServerDatabaseSettings>();
        }

        private SqlServerLogin publishToSqlServerLogin;
        public SqlServerLogin PublishToSqlServerLogin
        {
            get { return publishToSqlServerLogin; }
            set
            {
                if (publishToSqlServerLogin != value)
                {
                    OnPropertyChanging("PublishToSqlServerLogin");

                    publishToSqlServerLogin = value;

                    OnPropertyChanged("PublishToSqlServerLogin");
                }
            }
        }

        private string publishToInstanceName;
        public string PublishToInstanceName
        {
            get { return publishToInstanceName; }
            set
            {
                if (publishToInstanceName != value)
                {
                    OnPropertyChanging("PublishToInstanceName");

                    publishToInstanceName = value;

                    OnPropertyChanged("PublishToInstanceName");
                }
            }
        }

        private SqlServerLogin mountAtSqlServerLogin;
        public SqlServerLogin MountAtSqlServerLogin
        {
            get { return mountAtSqlServerLogin; }
            set
            {
                if (mountAtSqlServerLogin != value)
                {
                    OnPropertyChanging("MountAtSqlServerLogin");

                    mountAtSqlServerLogin = value;

                    OnPropertyChanged("MountAtSqlServerLogin");
                }
            }
        }

        private string mountAtInstanceName;
        public string MountAtInstanceName
        {
            get { return mountAtInstanceName; }
            set
            {
                if (mountAtInstanceName != value)
                {
                    OnPropertyChanging("MountAtInstanceName");

                    mountAtInstanceName = value;

                    OnPropertyChanged("MountAtInstanceName");
                }
            }
        }

        private int detachDatabaseTimeout;
        public int DetachDatabaseTimeout
        {
            get { return detachDatabaseTimeout; }
            set
            {
                if (detachDatabaseTimeout != value)
                {
                    OnPropertyChanging("DetachDatabaseTimeout");

                    detachDatabaseTimeout = value;

                    OnPropertyChanged("DetachDatabaseTimeout");
                }
            }
        }

        private int attachDatabaseTimeout;
        public int AttachDatabaseTimeout
        {
            get { return attachDatabaseTimeout; }
            set
            {
                if (attachDatabaseTimeout != value)
                {
                    OnPropertyChanging("AttachDatabaseTimeout");

                    attachDatabaseTimeout = value;

                    OnPropertyChanged("AttachDatabaseTimeout");
                }
            }
        }

        private int dropDatabaseTimeout;
        public int DropDatabaseTimeout
        {
            get { return dropDatabaseTimeout; }
            set
            {
                if (dropDatabaseTimeout != value)
                {
                    OnPropertyChanging("DropDatabaseTimeout");

                    dropDatabaseTimeout = value;

                    OnPropertyChanged("DropDatabaseTimeout");
                }
            }
        }

        private int shrinkDatabaseTimeout;
        public int ShrinkDatabaseTimeout
        {
            get { return shrinkDatabaseTimeout; }
            set
            {
                if (shrinkDatabaseTimeout != value)
                {
                    OnPropertyChanging("ShrinkDatabaseTimeout");

                    shrinkDatabaseTimeout = value;

                    OnPropertyChanged("ShrinkDatabaseTimeout");
                }
            }
        }

        private int publisherCommandTimeout;
        public int PublisherCommandTimeout
        {
            get { return publisherCommandTimeout; }
            set
            {
                if (publisherCommandTimeout != value)
                {
                    OnPropertyChanging("PublisherCommandTimeout");

                    publisherCommandTimeout = value;

                    OnPropertyChanged("PublisherCommandTimeout");
                }
            }
        }

        private int commandTimeout;
        public int CommandTimeout
        {
            get { return commandTimeout; }
            set
            {
                if (commandTimeout != value)
                {
                    OnPropertyChanging("CommandTimeout");

                    commandTimeout = value;

                    OnPropertyChanged("CommandTimeout");
                }
            }
        }

        private int maxParallelPublisherThread;
        public int MaxParallelPublisherThread
        {
            get { return maxParallelPublisherThread; }
            set
            {
                if (maxParallelPublisherThread != value)
                {
                    OnPropertyChanging("MaxParallelPublisherThread");

                    maxParallelPublisherThread = value;

                    OnPropertyChanged("MaxParallelPublisherThread");
                }
            }
        }

        private int chunkCount;
        public int ChunkCount
        {
            get { return chunkCount; }
            set
            {
                if (chunkCount != value)
                {
                    OnPropertyChanging("ChunkCount");

                    chunkCount = value;

                    OnPropertyChanged("ChunkCount");
                }
            }
        }

        #region Change handling
        public event PropertyChangingEventHandler PropertyChanging;
        public event PropertyChangedEventHandler PropertyChanged;

        [XmlIgnore]
        [DefaultValue(true)]
        public bool ExpandEnvironmentVariables { get; set; }

        [XmlIgnore]
        [DefaultValue(true)]
        public bool RaisePropertyChange { get; set; }

        [XmlIgnore]
        [DefaultValue(false)]
        public bool ChangeTracking { get; set; }

        private Dictionary<string, object> oldValues = new Dictionary<string, object>();
        private Dictionary<string, object> newValues = new Dictionary<string, object>();

        private bool isRestoring = false;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            Type type = this.GetType();
            PropertyInfo property = type.GetProperty(propertyName);

            if (property != null)
            {
                if (isRestoring == false && ChangeTracking == true)
                {
                    object value = property.GetValue(this, null);

                    if (newValues.ContainsKey(propertyName))
                    {
                        newValues.Remove(propertyName);
                    }

                    newValues.Add(propertyName, value);
                }

                if (PropertyChanged != null && RaisePropertyChange == true)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
            }
        }

        protected virtual void OnPropertyChanging(string propertyName)
        {
            Type type = this.GetType();
            PropertyInfo property = type.GetProperty(propertyName);

            if (property != null)
            {
                if (isRestoring == false && ChangeTracking == true)
                {
                    object value = property.GetValue(this, null);

                    if (!oldValues.ContainsKey(propertyName))
                    {
                        oldValues.Add(propertyName, value);
                    }
                }

                if (PropertyChanging != null && RaisePropertyChange == true)
                {
                    PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
                }
            }
        }
        #endregion
    }
    public class SqlServerLogin : INotifyPropertyChanged, INotifyPropertyChanging
    {
        public SqlServerLogin()
        {
            UseSqlServerUser = false;
            SqlServerUser = "Multivendor#User";
            SqlServerPassword = "Login#123!@$";
        }

        public static SqlServerLogin Create()
        {
            return Instance.Create<SqlServerLogin>();
        }

        private bool useSqlServerUser;
        public bool UseSqlServerUser
        {
            get { return useSqlServerUser; }
            set
            {
                if (useSqlServerUser != value)
                {
                    OnPropertyChanging("UseSqlServerUser");

                    useSqlServerUser = value;

                    OnPropertyChanged("UseSqlServerUser");
                }
            }
        }

        private string sqlServerUser;
        public string SqlServerUser
        {
            get { return sqlServerUser; }
            set
            {
                if (sqlServerUser != value)
                {
                    OnPropertyChanging("SqlServerUser");

                    sqlServerUser = value;

                    OnPropertyChanged("SqlServerUser");
                }
            }
        }

        private string sqlServerPassword;
        public string SqlServerPassword
        {
            get { return sqlServerPassword; }
            set
            {
                if (sqlServerPassword != value)
                {
                    OnPropertyChanging("SqlServerPassword");

                    sqlServerPassword = value;

                    OnPropertyChanged("SqlServerPassword");
                }
            }
        }

        public override string ToString()
        {
            return String.Format("UseSqlServerUser {0} SqlServerUser {1}", UseSqlServerUser, SqlServerUser);
        }

        #region Change handling
        public event PropertyChangingEventHandler PropertyChanging;
        public event PropertyChangedEventHandler PropertyChanged;

        [XmlIgnore]
        [DefaultValue(true)]
        public bool ExpandEnvironmentVariables { get; set; }

        [XmlIgnore]
        [DefaultValue(true)]
        public bool RaisePropertyChange { get; set; }

        [XmlIgnore]
        [DefaultValue(false)]
        public bool ChangeTracking { get; set; }

        private Dictionary<string, object> oldValues = new Dictionary<string, object>();
        private Dictionary<string, object> newValues = new Dictionary<string, object>();

        private bool isRestoring = false;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            Type type = this.GetType();
            PropertyInfo property = type.GetProperty(propertyName);

            if (property != null)
            {
                if (isRestoring == false && ChangeTracking == true)
                {
                    object value = property.GetValue(this, null);

                    if (newValues.ContainsKey(propertyName))
                    {
                        newValues.Remove(propertyName);
                    }

                    newValues.Add(propertyName, value);
                }

                if (PropertyChanged != null && RaisePropertyChange == true)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
            }
        }

        protected virtual void OnPropertyChanging(string propertyName)
        {
            Type type = this.GetType();
            PropertyInfo property = type.GetProperty(propertyName);

            if (property != null)
            {
                if (isRestoring == false && ChangeTracking == true)
                {
                    object value = property.GetValue(this, null);

                    if (!oldValues.ContainsKey(propertyName))
                    {
                        oldValues.Add(propertyName, value);
                    }
                }

                if (PropertyChanging != null && RaisePropertyChange == true)
                {
                    PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
                }
            }
        }
        #endregion
    }
    [Serializable]
    public class SmtpClientSettings : INotifyPropertyChanged
    {
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        public SmtpClientSettings()
        {
            host = "localhost";
            port = 25;
            enableSSL = useCredentials = false;
            useDefaultCredentials = true;
        }

        private string senderemail;
        [Bindable(true)]
        public string SenderEmail
        {
            get { return senderemail; }
            set
            {
                if (senderemail != value)
                {

                    senderemail = value;
                    OnPropertyChanged("SenderEmail");
                }
            }
        }

        private string host;
        [Bindable(true)]
        public string Host
        {
            get { return host; }
            set
            {
                if (host != value)
                {

                    host = value;
                    OnPropertyChanged("Host");
                }
            }
        }

        private int port;
        [Bindable(true)]
        public int Port
        {
            get { return port; }
            set
            {
                if (port != value)
                {

                    port = value;

                    OnPropertyChanged("Port");
                }
            }
        }

        private bool enableSSL = false;
        [Bindable(true)]
        public bool EnableSSL
        {
            get { return enableSSL; }
            set
            {
                if (enableSSL != value)
                {

                    enableSSL = value;

                    OnPropertyChanged("EnableSSL");
                }
            }
        }

        private bool useDefaultCredentials = false;
        [Bindable(true)]
        public bool UseDefaultCredentials
        {
            get { return useDefaultCredentials; }
            set
            {
                if (useDefaultCredentials != value)
                {

                    useDefaultCredentials = value;

                    OnPropertyChanged("UseDefaultCredentials");
                }
            }
        }

        private bool useCredentials = false;
        [Bindable(true)]
        public bool UseCredentials
        {
            get { return useCredentials; }
            set
            {
                if (useCredentials != value)
                {


                    useCredentials = value;

                    OnPropertyChanged("UseCredentials");
                }
            }
        }

        private string userName;
        [Bindable(true)]
        public string UserName
        {
            get { return userName; }
            set
            {
                if (userName != value)
                {

                    userName = value;

                    OnPropertyChanged("UserName");
                }
            }
        }

        private string passWord;
        [Bindable(true)]
        public string PassWord
        {
            get { return passWord; }
            set
            {
                if (passWord != value)
                {

                    passWord = value;

                    OnPropertyChanged("PassWord");
                }
            }
        }

        private string domain;
        [Bindable(true)]
        public string Domain
        {
            get { return domain; }
            set
            {
                if (domain != value)
                {

                    domain = value;

                    OnPropertyChanged("Domain");
                }
            }
        }

        public SmtpClient GetSmtpClient()
        {
            try
            {
                if (String.IsNullOrEmpty(Host)) throw new Exception("SMTP host name is undefined");

                SmtpClient client = new SmtpClient(Host);

                if (Port == 0) throw new Exception("SMTP port is zero");
                client.Port = Port;

                client.EnableSsl = EnableSSL;

                if (UseCredentials == true)
                {

                    NetworkCredential credentials = new NetworkCredential();

                    if (String.IsNullOrEmpty(UserName)) throw new Exception("SMTP user name is undefined");
                    if (UserName != null)
                    {
                        credentials.UserName = UserName;
                    }

                    if (PassWord != null)
                    {
                        credentials.Password = PassWord;
                    }

                    if (Domain != null)
                    {
                        credentials.Domain = Domain;
                    }

                    client.Credentials = credentials;
                }
                else if (UseDefaultCredentials == true)
                {
                    client.UseDefaultCredentials = true;
                }

                client.DeliveryMethod = SmtpDeliveryMethod.Network;

                return client;
            }
            catch (Exception exception)
            {
                throw new ApplicationException("Could not create Smtp client.", exception);
                //Log.WriteLine(System.Diagnostics.TraceEventType.Verbose, "Wrong Configuration in in SMTP Client setting:- " + exception.Message);
                //return null;
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
    public interface IServerConfigurationProvider
    {
        ServerConfiguration Configuration { get; }
    }

    public class ServerConfigurationProvider : ConfigurationFileProviderBase<ServerConfigurationProvider, ServerConfiguration>, IServerConfigurationProvider
    {
        public override bool DoWatch { get { return true; } }

        protected override bool CreateConfiguration { get { return true; } }

        protected override bool CreateFile { get { return true; } }

        protected override string DefaultFilePath
        {
            get
            {
                return Path.Combine(HttpRuntime.AppDomainAppPath, "App_Data\\server.config");
            }
        }


        protected override void OnChanged()
        {
            base.OnChanged();

            Configuration.Reset();
        }
    }

}
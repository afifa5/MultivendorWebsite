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
        private SiteDatabase siteDatabase;
        public SiteDatabase SiteDatabase
        {
            get { return siteDatabase; }
            set
            {
                if (siteDatabase != value)
                {
                    OnPropertyChanging("SiteDatabase");

                    siteDatabase = value;

                    OnPropertyChanged("SiteDatabase");
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
        public ServerConfiguration()
        {
            RaisePropertyChange = true;
            ExpandEnvironmentVariables = true;

            AttachDatabaseTimeout = 60;

            ServerDatabase = new ServerDatabase() { Enabled = false };
            ServerDatabase.PropertyChanged += ServerDatabase_PropertyChanged;
            
            SiteDatabase = new SiteDatabase() { Enabled = false };
            SiteDatabase.PropertyChanged += SiteDatabase_PropertyChanged;


            SmtpClientSettings = new SmtpClientSettings();
            SmtpClientSettings.PropertyChanged += SmtpClientSettings_PropertyChanged;

        }

        public void Reset()
        {
        }

        public void RaisePropertyChanged(string propertyName)
        {
            OnPropertyChanged(propertyName);
        }
        void AttachTimeoutSettings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged("AttachDatabaseTimeout");
        }
        void SmtpClientSettings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged("SmtpClientSettings");
        }

        void ServerDatabase_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged("ServerDatabase");
        }
        void SiteDatabase_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged("SiteDatabase");
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
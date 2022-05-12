using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Xml.Serialization;

namespace MultivendorWebViewer.Common
{
    public class DatabaseConfigBase : INotifyPropertyChanged, INotifyPropertyChanging
    {
        private bool enabled;
        public bool Enabled
        {
            get { return enabled; }
            set
            {
                if (enabled != value)
                {
                    OnPropertyChanging("Enabled");

                    enabled = value;

                    OnPropertyChanged("Enabled");
                }
            }
        }

        private string dataBaseName;
        public string DataBaseName
        {
            get { return dataBaseName; }
            set
            {
                if (dataBaseName != value)
                {
                    OnPropertyChanging("DataBaseName");

                    dataBaseName = value;

                    OnPropertyChanged("DataBaseName");
                }
            }
        }

        private string dataBaseServer;
        public string DataBaseServer
        {
            get { return dataBaseServer; }
            set
            {
                if (dataBaseServer != value)
                {
                    OnPropertyChanging("DataBaseServer");

                    dataBaseServer = value;

                    OnPropertyChanged("DataBaseServer");
                }
            }
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

        public DatabaseConfigBase()
        {
            Enabled = false;
            DataBaseName = "DHM#Server";
            DataBaseServer = String.Format(@"{0}\SQLEXPRESS", Environment.MachineName);
            SqlServerUser = "Multivendor#User";
            SqlServerPassword = "Login#123!@$";
        }
        
        [XmlIgnore]
        public string InstanceName
        {
            get
            {
                string[] list = DataBaseServer.Split('\\');
                if (list.Length == 2)
                    return list[1];
                else
                    return String.Empty;
            }
        }

        public override string ToString()
        {
            return String.Format("{0}@{1}", DataBaseName, DataBaseServer);
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

    public class ServerDatabase:DatabaseConfigBase
    {
        public ServerDatabase()
        {
            DataBaseName = "DHM#Server";
        }
    }
    public class SiteDatabase : DatabaseConfigBase
    {
        public SiteDatabase() {
            DataBaseName = "DHM#Web";
        }
    }

}

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Threading;
using System.Diagnostics;
using System.Xml.Serialization;
using HashTag.Text;
using System.Collections.Specialized;
using System.Runtime.Serialization;
using HashTag.Collections;
using Newtonsoft.Json;


namespace HashTag.Diagnostics
{
    /// <summary>
    /// Probe system for machine centric properties
    /// </summary>
        [DataContract(Namespace = CoreConfig.WcfNamespace)]
    public sealed class LogMachineContext 
    {
        
        /// <summary>
        /// Default constructor
        /// </summary>
        public LogMachineContext()
        {
            getSystemProperties();
        }
        [JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
        public string CommandLine { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? Is64BitOperatingSystem { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? Is64BitProcess { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string OsVersion { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? ProcessorCount { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string AppFolder { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string DomainConfigFile { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string DomainAppName { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? DomainId { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string DomainCtxIdentity { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string DomainAppIdentity { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string DomainAssmVersion { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string DomainAssmName { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        private static string _hostName = Environment.MachineName;
        /// <summary>
        /// Computer where this event occured.  Can be different than the computer where
        /// message is actually stored.  If not explicitly set, use runtime information
        /// </summary>
        [DataMember]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string HostName
        {
            get
            {
                if (string.IsNullOrEmpty(_hostName) == true)
                {
                    _hostName = System.Environment.MachineName;
                }
                return _hostName;
            }
            set
            {
                _hostName = value;
            }
        }
      

        /// <summary>
        /// The <see cref="AppDomain"/> in which the program is running
        /// </summary>
        [DataMember]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string AppDomainName { get; set; }
        
   
        /// <summary>
        /// The Win32 process ID for the current running process.
        /// </summary>
        [DataMember]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ProcessId { get; set; }
      

        /// <summary>
        /// The name of the current running process.
        /// </summary>
        [DataMember]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ProcessName { get; set; }

        /// <summary>
        /// The name of the .NET thread.
        /// </summary>
        ///  <seealso cref="Win32ThreadId"/>
        [DataMember]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ManagedThreadName { get; set; }
        
        /// <summary>
        /// The Win32 Thread ID for the current thread.
        /// </summary>
        [DataMember]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Win32ThreadId { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string StackTrace { get; set; }


        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public long? WorkingMemoryBytes { get; set; }

        #region Win32 calls

        string _ipAddressList;
        /// <summary>
        /// Gets/Set comma separated list of IP addresses for this host
        /// </summary>
        [Citation("20080901",Source="http://www.geekpedia.com/tutorial149_Get-the-IP-address-in-a-Windows-application.html",SourceDate="October 27th 2005")]
        [DataMember]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string IPAddressList
        {
            get
            {
                if (string.IsNullOrEmpty(_ipAddressList))
                {
                    string myHost = System.Net.Dns.GetHostName();

                    System.Net.IPHostEntry myIPs = System.Net.Dns.GetHostEntry(myHost);

                    // Loop through all IP addresses and display each 
                    _ipAddressList = "";
                    foreach (System.Net.IPAddress myIP in myIPs.AddressList)
                    {
                        if (_ipAddressList.Length > 0)
                            _ipAddressList += ",";
                        _ipAddressList += myIP.ToString();
                    }
                }
                return _ipAddressList;
            }
            set
            {
                _ipAddressList = value;
            }
        }

        private void getSystemProperties()
        {
            this.CommandLine = Environment.CommandLine;
            this.Is64BitOperatingSystem = Environment.Is64BitOperatingSystem;
            this.Is64BitProcess = Environment.Is64BitProcess;
            this.OsVersion = Environment.OSVersion.VersionString;
            this.ProcessorCount = Environment.ProcessorCount;
            this.StackTrace = Environment.StackTrace;
            this.WorkingMemoryBytes = Environment.WorkingSet;

            if (AppDomain.CurrentDomain != null)
            {
                this.AppDomainName = AppDomain.CurrentDomain.FriendlyName;
                this.DomainId = AppDomain.CurrentDomain.Id;
                if (AppDomain.CurrentDomain.ActivationContext != null)
                    this.DomainCtxIdentity = AppDomain.CurrentDomain.ActivationContext.Identity.FullName;

                if (AppDomain.CurrentDomain.ApplicationIdentity != null)
                    this.DomainAppIdentity = AppDomain.CurrentDomain.ApplicationIdentity.FullName;

                if (AppDomain.CurrentDomain.DomainManager != null && AppDomain.CurrentDomain.DomainManager.EntryAssembly != null)
                {
                    this.DomainAssmVersion = AppDomain.CurrentDomain.DomainManager.EntryAssembly.GetName().Version.ToString();
                    this.DomainAssmName = AppDomain.CurrentDomain.DomainManager.EntryAssembly.FullName;
                }
                if (AppDomain.CurrentDomain.SetupInformation != null)
                {
                    this.DomainAppName = AppDomain.CurrentDomain.SetupInformation.ApplicationName;
                    this.DomainConfigFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
                    this.AppFolder = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
                }
            }

            this.HostName = Environment.MachineName;
             
            this.ManagedThreadName = Thread.CurrentThread.Name;
            this.ProcessId = Utils.Win32.GetCurrentProcessId().ToString(System.Globalization.NumberFormatInfo.InvariantInfo);
            var currentProc = System.Diagnostics.Process.GetCurrentProcess();
            if (currentProc != null && currentProc.MainModule != null)
            {
                this.ProcessName = currentProc.MainModule.FileName;
            }
            this.Win32ThreadId = Utils.Win32.GetCurrentThreadId().ToString(System.Globalization.NumberFormatInfo.InvariantInfo);
            _className = ClassName;
            _methodName = MethodName;
            _ipAddressList = IPAddressList; //force load of lazy loaded properties
        }

        #endregion


        private string _className;
        /// <summary>
        /// Name of first non-Utilities class within this stack trace.  NOTE:  Will return "" if called from any method within the same namespace as this method. HashTag.Diagnostics
        /// </summary>
        [DataMember]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ClassName
        {
            get
            {
                if ((string.IsNullOrEmpty(_className) == true) || string.IsNullOrEmpty(_methodName) == true)
                    getStackTraceInfo();
                return _className;
            }
            set { _className = value; }
        }
     
        private void getStackTraceInfo()
        {
            StackTrace st = new StackTrace();
            StackFrame[] frames = st.GetFrames();
            string namespaceName = frames[0].GetMethod().ReflectedType.Namespace;
            foreach (StackFrame sf in frames)
            {
                string operation = sf.GetMethod().Name;
                Type t = sf.GetMethod().ReflectedType;
                if (t!=null && t.Namespace != namespaceName)
                {
                    _className = t.FullName;
                    _methodName = operation;
                    break;
                }
            }
        }

        private string _methodName;

        /// <summary>
        /// Name of non-Utilities method that is calling this method. NOTE:  Will return "" if called from any method within the same namespace as this method. 
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string MethodName
        {
            get
            {
                if ((string.IsNullOrEmpty(_className) == true) || string.IsNullOrEmpty(_methodName) == true)
                    getStackTraceInfo();
                return _methodName;
            }
            set { _methodName = value; }
        }

        /// <summary>
        /// Convert all RunTime fields into XML elements
        /// </summary>
        /// <param name="writer">Writer to write XML elements to</param>
        public void ToXml(XmlWriter writer)
        {
            try
            {
                writer.WriteStartElement("MachineContext");
                writer.WriteElementString("HostName", this.HostName);                
                writer.WriteElementString("AppDomainName", this.AppDomainName);
                writer.WriteElementString("ProcessId", this.ProcessId);
                writer.WriteElementString("ProcessName", this.ProcessName);
                writer.WriteElementString("ManagedThreadName", this.ManagedThreadName);
                writer.WriteElementString("Win32ThreadId", this.Win32ThreadId);
                writer.WriteElementString("ClassName", this.ClassName);
                writer.WriteElementString("MethodName", this.MethodName);
                writer.WriteElementString("StackTrace", StackTrace);
            }
            finally
            {
                writer.WriteEndElement(); //runtimeContext
            }
        }

        /// <summary>
        /// Returns a list of property/values of the machine taken at the time this class was created.  Used most frequently in logging scenarios
        /// </summary>
        /// <returns></returns>
        public PropertyBag ToList()
        {
            var retList = new PropertyBag();
            retList.Add("HostName", this.HostName);            
            retList.Add("AppDomainName", this.AppDomainName);
            retList.Add("ProcessId", this.ProcessId);
            retList.Add("ProcessName", this.ProcessName);
            retList.Add("ManagedThreadName", this.ManagedThreadName);
            retList.Add("Win32ThreadId", this.Win32ThreadId);
            retList.Add("ClassName", this.ClassName);
            retList.Add("MethodName", this.MethodName);
            retList.Add("StackTrace", StackTrace);
            return retList;
        }

        /// <summary>
        /// Generate an XML representation of this class. &gt;MachineContext&lt; is the first node returned.
        /// </summary>
        /// <returns>Xml representaion of this object</returns>
        public string ToXml()
        {
            return Serialize.To.Xml(this);
            
        }


     
    } //machine context
}

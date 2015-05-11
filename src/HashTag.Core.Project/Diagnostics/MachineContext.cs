
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


namespace HashTag.Diagnostics
{
    /// <summary>
    /// Probe system for machine centric properties
    /// </summary>
        [DataContract(Namespace = CoreConfig.WcfNamespace)]
    public sealed class MachineContext 
    {
        
        /// <summary>
        /// Default constructor
        /// </summary>
        public MachineContext()
        {
            getSystemProperties();
        }

        private static string _hostName = Environment.MachineName;
        /// <summary>
        /// Computer where this event occured.  Can be different than the computer where
        /// message is actually stored.  If not explicitly set, use runtime information
        /// </summary>
        [DataMember]
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
        private string _identity = Environment.UserDomainName + "/" + Environment.UserName;
        /// <summary>
        /// Identity (usually username) process is running under when application created this
        /// message.
        /// </summary>
        [DataMember]
        public string Identity
        {
            get
            {
                return _identity;
            }
            set
            {
                _identity = value;
            }
        }
        private string _appDomainName;

        /// <summary>
        /// The <see cref="AppDomain"/> in which the program is running
        /// </summary>
        [DataMember]
        public string AppDomainName
        {
            get
            {
                return _appDomainName;
            }
            set
            {
                _appDomainName = value;
            }
        }

        
        private string _userDomainName = Environment.UserDomainName;
        /// <summary>
        /// Identity domain caller of this application is running under
        /// </summary>
        [DataMember]
        public string UserDomainName
        {
            get { return _userDomainName; }
            set { _userDomainName = value; }
        }
    

        private string _processId;
        /// <summary>
        /// The Win32 process ID for the current running process.
        /// </summary>
        [DataMember]
        public string ProcessId
        {
            get
            {
                return _processId;
            }
            set
            {
                _processId = value;
            }
        }

        private string _processName;
        /// <summary>
        /// The name of the current running process.
        /// </summary>
        [DataMember]
        public string ProcessName
        {
            get
            {
                return _processName;
            }
            set
            {
                _processName = value;
            }
        }

        private string _managedThreadName;
        /// <summary>
        /// The name of the .NET thread.
        /// </summary>
        ///  <seealso cref="Win32ThreadId"/>
        [DataMember]
        public string ManagedThreadName
        {
            get
            {
                return _managedThreadName;
            }
            set
            {
                _managedThreadName = value;
            }
        }

        private string win32ThreadId;
        /// <summary>
        /// The Win32 Thread ID for the current thread.
        /// </summary>
        [DataMember]
        public string Win32ThreadId
        {
            get
            {
                return win32ThreadId;
            }
            set
            {
                win32ThreadId = value;
            }
        }
        #region Win32 calls

        string _ipAddressList;
        /// <summary>
        /// Gets/Set comma separated list of IP addresses for this host
        /// </summary>
        [Citation("20080901",Source="http://www.geekpedia.com/tutorial149_Get-the-IP-address-in-a-Windows-application.html",SourceDate="October 27th 2005")]
        [DataMember]
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
            this.HostName = Environment.MachineName;
            this.AppDomainName = AppDomain.CurrentDomain.FriendlyName;
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
        private string _stackTrace = Environment.StackTrace;

        /// <summary>
        /// Return a string that represents the stack trace at the moment of this call
        /// </summary>
         [DataMember]
        public string StackTrace
        {
            get
            {
                return _stackTrace;
            }
            set
            {
                _stackTrace = value;
            }
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
                if (t.Namespace != namespaceName)
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
                writer.WriteElementString("Identity", this.Identity);
                writer.WriteElementString("AppDomainName", this.AppDomainName);
                writer.WriteElementString("ProcessId", this.ProcessId);
                writer.WriteElementString("ProcessName", this.ProcessName);
                writer.WriteElementString("ManagedThreadName", this.ManagedThreadName);
                writer.WriteElementString("Win32ThreadId", this.win32ThreadId);
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
            retList.Add("Identity", this.Identity);
            retList.Add("AppDomainName", this.AppDomainName);
            retList.Add("ProcessId", this.ProcessId);
            retList.Add("ProcessName", this.ProcessName);
            retList.Add("ManagedThreadName", this.ManagedThreadName);
            retList.Add("Win32ThreadId", this.win32ThreadId);
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


        #region IXmlSerializable Members

        /// <summary>
        /// IXmlSerializable Interface Implementation
        /// </summary>
        /// <returns>Null</returns>
        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        /// <summary>
        /// Not Implemented.  Class is intended for for serialization only.
        /// </summary>
        /// <param name="reader"></param>
        public void ReadXml(XmlReader reader)
        {
            throw new NotImplementedException("This class is intended to be serialized only.  Deserialization is not supported");
        }

        /// <summary>
        /// Generate an XML representation of this class. &gt;MachineContext&lt; is the first node returned.
        /// </summary>
        /// <param name="writer">Writer to which Xml of this object is sent</param>
        public void WriteXml(XmlWriter writer)
        {
            ToXml(writer);
        }

        #endregion

        
    } //machine context
}

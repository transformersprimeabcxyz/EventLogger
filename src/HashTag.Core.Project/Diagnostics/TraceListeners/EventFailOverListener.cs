using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;

namespace HashTag.Diagnostics.TraceListeners
{
    public class EventBlockFileListener : TraceListener
    {
        private static object _lockObject = new object();
        public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, object data)
        {
            try
            {
                lock (_lockObject)
                {
                    string fileName = string.Format("{0}.{1}.Log", DateTime.Now.ToString("yyyy-MM-dd-hh-mm-"), Guid.NewGuid().ToString());
                    string fullFileName = Path.Combine(LogfilePath, fileName);
                    if (File.Exists(fullFileName) == false)
                    {
                        File.WriteAllText(fullFileName, "[]");
                    }
                }
            }
            catch (Exception)
            {               
                // intentionally ignore exceptions
            }
            

        }

        public override void Write(string message)
        {
            throw new NotImplementedException();
        }

        public override void WriteLine(string message)
        {
            throw new NotImplementedException();
        }

        protected override string[] GetSupportedAttributes()
        {
            return new string[] {
                "logFolder"
            };
        }

        static string _logFilePath;
        public string LogfilePath
        {
            get
            {
                if (!string.IsNullOrEmpty(_logFilePath)) return _logFilePath;

                var pathsToCheck = new List<string>()
                {
                    base.Attributes["logFolder"],
                    HttpContext.Current != null ? HttpContext.Current.Server.MapPath("~/App_Data") : (string) null,
                    Path.GetDirectoryName((new Uri(Assembly.GetExecutingAssembly().GetName().CodeBase)).PathAndQuery),
                    Path.GetTempPath(), 
                   
                    HostingEnvironment.ApplicationPhysicalPath,
                    AppDomain.CurrentDomain.SetupInformation.ApplicationBase,
                    Path.GetDirectoryName(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile),
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),                    
                    
                    Environment.CurrentDirectory,
                };
                
                return pathsToCheck.Any(path => verifyAccess(path)) ? _logFilePath : _logFilePath;
            }
        }

        /// <summary>
        /// Verifies log folder exists and the user has CRUD priveliges.  Sets the _logFilePath field
        /// </summary>
        /// <param name="rootFolder"></param>
        /// <param name="subFolderPath"></param>
        /// <returns>True if user has r/w permissions on folder</returns>
        private bool verifyAccess(string rootFolder, string subFolderPath="Logs")
        {
            if (string.IsNullOrWhiteSpace(rootFolder)) return false;
            try
            {
                var logFolder = Path.Combine(rootFolder, subFolderPath);
                Directory.CreateDirectory(logFolder); // CreateDirectory creates any missing folders in path
                var tmpFileName = Path.GetTempFileName();
                tmpFileName = Path.Combine(logFolder, tmpFileName);
                File.WriteAllText(tmpFileName,DateTime.Now.ToString());
                File.ReadAllLines(tmpFileName);
                File.Delete(tmpFileName);

                _logFilePath = logFolder;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

    }
}

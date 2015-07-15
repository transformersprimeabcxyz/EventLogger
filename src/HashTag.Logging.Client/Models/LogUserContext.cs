using Newtonsoft.Json;
using System;
using System.Threading;
using System.Web;

namespace HashTag.Diagnostics
{
    /// <summary>
    /// Collects various identities from runtime (e.g. HttpContext.User, ThreadPrincipal, etc)
    /// </summary>
    public class LogUserContext
    {
        public LogUserContext()
        {           
            if (HttpContext.Current != null 
                && HttpContext.Current.User != null 
                && HttpContext.Current.User.Identity != null 
                && !string.IsNullOrEmpty(HttpContext.Current.User.Identity.Name))
            {
                HttpUser = HttpContext.Current.User.Identity.Name;
            }

            if (Thread.CurrentPrincipal != null && Thread.CurrentPrincipal.Identity != null && !string.IsNullOrWhiteSpace(Thread.CurrentPrincipal.Identity.Name))
            {
                ThreadPrincipal = Thread.CurrentPrincipal.Identity.Name;
            }
            if (AppDomain.CurrentDomain != null && AppDomain.CurrentDomain.ActivationContext != null && AppDomain.CurrentDomain.ActivationContext.Identity != null)
            {
                AppDomainIdentity = AppDomain.CurrentDomain.ActivationContext.Identity.FullName;
            }
            EnvUserName = Environment.UserName;
            UserDomain = Environment.UserDomainName;
            IsInteractive = Environment.UserInteractive;
        }

        /// <summary>
        /// Identity on current thread at time of collection. (Thread.CurrentPrincipal.Identity.Name)
        /// </summary>
        [JsonProperty(PropertyName="threadPrincipal", NullValueHandling=NullValueHandling.Ignore)]
        public string ThreadPrincipal { get; set; }

        /// <summary>
        /// Identity of HttpUser at time of collection. (HttpContext.Current.User.Identiy.Name)
        /// </summary>
        [JsonProperty(PropertyName = "httpUser", NullValueHandling = NullValueHandling.Ignore)]
        public string HttpUser { get; set; }

        /// <summary>
        /// Identity of .Net framework Environment.UserName at time of collection. (HttpContext.Current.User.Identiy.Name)
        /// </summary>
        [JsonProperty(PropertyName = "envUserName", NullValueHandling = NullValueHandling.Ignore)]
        public string EnvUserName { get; set; }

        /// <summary>
        /// Environment.UserDomainName
        /// </summary>
        [JsonProperty(PropertyName = "userDomain", NullValueHandling = NullValueHandling.Ignore)]
        public string UserDomain { get; set; }

        /// <summary>
        /// AppDomain.CurrentDomain.ActivationContext.Identity.FullName;
        /// </summary>
        [JsonProperty(PropertyName = "appDomain", NullValueHandling = NullValueHandling.Ignore)]
        public string AppDomainIdentity { get; set; }

        /// <summary>
        /// Environment.UserInteractive
        /// </summary>
        [JsonProperty(PropertyName = "isInteractive", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsInteractive { get; set; }

        private string _defaultUser;

        /// <summary>
        /// Returns first non-null value of HttpUser, ThreadPrincipal, EnvUserName, AppDomainIdentity in that order.
        /// This value is used as the default value for LogEvent.User
        /// </summary>
        public string DefaultUser
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_defaultUser))
                {
                    if (!string.IsNullOrWhiteSpace(HttpUser)) return this.HttpUser;
                    if (!string.IsNullOrWhiteSpace(ThreadPrincipal)) return this.ThreadPrincipal;
                    if (!string.IsNullOrWhiteSpace(EnvUserName)) return this.EnvUserName;
                    if (!string.IsNullOrWhiteSpace(AppDomainIdentity)) return this.AppDomainIdentity;
                }
                return _defaultUser;
            }
            set
            {
                _defaultUser = value;
            }
        }

        /// <summary>
        /// Assign current default user so it won't search when querying DefaultUser
        /// </summary>
        public void Fix()
        {
            DefaultUser = DefaultUser;
        }
    }
}

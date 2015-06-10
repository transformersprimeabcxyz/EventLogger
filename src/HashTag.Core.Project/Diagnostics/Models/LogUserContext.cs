using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;

namespace HashTag.Diagnostics
{
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

        public string ThreadPrincipal { get; set; }
        public string HttpUser { get; set; }
        public string EnvUserName { get; set; }
        public string UserDomain { get; set; }
        public string AppDomainIdentity { get; set; }

        public bool? IsInteractive { get; set; }
        private string _defaultUser;
        public string DefaultUser
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_defaultUser))
                {
                    return HttpUser ?? ThreadPrincipal ?? EnvUserName ?? AppDomainIdentity;
                }
                return _defaultUser;
            }
            set
            {
                _defaultUser = value;
            }
        }
        public void Fix()
        {
            DefaultUser = DefaultUser;
        }
    }
}

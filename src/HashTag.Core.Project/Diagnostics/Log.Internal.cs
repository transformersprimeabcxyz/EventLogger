using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace HashTag.Diagnostics
{
    public partial class Log
    {
        private static InternalLogWriter _internal;
        public static InternalLogWriter Internal
        {
            get
            {
                if (_internal == null)
                {
                    _internal = new InternalLogWriter();
                }
                return _internal;
            }
            set
            {
                _internal = value;
            }
        }
    }
    public class InternalLogWriter
    {
        public void Write(string message, params object[] args)
        {
            this.Write(TraceEventType.Information, message, args);
        }
        public void Write(Exception ex, string message=null, params object[] args)
        {
            
        }
        public void Write(TraceEventType severity,string message, params object[] args)
        {

        }
    }
}

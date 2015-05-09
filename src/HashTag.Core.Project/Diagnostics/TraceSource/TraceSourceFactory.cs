using HashTag.Reflection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;

namespace HashTag.Diagnostics
{

    public class TraceSourceFactory
    {
        static TraceConfig _config = new TraceConfig();
        volatile static bool _isConfigured = false;

        internal static TraceConfig Config
        {
            get
            {
                return _config;
            }
        }
        public static bool Configure(TraceConfig config = null)
        {

            if (config == null)
            {
                _config = new TraceConfig();

            }
            _isConfigured= TraceSourceCollection.Configure(_config);
            return _isConfigured;
        }


        private static TraceSourceCollection _sources = new TraceSourceCollection();
        public static TraceSourceCollection Sources
        {

            get
            {
                if (!_isConfigured)
                {
                    Configure(_config);
                }
                return _sources;
            }
        }

        ///// <summary>
        ///// Last chance log source
        ///// </summary>
        //public static TraceSource LastChance
        //{
        //    get
        //    {
        //        return Sources.LastChanceSource;
        //    }
        //}

        /// <summary>
        /// Source to use if no sources cofigured or couldn't be located as a configured source.
        /// This is the zero-config source. Often the 'Application' level source.  
        /// NOTE:  Do not normally call this as
        /// it will be returned if source is not in internal collection.
        /// </summary>
        public static TraceSource Application
        {
            get
            {
                return Sources.ApplicationSource;
            }
        }


    }
}

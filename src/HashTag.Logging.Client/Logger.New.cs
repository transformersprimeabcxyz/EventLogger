/**
/// HashTag.Core Library
/// Copyright © 2010-2014
///
/// This module is Copyright © 2010-2014 Steve Powell
/// All rights reserved.
///
/// This library is free software; you can redistribute it and/or
/// modify it under the terms of the Microsoft Public License (Ms-PL)
/// 
/// This library is distributed in the hope that it will be
/// useful, but WITHOUT ANY WARRANTY; without even the implied
/// warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR
/// PURPOSE.  See theMicrosoft Public License (Ms-PL) License for more
/// details.
///
/// You should have received a copy of the Microsoft Public License (Ms-PL)
/// License along with this library; if not you may 
/// find it here: http://www.opensource.org/licenses/ms-pl.html
///
/// Steve Powell, hashtagdonet@gmail.com
**/
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace HashTag.Diagnostics
{
    /// <summary>
    /// 
    /// </summary>
    public partial class Logger
    {

        
        private volatile static bool _isInitialized = false;
        internal SourceLevels _logLevels { get; set; }

        /// <summary>
        /// Initialize global (i.e. static) fields for logging subsystem
        /// </summary>
        private static void initialize()
        {
           

            _isInitialized = true;
        }

      



        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="logName"></param>
        internal Logger(string logName):this()
        {
            _logName = logName;
        }

        
        internal Logger()
        {
            if (!_isInitialized)
            {
                initialize();
            }


            _logLevels = CoreConfig.Log.ApplicationLogLevels;
        }

        internal Logger(string sourceName, SourceLevels levels)
            : this()
        {
            LogName = sourceName;
            _logLevels = levels;
        }

    }
}
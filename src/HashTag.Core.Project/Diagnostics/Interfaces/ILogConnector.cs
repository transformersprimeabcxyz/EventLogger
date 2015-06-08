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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HashTag.Diagnostics
{
    /// <summary>
    /// Used to define custom log sinks (e.g TraceSource, EntLib, log4Net, etc).  Called on
    /// separate thread from logging subsystem
    /// </summary>
    public interface ILogConnector:IDisposable 
    {
        /// <summary>
        /// Do any class initialization (e.g. open file, database connections, verify sink existance)
        /// </summary>
        void Initialize();

        /// <summary>
        /// Hands off a block of messages to be persisted by the dispatcher implementation
        /// </summary>
        /// <param name="messageBlock">A block of messages that needs persisted to permanent storage</param>
        /// <returns>True if persister did not throw unhandled error</returns>
        bool PersistMessages(List<LogMessage> messageBlock);

        /// <summary>
        /// Tell persister to force writing any log messages it might be storing in its private internal buffers
        /// </summary>
        /// <returns>True if records were flushed and no persister exceptions occured</returns>
        bool Flush();

        /// <summary>
        /// Close any open resources (e.g. file handles, db connections, web service proxies)
        /// </summary>
        /// <returns>True if persister closed and no exceptions occured</returns>
        bool Close();
    }
}
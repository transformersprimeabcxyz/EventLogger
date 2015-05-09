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

namespace HashTag.Diagnostics
{
    
    public sealed partial class Log
    {
        private static InternalLogger _internalLog;
        /// <summary>
        /// Logger to output library level messages
        /// </summary>
        public static InternalLogger Internal
        {
            get
            {
                if (_internalLog == null)
                {
                    _internalLog = InternalLogger.configureInternalLog();
                }
                return _internalLog;
            }
        }

        private static InternalLogger _lastChanceLog;

        public static InternalLogger Local
        {
            get
            {
                if (_lastChanceLog == null)
                {
                    _lastChanceLog = InternalLogger.configureLocalLog();
                }
                return _lastChanceLog;
            }
        }
    }
}
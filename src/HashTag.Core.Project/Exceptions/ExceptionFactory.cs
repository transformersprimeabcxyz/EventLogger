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
using HashTag.Text;
using System.ServiceModel;
using HashTag.ServiceModel;



namespace HashTag
{
    public class ExceptionFactory
    {
        /// <summary>
        /// Helper method to allow users to create exceptions with parameter subsitution in the message text
        /// </summary>
        /// <typeparam name="T">Any type derived from Exception</typeparam>
        /// <param name="message">Text for message including any format specifiers.  (see String.Format)</param>
        /// <param name="args">Arguments to be placed into message</param>
        /// <returns>Newly created exception containing message</returns>        
        public static T New<T>(string message, params object[] args) where T : Exception
        {
            Type exceptionType = typeof(T);
            object[] exceptionArgs = { TextUtils.StringFormat(message, args) };
            return (T)Activator.CreateInstance(exceptionType, exceptionArgs);
        }

        /// <summary>
        /// Helper method to allow users to create exceptions with parameter subsitution in the message text
        /// </summary>
        /// <typeparam name="T">Any type derived from Exception</typeparam>
        /// <param name="message">Text for message including any format specifiers.  (see String.Format)</param>
        /// <param name="args">Arguments to be placed into message</param>
        /// <param name="innerException">Exception that will be assigned to the inner exception property of this exception</param>
        /// <returns>Newly created exception containing message</returns>            
        /// <remarks>Note:  Inner exception is listed first due to the optional parameter arguments.  This is 
        /// different than normal .Net exception constructor pattern which has the inner exception following the message</remarks>
        public static T New<T>(Exception innerException, string message, params object[] args) where T : Exception
        {
            Type exceptionType = typeof(T);
            object[] exceptionArgs = { TextUtils.StringFormat(message, args), innerException };
            return (T)Activator.CreateInstance(exceptionType, exceptionArgs);
        }

        /// <summary>
        /// Convenience method for createing a WCF compatible fault
        /// </summary>
        /// <param name="ex">Native .Net exception that will be wrapped in a WcfFault</param>
        /// <returns>Newly created WcfFault</returns>
        public static FaultException<WcfFault> NewFault(Exception ex)
        {
            return WcfFault.Create(ex);            
        }

        /// <summary>
        /// Helper method to allow users to create exceptions with parameter subsitution in the message text
        /// </summary>
        /// <typeparam name="T">Any type derived from Exception</typeparam>
        /// <param name="message">Text for message including any format specifiers.  (see String.Format)</param>
        /// <param name="args">Arguments to be placed into message</param>
        /// <returns>Newly created exception containing message</returns>        
        public static void Throw<T>(string message, params object[] args) where T : Exception
        {
            Type exceptionType = typeof(T);
            object[] exceptionArgs = { TextUtils.StringFormat(message, args) };
            throw (T)Activator.CreateInstance(exceptionType, exceptionArgs);
        }

        /// <summary>
        /// Helper method to allow users to create exceptions with parameter subsitution in the message text
        /// </summary>
        /// <typeparam name="T">Any type derived from Exception</typeparam>
        /// <param name="message">Text for message including any format specifiers.  (see String.Format)</param>
        /// <param name="args">Arguments to be placed into message</param>
        /// <param name="innerException">Exception that will be assigned to the inner exception property of this exception</param>
        /// <returns>Newly created exception containing message</returns>            
        /// <remarks>Note:  Inner exception is listed first due to the optional parameter arguments.  This is 
        /// different than normal .Net exception constructor pattern which has the inner exception following the message</remarks>
        public static void Throw<T>(Exception innerException, string message, params object[] args) where T : Exception
        {
            Type exceptionType = typeof(T);
            object[] exceptionArgs = { TextUtils.StringFormat(message, args), innerException };
            throw (T)Activator.CreateInstance(exceptionType, exceptionArgs);
        }

       
    }
}
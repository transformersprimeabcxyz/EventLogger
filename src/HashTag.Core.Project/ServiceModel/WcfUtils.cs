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
using System.ServiceModel;

namespace HashTag.ServiceModel
{
    public static class WcfUtils
    {
        public static FaultException<WcfFault> NewFault(this Exception ex)
        {
            return new FaultException<WcfFault>(new WcfFault(ex), new FaultReason(new FaultReasonText(ex.Message)));
        }

        /// <summary>
        /// Extract internal exception, if any from FaultException.
        /// </summary>
        /// <param name="ex">Hydradted FaultException&lt;&gt;</param>
        /// <returns>Internal exception, if found, or parameter <paramref name="ex"/> if not</returns>
        public static Exception Extract(Exception ex)
        {
            if (ex is FaultException<WcfFault>)
            {
                return (ex as FaultException<WcfFault>).Detail.NativeException;
            }
            if (ex is FaultException<Exception>)
            {
                return ((ex as FaultException<Exception>).Detail);
            }
            return ex;
        }
    }
}
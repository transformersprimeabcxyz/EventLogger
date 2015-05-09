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
using System.Reflection;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace HashTag.Data
{
    /// <summary>
    /// Remove an entity from it's data context
    /// </summary>
    [DataContract]
    public abstract class LinqBase : IDetachable
    {
        /// <summary>
        /// Force a call to the Linq Entity's 'Initialize' method which effectively breaks the binding to data context
        /// </summary>
        [Citation("http://www.jstawski.com/archive/2008/07/09/linq-to-sql-generic-detach.aspx", Author = "Jonas Stawski")]
        public virtual void Detach()
        {
            if (IsAttached == true)
            {
                GetType().GetMethod("Initialize", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(this, null);
             
            }
        }
        /// <summary>
        /// Returns TRUE if Linq entity is bound to a data context
        /// </summary>
        public virtual bool IsAttached
        {
            get
            {
               //return (PropertyChanged.GetInvocationList().Length > 0);
                return false;
            }

        }


        //public abstract event PropertyChangingEventHandler PropertyChanging;

        //public abstract event PropertyChangedEventHandler PropertyChanged;
    }
}
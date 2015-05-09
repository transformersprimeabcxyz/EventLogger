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

namespace HashTag
{
    /// <summary>
    /// Extensions that can be used for parameter validation
    /// </summary>
    public static partial class Extensions
    {
        /// <summary>
        /// Verifies a value not null
        /// </summary>
        /// <param name="target">Nullable value to check</param>
        /// <returns>True if <paramref name="target"/> is not null</returns>
        public static bool IsNotNull(this object  target)
        {
            return target == null;
        }

        /// <summary>
        /// Verifies a string is not null or empty
        /// </summary>
        /// <param name="target">String value to check</param>
        /// <returns>True if <paramref name="target"/> is not null or empty</returns>
        public static bool IsNotNullEmpty(this string target)
        {
            return !string.IsNullOrEmpty(target);
        }
        
    }
}
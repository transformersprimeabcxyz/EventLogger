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
using System.Runtime.InteropServices;
using System.Text;

namespace HashTag.Interop
{
    public class Win32Utils
    {
        /// <summary>
        /// Compares two binary values.  Uses Win32 API.  NOTE:  Some anectodatal evidence suggests this call can be up to 7X
        /// faster than LINQ SequenceCompare
        /// </summary>
        /// <param name="b1"></param>
        /// <param name="b2"></param>
        /// <returns>True if both array contain the same content</returns>
        [Citation("20130504", Source="http://stackoverflow.com/questions/43289/comparing-two-byte-arrays-in-net")]
        public static bool ByteArrayCompare(byte[] b1, byte[] b2)
        {            
            // Validate buffers are the same length.
            // This also ensures that the count does not exceed the length of either buffer.  
            return b1 != null && b2 != null && b1.Length == b2.Length && Utils.Win32.memcmp(b1, b2, b1.Length) == 0;
        }
    }
}
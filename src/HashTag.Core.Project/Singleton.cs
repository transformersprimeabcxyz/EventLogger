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

namespace HashTag
{
    /// <summary>
    /// Create a singleton instance of any newable class
    /// </summary>
    /// <typeparam name="T">Class type that is being created as a singleton</typeparam>
    [Citation("http://www.codeproject.com/KB/architecture/GenericSingletonPattern.aspx#")]
    public static class Singleton<T>
           where T : class
    {
        static volatile T _instance;
        static object _lock = new object();

        static Singleton()
        {
        }

        /// <summary>
        /// Create/get a single instance of a class
        /// </summary>
        public static T Instance
        {
            get
            {
                if (_instance == null)
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            ConstructorInfo constructor = null;

                            try
                            {
                                // Binding flags exclude public constructors.
                                constructor = typeof(T).GetConstructor(BindingFlags.Instance |
                                              BindingFlags.NonPublic, null, new Type[0], null);
                            }
                            catch (Exception exception)
                            {
                                throw new InvalidOperationException(exception.Message,exception);
                            }

                            if (constructor == null || constructor.IsAssembly)
                                // Also exclude internal constructors.
                                throw new InvalidOperationException(string.Format("A private or " +
                                      "protected constructor is missing for '{0}'.", typeof(T).Name));

                            _instance = (T)constructor.Invoke(null);
                        }
                    }

                return _instance;
            }
        }
    }
}
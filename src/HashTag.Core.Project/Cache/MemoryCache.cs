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

using System.Threading;


namespace HashTag.Cache
{
    /// <summary>
    /// Simple in-memory cache; used for development until enterprise caching (memcached, AppFrabric, etc) can be used
    /// </summary>
    public class MemoryCache : ICacheProvider
    {
        private static Dictionary<string, object> _list = new Dictionary<string, object>();
        private static ReaderWriterLockSlim _lock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        public void Remove(string key)
        {
            try
            {
                _lock.EnterWriteLock();
                _list.Remove(key);
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public object Get(string key)
        {
            try
            {
                _lock.EnterReadLock();
                return _list[key];
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        public object this[string key]
        {

            get
            {
                return Get(key);
            }
        }

        public T Get<T>(string key)
        {
            return (T)Get(key);
        }

        public T Get<T>(string key, T defaultValue)
        {
            if (HasKey(key) == false) return defaultValue;
            return Get<T>(key);
        }

        public bool HasKey(string key)
        {
            try
            {
                _lock.EnterReadLock();
                return _list.ContainsKey(key);
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        public void Store(string key, object data)
        {
            try
            {
                _lock.EnterWriteLock();
                _list[key] = data;
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public void Flush()
        {
            try
            {
                _lock.EnterWriteLock();
                _list.Clear();
            }
            finally
            {
                _lock.EnterWriteLock();
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        public void Dispose(bool isDisposing)
        {
            if (isDisposing == true)
            {
                try
                {
                    _lock.EnterWriteLock();
                    foreach (var kvp in _list)
                    {
                        try
                        {
                            if (kvp.Value is IDisposable)
                            {
                                ((IDisposable)kvp.Value).Dispose();
                            }
                        }
                        catch
                        {  //ignore disposing exceptions
                        }
                    }
                }
                finally
                {
                    _lock.ExitWriteLock();
                }
                _lock.Dispose();
                GC.SuppressFinalize(this);
            }

        }
        ~MemoryCache()
        {
            Dispose(false);
        }
    }
}
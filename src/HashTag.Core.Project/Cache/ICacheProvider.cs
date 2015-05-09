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

namespace HashTag.Cache
{
    public interface ICacheProvider:IDisposable 
    {

        void Remove(string key);
        object Get(string key);
        T Get<T>(string key);
        T Get<T>(string key, T defaultValue);
        bool HasKey(string key);
        void Store(string key, object data);
        //void Store(string key, object data, DateTime expiryTime);
        void Flush();
        object this[string key] { get; }
    }

}
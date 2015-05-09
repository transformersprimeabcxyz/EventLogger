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
using HashTag.ServiceModel;

namespace HashTag
{
	public class ServiceParameter
	{
		public ServiceParameter(){}

		public ServiceParameter(string key, object value)
		{
			Key = key;
			Value = value;

		}

		public string Key;
		public object Value { get; set; }
	}
	public interface IServiceLocator
	{
		IServiceLocator Register<I>(WcfEndPoint endPoint);
		I GetInstance<I>(params ServiceParameter[] constructorParameters);
		IServiceLocator Register<IFromInterface, TToConcrete>() where TToConcrete : class,IFromInterface;
		void Empty();
		T InternalContainer<T>() where T:class;
		bool ErrorOnMissingRegistration { get; set; }
	}
}
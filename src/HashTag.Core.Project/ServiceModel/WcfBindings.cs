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
using System.Runtime.Serialization;

namespace HashTag.ServiceModel
{
	
	/// <summary>
	/// List of supported Wcf Binding for library. See link for complete list of possible bindings and capabilities of each binding: http://msdn.microsoft.com/en-us/library/ms730879.aspx
	/// </summary>
	[DataContract]
	public enum WcfBindings
	{
		/// <summary>
		/// A binding that is suitable for communicating with WS-Basic Profile conformant Web services, 
		/// for example, ASP.NET Web services (ASMX)-based services. This binding uses HTTP as the 
		/// transport and text/XML as the default message encoding.
		/// </summary>
		[EnumMember]
		BasicHttp,

		/// <summary>
		/// A secure and interoperable binding that is suitable for non-duplex service contracts.
		/// </summary>
		[EnumMember]
		WsHttpBinding,

		/// <summary>
		/// A secure and optimized binding suitable for cross-machine communication between WCF applications.
		/// </summary>
		[EnumMember]
		NetTcp,

		/// <summary>
		/// A binding that is suitable for communicating with WS-Basic Profile conformant Web services that enables HTTP cookies to be used to exchange context.
		/// </summary>
		[EnumMember]
		BasicContext, 

		/// <summary>
		/// A binding used to configure endpoints for WCF Web services that are exposed through HTTP requests instead of SOAP messages. (e.g. JSON)
		/// </summary>
		[EnumMember]
		WebHttp,

		/// <summary>
		/// A secure and interoperable binding that is suitable for non-duplex service contracts that enables SOAP headers to be used to exchange context.
		/// </summary>
		[EnumMember]
		WsHttpContext, 

		/// <summary>
		/// Service end-point will instantiate service class directly and not through a hosted end-point.  Used most often during development
		/// process to enable more convenient debugging and local deployments (e.g. developer, team level, etc).  Not used frequently in production 
		/// environments
		/// </summary>
		[EnumMember]
		Direct,
	}
}
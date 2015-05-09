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
using System.ServiceModel.Activation;
using HashTag.Configuration;

namespace HashTag.ServiceModel
{
    /// <summary>
    /// Custom host factory to use when hosting WCF on shared hosting site.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Addresses: 'The collection already contains an address with scheme Http'
    /// </para>
    /// </remarks>
    /// <example>
    /// <![CDATA[ 
    /// First, open your .svc file. Add the "Factory" attribute to the ServiceHost declaration, replacing the web project name with your own: 
    /// <%@ ServiceHost Language="C#" Debug="true" Service="Concrete.Service.Class"
    ///    Factory="HashTag.ServiceModel.CustomHostFactory" %>
    /// ]]>
    /// </example>
    [Citation("http://www.itscodingtime.com/itscodingtime/post/Installing-a-WCF-Service-to-GoDaddy.aspx")]
    class WcfSharedHostFactory : ServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            // If more than one base address exists then return the second address,
            // otherwise return the first address
            if (baseAddresses.Length > 1) //this returns the second address in list.  Might need to change depending on hosting provider
            {
                return new ServiceHost(serviceType, baseAddresses[1]);
            }
            else
            {
                var serviceUri = new Uri(ConfigManager.AppSetting<string>("HashTag.ServiceModel.HostUri"));  //fully qualified name to location of service in shared hosted environment; often might be a full file path including .svc
                                                    //NOTE:  We check for existance of this .config entry even in non-production environments so
                                                    //       missing configuration is caught before being deployed to shared host
                var webServiceAddress = baseAddresses[0].ToString().Contains("localhost") ? baseAddresses[0] : serviceUri;
                return new ServiceHost(serviceType, webServiceAddress);
            }
        }
    }

    class WcfSharedHost : ServiceHost
    {
        public WcfSharedHost(Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        { 
        
        }
    }
}
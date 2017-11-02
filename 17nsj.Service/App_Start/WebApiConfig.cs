//----------------------------------------------------------------------
// <copyright file="WebApiConfig.cs" company="17NSJ PR Dept">
// Copyright (c) 17NSJ PR Dept. All rights reserved.
// </copyright>
// <summary>WebApiConfigクラス</summary>
//----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.OData.Extensions;
using Microsoft.Owin.Security.OAuth;

namespace _17nsj.Service
{
    /// <summary>
    /// WebApiConfigクラス
    /// </summary>
    public static class WebApiConfig
    {
        /// <summary>
        /// 設定を登録します。
        /// </summary>
        /// <param name="config">設定</param>
        public static void Register(HttpConfiguration config)
        {
            // Web API の設定およびサービス
            config.SuppressDefaultHostAuthentication();
            config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));

            // Web API ルート
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional });
        }
    }
}

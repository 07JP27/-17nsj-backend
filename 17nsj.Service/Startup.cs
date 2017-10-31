//----------------------------------------------------------------------
// <copyright file="Startup.cs" company="17NSJ PR Dept">
// Copyright (c) 17NSJ PR Dept. All rights reserved.
// </copyright>
// <summary>Startupクラス</summary>
//----------------------------------------------------------------------

using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Owin;

[assembly: OwinStartup(typeof(_17nsj.Service.Startup))]

namespace _17nsj.Service
{
    /// <summary>
    /// Startupクラス
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// owinの設定を行います。
        /// </summary>
        /// <param name="app">app</param>
        public void Configuration(IAppBuilder app)
        {
            app.UseOAuthBearerTokens(new OAuthAuthorizationServerOptions
            {
                TokenEndpointPath = new PathString("/Token"),
                Provider = new ApplicationOAuthProvider(),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(1), // 要件に応じて期限を
                AllowInsecureHttp = true
            });
        }
    }
}
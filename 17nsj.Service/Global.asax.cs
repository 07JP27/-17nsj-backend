//----------------------------------------------------------------------
// <copyright file="Global.asax.cs" company="17NSJ PR Dept">
// Copyright (c) 17NSJ PR Dept. All rights reserved.
// </copyright>
// <summary>WebApiApplicationクラス</summary>
//----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;

namespace _17nsj.Service
{
    /// <summary>
    /// WebApiApplicationクラス
    /// </summary>
    public class Global : System.Web.HttpApplication
    {
        /// <summary>
        /// アプリケーションスタート時に呼ばれます。
        /// </summary>
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}

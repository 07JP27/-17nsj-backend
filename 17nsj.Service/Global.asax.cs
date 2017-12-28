//----------------------------------------------------------------------
// <copyright file="Global.asax.cs" company="17NSJ PR Dept">
// Copyright (c) 17NSJ PR Dept. All rights reserved.
// </copyright>
// <summary>WebApiApplicationクラス</summary>
//----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;

namespace _17nsj.Service
{
    /// <summary>
    /// WebApiApplicationクラス
    /// </summary>
    public class Global : System.Web.HttpApplication
    {
        /// <summary>
        /// ApplicationInsights用テレメトリクライアント
        /// </summary>
        public static readonly TelemetryClient Telemetry = new TelemetryClient();

        /// <summary>
        /// アプリケーションスタート時に呼ばれます。
        /// </summary>
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            TelemetryConfiguration.Active.InstrumentationKey = ConfigurationManager.AppSettings["ApplicationInsightsInstrumentationKey"];

            // デバックモードの時はApplication Insightsへのログ送信を有効にしない
            if (Debugger.IsAttached)
            {
                TelemetryConfiguration.Active.DisableTelemetry = true;
            }
        }
    }
}

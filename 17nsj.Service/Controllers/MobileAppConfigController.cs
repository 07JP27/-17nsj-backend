//----------------------------------------------------------------------
// <copyright file="MobileAppConfigController.cs" company="17NSJ PR Dept">
// Copyright (c) 17NSJ PR Dept. All rights reserved.
// </copyright>
// <summary>MobileAppConfigControllerクラス</summary>
//----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using _17nsj.DataAccess;

namespace _17nsj.Service.Controllers
{
    /// <summary>
    /// MobileAppConfigControllerクラス
    /// </summary>
    [Authorize]
    [RoutePrefix("api/mobile_app_config")]
    public class MobileAppConfigController : ControllerBase
    {
        /// <summary>
        /// モバイルアプリ設定情報を取得します。
        /// </summary>
        /// <returns>モバイルアプリ設定情報</returns>
        [HttpGet]
        [Route("")]
        public HttpResponseMessage Get()
        {
            if (!this.CanRead())
            {
                return this.Request.CreateResponse(HttpStatusCode.Forbidden);
            }

            using (Entities entitiies = new Entities())
            {
                var entity = entitiies.MobileAppConfig.FirstOrDefault();

                if (entity != null)
                {
                    return this.Request.CreateResponse(HttpStatusCode.OK, entity);
                }
                else
                {
                    return this.Request.CreateErrorResponse(HttpStatusCode.NotFound, "Not found");
                }
            }
        }
    }
}

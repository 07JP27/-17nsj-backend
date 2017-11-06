//----------------------------------------------------------------------
// <copyright file="NewspapersController.cs" company="17NSJ PR Dept">
// Copyright (c) 17NSJ PR Dept. All rights reserved.
// </copyright>
// <summary>NewspapersControllerクラス</summary>
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
    /// NewspapersControllerクラス
    /// </summary>
    [Authorize]
    [RoutePrefix("api/newspapers")]
    public class NewspapersController : ControllerBase
    {
        /// <summary>
        /// ユーザー情報を取得します。
        /// </summary>
        /// <returns>HTTPレスポンス</returns>
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
                var newspapers = entitiies.Newspapers.ToList();

                if (newspapers != null)
                {
                    return this.Request.CreateResponse(HttpStatusCode.OK, newspapers);
                }
                else
                {
                    return this.Request.CreateErrorResponse(HttpStatusCode.NotFound, "Not found");
                }
            }
        }
    }
}

//----------------------------------------------------------------------
// <copyright file="SchedulesController.cs" company="17NSJ PR Dept">
// Copyright (c) 17NSJ PR Dept. All rights reserved.
// </copyright>
// <summary>SchedulesControllerクラス</summary>
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
    [RoutePrefix("api/Schedules")]
    public class SchedulesController : ControllerBase
    {
        /// <summary>
        /// スケジュール情報を取得します。
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
                var schedules = entitiies.Schedules.ToList();

                if (schedules != null)
                {
                    return this.Request.CreateResponse(HttpStatusCode.OK, schedules);
                }
                else
                {
                    return this.Request.CreateErrorResponse(HttpStatusCode.NotFound, "Not found");
                }
            }
        }
    }
}

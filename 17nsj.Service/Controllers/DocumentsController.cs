//----------------------------------------------------------------------
// <copyright file="DocumentsController.cs" company="17NSJ PR Dept">
// Copyright (c) 17NSJ PR Dept. All rights reserved.
// </copyright>
// <summary>DocumentsControllerクラス</summary>
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
    /// documentsControllerクラス
    /// </summary>
    [Authorize]
    [RoutePrefix("api/documents")]
    public class DocumentsController : ControllerBase
    {
        /// <summary>
        /// ドキュメント情報を取得します。
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
                var doc = entitiies.Documents.ToList();

                if (doc != null)
                {
                    return this.Request.CreateResponse(HttpStatusCode.OK, doc);
                }
                else
                {
                    return this.Request.CreateErrorResponse(HttpStatusCode.NotFound, "Not found");
                }
            }
        }
    }
}
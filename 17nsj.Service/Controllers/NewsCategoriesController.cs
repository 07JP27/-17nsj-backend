﻿//----------------------------------------------------------------------
// <copyright file="NewsCategoriesController.cs" company="17NSJ PR Dept">
// Copyright (c) 17NSJ PR Dept. All rights reserved.
// </copyright>
// <summary>NewsCategoriesControllerクラス</summary>
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
    /// NewsCategoriesControllerクラス
    /// </summary>
    [Authorize]
    [RoutePrefix("api/news_categories")]
    public class NewsCategoriesController : ControllerBase
    {
        /// <summary>
        /// ニュースカテゴリ情報を取得します。
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
                var newsCategories = entitiies.NewsCategories.ToList();

                if (newsCategories != null)
                {
                    return this.Request.CreateResponse(HttpStatusCode.OK, newsCategories);
                }
                else
                {
                    return this.Request.CreateErrorResponse(HttpStatusCode.NotFound, "Not found");
                }
            }
        }
    }
}

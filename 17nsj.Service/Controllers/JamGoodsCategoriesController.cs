//----------------------------------------------------------------------
// <copyright file="JamGoodsCategoriesController.cs" company="17NSJ PR Dept">
// Copyright (c) 17NSJ PR Dept. All rights reserved.
// </copyright>
// <summary>JamGoodsCategoriesControllerクラス</summary>
//----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using _17nsj.DataAccess;

namespace _17nsj.Service.Controllers
{
    /// <summary>
    /// JamGoodsCategoriesControllerクラス
    /// </summary>
    [Authorize]
    [RoutePrefix("api/jamgoods_categories")]
    public class JamGoodsCategoriesController : ControllerBase
    {
        /// <summary>
        /// グッズカテゴリ情報を取得します。
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
                var goodsCategories = entitiies.JamGoodsCategories.ToList();

                if (goodsCategories != null)
                {
                    return this.Request.CreateResponse(HttpStatusCode.OK, goodsCategories);
                }
                else
                {
                    return this.Request.CreateErrorResponse(HttpStatusCode.NotFound, "Not found");
                }
            }
        }
    }
}

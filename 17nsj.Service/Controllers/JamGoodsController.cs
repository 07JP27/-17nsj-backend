//----------------------------------------------------------------------
// <copyright file="JamGoodsController.cs" company="17NSJ PR Dept">
// Copyright (c) 17NSJ PR Dept. All rights reserved.
// </copyright>
// <summary>JamGoodsControllerクラス</summary>
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
    /// JamGoodsControllerクラス
    /// </summary>
    [Authorize]
    [RoutePrefix("api/jamgoods")]
    public class JamGoodsController : ControllerBase
    {
        /// <summary>
        /// 全てのグッズ情報を取得します。
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
                var goods = entitiies.JamGoods.Where(e => e.IsAvailable == true).OrderByDescending(e => e.Id).ToList();

                if (goods != null)
                {
                    return this.Request.CreateResponse(HttpStatusCode.OK, goods);
                }
                else
                {
                    return this.Request.CreateErrorResponse(HttpStatusCode.NotFound, "Not found");
                }
            }
        }
    }
}

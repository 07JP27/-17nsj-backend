//----------------------------------------------------------------------
// <copyright file="NewsController.cs" company="17NSJ PR Dept">
// Copyright (c) 17NSJ PR Dept. All rights reserved.
// </copyright>
// <summary>NewsControllerクラス</summary>
//----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using _17nsj.DataAccess;

namespace _17nsj.Service.Controllers
{
    /// <summary>
    /// NewsControllerクラス
    /// </summary>
    [Authorize]
    [RoutePrefix("api/news")]
    public class NewsController : ControllerBase
    {
        /// <summary>
        /// 全てのニュース情報を取得します。
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
                var news = entitiies.News.Where(e => e.IsAvailable == true).OrderByDescending(e => e.CreatedAt).ToList();

                if (news != null)
                {
                    return this.Request.CreateResponse(HttpStatusCode.OK, news);
                }
                else
                {
                    return this.Request.CreateErrorResponse(HttpStatusCode.NotFound, "Not found");
                }
            }
        }

        /// <summary>
        /// カテゴリを指定してニュース情報を取得します。
        /// </summary>
        /// <param name="category">category</param>
        /// <returns>HTTPレスポンス</returns>
        [HttpGet]
        [Route("{category}")]
        public HttpResponseMessage Get(string category)
        {
            if (!this.CanRead())
            {
                return this.Request.CreateResponse(HttpStatusCode.Forbidden);
            }

            using (Entities entitiies = new Entities())
            {
                var news = entitiies.News.Where(e => e.Category == category && e.IsAvailable == true).OrderByDescending(e => e.CreatedAt).ToList();

                if (news != null)
                {
                    return this.Request.CreateResponse(HttpStatusCode.OK, news);
                }
                else
                {
                    return this.Request.CreateErrorResponse(HttpStatusCode.NotFound, "Not found");
                }
            }
        }

        /// <summary>
        /// カテゴリとIDを指定してニュース情報を取得します。
        /// </summary>
        /// <param name="category">category</param>
        /// <param name="id">id</param>
        /// <returns>HTTPレスポンス</returns>
        [HttpGet]
        [Route("{category}/{id}")]
        public HttpResponseMessage Get(string category, int id)
        {
            if (!this.CanRead())
            {
                return this.Request.CreateResponse(HttpStatusCode.Forbidden);
            }

            using (Entities entitiies = new Entities())
            {
                var news = entitiies.News.FirstOrDefault(e => e.Category == category && e.Id == id && e.IsAvailable == true);

                if (news != null)
                {
                    return this.Request.CreateResponse(HttpStatusCode.OK, news);
                }
                else
                {
                    return this.Request.CreateErrorResponse(HttpStatusCode.NotFound, "Not found");
                }
            }
        }

        /// <summary>
        /// ニュース情報を登録します。
        /// </summary>
        /// <param name="news">ニュース情報</param>
        /// <returns>HTTPレスポンス</returns>
        [HttpPost]
        [Route("")]
        public HttpResponseMessage Post([FromBody] News news)
        {
            // 権限チェック
            if (!this.CanWrite())
            {
                return this.Request.CreateResponse(HttpStatusCode.Forbidden);
            }

            // オブジェクト自体のnullチェック
            if (news == null)
            {
                return this.Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid News object.");
            }

            // 各値のnullチェック
            var validationResult = this.ValidateUserModel(news);

            if (validationResult != null)
            {
                return this.Request.CreateResponse(HttpStatusCode.BadRequest, validationResult);
            }

            // 登録
            news.Category = news.Category.ToUpper();
            news.CreatedBy = this.UserId;
            news.CreatedAt = DateTime.Now;

            using (Entities entitiies = new Entities())
            using (var tran = entitiies.Database.BeginTransaction(IsolationLevel.Serializable))
            {
                try
                {
                    // 該当行が１行もないとMax値をとれないので行数チェック
                    var count = entitiies.News.Where(e => e.Category == news.Category && e.IsAvailable == true).Count();
                    int maxId;

                    if (count == 0)
                    {
                        maxId = 0;
                    }
                    else
                    {
                        maxId = entitiies.News.Where(e => e.Category == news.Category && e.IsAvailable == true).Max(e => e.Id);
                    }

                    news.Id = maxId + 1;

                    entitiies.News.Add(news);
                    entitiies.SaveChanges();

                    tran.Commit();
                    var message = this.Request.CreateResponse(HttpStatusCode.Created, news);
                    message.Headers.Location = new Uri(this.Request.RequestUri + "/" + news.Category + "/" + news.Id.ToString());
                    return message;
                }
                catch (Exception e)
                {
                    tran.Rollback();
                    return this.Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
                }
            }
        }

        /// <summary>
        /// ニュース情報登録前の検証を行います。
        /// </summary>
        /// <param name="news">ニュース情報</param>
        /// <returns>エラーメッセージ</returns>
        private string ValidateUserModel(News news)
        {
            if (string.IsNullOrEmpty(news.Category) || news.Category.Length != 1)
            {
                return "Invalid Category.";
            }

            if (string.IsNullOrEmpty(news.Author) || news.Author.Length > 30)
            {
                return "Invalid  Author.";
            }

            if (string.IsNullOrEmpty(news.Title) || news.Title.Length > 30)
            {
                return "Invalid Title.";
            }

            return null;
        }
    }
}

//----------------------------------------------------------------------
// <copyright file="NoticesController.cs" company="17NSJ PR Dept">
// Copyright (c) 17NSJ PR Dept. All rights reserved.
// </copyright>
// <summary>NoticesControllerクラス</summary>
//----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using System.Web.Http.OData.Query;
using _17nsj.DataAccess;

namespace _17nsj.Service.Controllers
{
    /// <summary>
    /// NoticesControllerクラス
    /// </summary>
    [Authorize]
    [RoutePrefix("api/notices")]
    public class NoticesController : ControllerBase
    {
        /// <summary>
        /// お知らせ情報を取得します。
        /// </summary>
        /// <param name="options">ODataオプション</param>
        /// <returns>HTTPレスポンス</returns>
        [HttpGet]
        [Route("")]
        public HttpResponseMessage Get(ODataQueryOptions<Notices> options)
        {
            if (!this.CanRead())
            {
                return this.Request.CreateResponse(HttpStatusCode.Forbidden);
            }

            using (Entities entitiies = new Entities())
            {
                var notices = entitiies.Notices.Where(e => e.IsAvailable == true).OrderByDescending(e => e.CreatedAt).ToList();
                var filterNotices = options.ApplyTo(notices.AsQueryable()) as IQueryable<Notices>;
                notices = filterNotices.ToList();

                if (notices != null)
                {
                    return this.Request.CreateResponse(HttpStatusCode.OK, notices);
                }
                else
                {
                    return this.Request.CreateErrorResponse(HttpStatusCode.NotFound, "Not found");
                }
            }
        }

        /// <summary>
        /// お知らせ情報を登録します。
        /// </summary>
        /// <param name="notice">お知らせ情報</param>
        /// <returns>HTTPレスポンス</returns>
        [HttpPost]
        [Route("")]
        public HttpResponseMessage Post([FromBody] Notices notice)
        {
            // 権限チェック
            if (!this.CanWrite())
            {
                return this.Request.CreateResponse(HttpStatusCode.Forbidden);
            }

            // オブジェクト自体のnullチェック
            if (notice == null)
            {
                return this.Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid Notice object.");
            }

            // 各値のnullチェック
            var validationResult = this.ValidateNoticeModel(notice);

            if (validationResult != null)
            {
                return this.Request.CreateResponse(HttpStatusCode.BadRequest, validationResult);
            }

            // 登録
            var userId = this.UserId;
            var now = DateTime.Now;
            notice.CreatedBy = userId;
            notice.CreatedAt = now;
            notice.UpdatedBy = userId;
            notice.UpdatedAt = now;

            using (Entities entitiies = new Entities())
            using (var tran = entitiies.Database.BeginTransaction(IsolationLevel.Serializable))
            {
                try
                {
                    // 該当行が１行もないとMax値をとれないので行数チェック
                    var count = entitiies.Notices.Count();
                    int maxId;

                    if (count == 0)
                    {
                        maxId = 0;
                    }
                    else
                    {
                        maxId = entitiies.Notices.Max(e => e.Id);
                    }

                    notice.Id = maxId + 1;

                    entitiies.Notices.Add(notice);
                    entitiies.SaveChanges();

                    tran.Commit();
                    var message = this.Request.CreateResponse(HttpStatusCode.Created, notice);
                    message.Headers.Location = new Uri(this.Request.RequestUri + "/" + notice.Id);
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
        /// お知らせ情報を更新します。
        /// </summary>
        /// <param name="id">id</param>
        /// <param name="newNotice">お知らせ情報</param>
        /// <returns>HTTPレスポンス</returns>
        [HttpPatch]
        [Route("{id}")]
        public HttpResponseMessage Patch(int id, [FromBody] Notices newNotice)
        {
            // 権限チェック
            if (!this.IsAdmin())
            {
                return this.Request.CreateResponse(HttpStatusCode.Forbidden);
            }

            // オブジェクト自体のnullチェック
            if (newNotice == null)
            {
                return this.Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid Notice object.");
            }

            // 既存チェック
            using (Entities entitiies = new Entities())
            {
                var notice = entitiies.Notices.FirstOrDefault(e => e.Id == id);

                if (notice == null)
                {
                    return this.Request.CreateResponse(HttpStatusCode.BadRequest, $"The notice {id} not exists.");
                }
            }

            using (Entities entitiies = new Entities())
            using (var tran = entitiies.Database.BeginTransaction(IsolationLevel.Serializable))
            {
                try
                {
                    var notice = entitiies.Notices.Single(e => e.Id == id);
                    notice.Author = newNotice.Author;
                    notice.Title = newNotice.Title;
                    notice.Outline = newNotice.Outline;
                    notice.MediaURL = newNotice.MediaURL;
                    notice.UpdatedAt = DateTime.Now;
                    notice.UpdatedBy = this.UserId;

                    entitiies.SaveChanges();

                    tran.Commit();
                    var message = this.Request.CreateResponse(HttpStatusCode.Created, notice);
                    message.Headers.Location = new Uri(this.Request.RequestUri + "/" + notice.Id.ToString());
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
        /// お知らせ情報登録前の検証を行います。
        /// </summary>
        /// <param name="notices">お知らせ情報</param>
        /// <returns>エラーメッセージ</returns>
        private string ValidateNoticeModel(Notices notices)
        {
            if (string.IsNullOrEmpty(notices.Author) || notices.Author.Length > 30)
            {
                return "Invalid  Author.";
            }

            if (string.IsNullOrEmpty(notices.Title) || notices.Title.Length > 30)
            {
                return "Invalid Title.";
            }

            if (string.IsNullOrEmpty(notices.Outline) || notices.Outline.Length > 500)
            {
                return "Invalid Category.";
            }

            return null;
        }
    }
}

//----------------------------------------------------------------------
// <copyright file="ProgramsController.cs" company="17NSJ PR Dept">
// Copyright (c) 17NSJ PR Dept. All rights reserved.
// </copyright>
// <summary>ProgramsControllerクラス</summary>
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
    /// ProgramsControllerクラス
    /// </summary>
    [Authorize]
    [RoutePrefix("api/programs")]
    public class ProgramsController : ControllerBase
    {
        /// <summary>
        /// プログラム情報を取得します。
        /// </summary>
        /// <param name="options">odataoption</param>
        /// <returns>HTTPレスポンス</returns>
        [HttpGet]
        [Route("")]
        public HttpResponseMessage Get(ODataQueryOptions<Programs> options)
        {
            if (!this.CanRead())
            {
                return this.Request.CreateResponse(HttpStatusCode.Forbidden);
            }

            using (Entities entitiies = new Entities())
            {
                var programs = entitiies.Programs.Where(e => e.IsAvailable == true).OrderByDescending(e => e.CreatedAt).ToList();
                var filterPrograms = options.ApplyTo(programs.AsQueryable()) as IQueryable<Programs>;
                programs = filterPrograms.ToList();

                if (programs != null)
                {
                    return this.Request.CreateResponse(HttpStatusCode.OK, programs);
                }
                else
                {
                    return this.Request.CreateErrorResponse(HttpStatusCode.NotFound, "Not found");
                }
            }
        }

        /// <summary>
        /// カテゴリを指定してプログラム情報を取得します。
        /// </summary>
        /// <param name="category">category</param>
        /// <param name="options">odataoption</param>
        /// <returns>HTTPレスポンス</returns>
        [HttpGet]
        [Route("{category}")]
        public HttpResponseMessage Get(string category, ODataQueryOptions<Programs> options)
        {
            if (!this.CanRead())
            {
                return this.Request.CreateResponse(HttpStatusCode.Forbidden);
            }

            using (Entities entitiies = new Entities())
            {
                var programs = entitiies.Programs.Where(e => e.Category == category && e.IsAvailable == true).OrderByDescending(e => e.CreatedAt).ToList();
                var filterPrograms = options.ApplyTo(programs.AsQueryable()) as IQueryable<Programs>;
                programs = filterPrograms.ToList();

                if (programs != null)
                {
                    return this.Request.CreateResponse(HttpStatusCode.OK, programs);
                }
                else
                {
                    return this.Request.CreateErrorResponse(HttpStatusCode.NotFound, "Not found");
                }
            }
        }

        /// <summary>
        /// カテゴリとIDを指定してプログラム情報を取得します。
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
                var programs = entitiies.Programs.FirstOrDefault(e => e.Category == category && e.Id == id && e.IsAvailable == true);

                if (programs != null)
                {
                    return this.Request.CreateResponse(HttpStatusCode.OK, programs);
                }
                else
                {
                    return this.Request.CreateErrorResponse(HttpStatusCode.NotFound, "Not found");
                }
            }
        }

        /// <summary>
        /// プログラム情報を登録します。
        /// </summary>
        /// <param name="programs">プログラム情報</param>
        /// <returns>HTTPレスポンス</returns>
        [HttpPost]
        [Route("")]
        public HttpResponseMessage Post([FromBody] Programs programs)
        {
            // 権限チェック
            if (!this.CanWrite())
            {
                return this.Request.CreateResponse(HttpStatusCode.Forbidden);
            }

            // オブジェクト自体のnullチェック
            if (programs == null)
            {
                return this.Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid Programs object.");
            }

            // 各値のnullチェック
            var validationResult = this.ValidateProgramsModel(programs);

            if (validationResult != null)
            {
                return this.Request.CreateResponse(HttpStatusCode.BadRequest, validationResult);
            }

            // 登録
            var userId = this.UserId;
            var now = DateTime.Now;
            programs.CreatedBy = userId;
            programs.CreatedAt = now;
            programs.UpdatedBy = userId;
            programs.UpdatedAt = now;

            using (Entities entitiies = new Entities())
            using (var tran = entitiies.Database.BeginTransaction(IsolationLevel.Serializable))
            {
                try
                {
                    // 該当行が１行もないとMax値をとれないので行数チェック
                    var count = entitiies.Programs.Where(e => e.Category == programs.Category).Count();
                    int maxId;

                    if (count == 0)
                    {
                        maxId = 0;
                    }
                    else
                    {
                        maxId = entitiies.Programs.Where(e => e.Category == programs.Category).Max(e => e.Id);
                    }

                    programs.Id = maxId + 1;

                    entitiies.Programs.Add(programs);
                    entitiies.SaveChanges();

                    tran.Commit();
                    var message = this.Request.CreateResponse(HttpStatusCode.Created, programs);
                    message.Headers.Location = new Uri(this.Request.RequestUri + "/" + programs.Category + "/" + programs.Id.ToString());
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
        /// プログラム情報を更新します。
        /// </summary>
        /// <param name="category">カテゴリ</param>
        /// <param name="id">id</param>
        /// <param name="newPrograms">プログラム情報</param>
        /// <returns>HTTPレスポンス</returns>
        [HttpPatch]
        [Route("{category}/{id}")]
        public HttpResponseMessage Patch(string category, int id, [FromBody] Programs newPrograms)
        {
            // 権限チェック
            if (!this.IsAdmin())
            {
                return this.Request.CreateResponse(HttpStatusCode.Forbidden);
            }

            // オブジェクト自体のnullチェック
            if (newPrograms == null)
            {
                return this.Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid Programs object.");
            }

            // 既存チェック
            using (Entities entitiies = new Entities())
            {
                var programs = entitiies.Programs.FirstOrDefault(e => e.Category == category && e.Id == id);

                if (programs == null)
                {
                    return this.Request.CreateResponse(HttpStatusCode.BadRequest, $"The Programs {category.ToUpper()}-{id} not exists.");
                }
            }

            using (Entities entitiies = new Entities())
            using (var tran = entitiies.Database.BeginTransaction(IsolationLevel.Serializable))
            {
                try
                {
                    var programs = entitiies.Programs.Single(e => e.Category == category && e.Id == id);
                    programs.Title = newPrograms.Title;
                    programs.Outline = newPrograms.Outline;
                    programs.MediaURL = newPrograms.MediaURL;
                    programs.UpdatedAt = DateTime.Now;
                    programs.UpdatedBy = this.UserId;

                    entitiies.SaveChanges();

                    tran.Commit();
                    var message = this.Request.CreateResponse(HttpStatusCode.Created, programs);
                    message.Headers.Location = new Uri(this.Request.RequestUri + "/" + programs.Category + "/" + programs.Id.ToString());
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
        /// プログラム情報登録前の検証を行います。
        /// </summary>
        /// <param name="programs">プログラム情報</param>
        /// <returns>エラーメッセージ</returns>
        private string ValidateProgramsModel(Programs programs)
        {
            if (string.IsNullOrEmpty(programs.Category) || programs.Category.Length != 1)
            {
                return "Invalid Category.";
            }

            if (string.IsNullOrEmpty(programs.Title) || programs.Title.Length > 30)
            {
                return "Invalid Title.";
            }

            if (string.IsNullOrEmpty(programs.Outline) || programs.Outline.Length > 500)
            {
                return "Invalid  Outline.";
            }

            return null;
        }
    }
}

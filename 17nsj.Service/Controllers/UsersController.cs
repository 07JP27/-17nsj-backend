//----------------------------------------------------------------------
// <copyright file="UsersController.cs" company="17NSJ PR Dept">
// Copyright (c) 17NSJ PR Dept. All rights reserved.
// </copyright>
// <summary>UsersControllerクラス</summary>
//----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using _17nsj.DataAccess;
using Microsoft.ApplicationInsights.DataContracts;

namespace _17nsj.Service.Controllers
{
    /// <summary>
    /// UsersControllerクラス
    /// </summary>
    [Authorize]
    [RoutePrefix("api/users")]
    public class UsersController : ControllerBase
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
                var users = entitiies.Users.Where(e => e.IsAvailable == true).ToList();

                if (users != null)
                {
                    foreach (var user in users)
                    {
                        // セキュリティ保護のためパスワードは返さない
                        user.Password = string.Empty;
                    }

                    return this.Request.CreateResponse(HttpStatusCode.OK, users);
                }
                else
                {
                    return this.Request.CreateErrorResponse(HttpStatusCode.NotFound, "Not found");
                }
            }
        }

        /// <summary>
        /// ユーザー情報を取得します。
        /// </summary>
        /// <param name="id">user id</param>
        /// <returns>HTTPレスポンス</returns>
        [HttpGet]
        [Route("{id}")]
        public HttpResponseMessage Get(string id)
        {
            if (!this.CanRead())
            {
                return this.Request.CreateResponse(HttpStatusCode.Forbidden);
            }

            using (Entities entitiies = new Entities())
            {
                var user = entitiies.Users.FirstOrDefault(e => e.UserId == id && e.IsAvailable == true);

                if (user != null)
                {
                    // セキュリティ保護のためパスワードは返さない
                    user.Password = string.Empty;
                    return this.Request.CreateResponse(HttpStatusCode.OK, user);
                }
                else
                {
                    return this.Request.CreateErrorResponse(HttpStatusCode.NotFound, "Not found");
                }
            }
        }

        /// <summary>
        /// ユーザー情報を登録します。
        /// </summary>
        /// <param name="user">ユーザー情報</param>
        /// <returns>HTTPレスポンス</returns>
        [HttpPost]
        [Route("")]
        public HttpResponseMessage Post([FromBody] Users user)
        {
            // 権限チェック
            if (!this.IsAdmin())
            {
                return this.Request.CreateResponse(HttpStatusCode.Forbidden);
            }

            // オブジェクト自体のnullチェック
            if (user == null)
            {
                return this.Request.CreateResponse(HttpStatusCode.BadRequest, "The User is null.");
            }

            // 各値のnullチェック
            var validationResult = this.ValidateUserModel(user);

            if (validationResult != null)
            {
                return this.Request.CreateResponse(HttpStatusCode.BadRequest, validationResult);
            }

            // 既存チェック
            using (Entities entitiies = new Entities())
            {
                var entity = entitiies.Users.FirstOrDefault(e => e.UserId == user.UserId);

                if (entity != null)
                {
                    return this.Request.CreateResponse(HttpStatusCode.BadRequest, $"The user {user.UserId} already exists.");
                }
            }

            // 登録
            var userId = this.UserId;
            var now = DateTime.Now;
            user.CreatedBy = userId;
            user.CreatedAt = now;
            user.UpdatedBy = userId;
            user.UpdatedAt = now;

            try
            {
                using (Entities entitiies = new Entities())
                {
                    entitiies.Users.Add(user);
                    entitiies.SaveChanges();

                    Global.Telemetry.TrackTrace($"【登録】[{this.UserId}]によってユーザー[{user.UserId}({user.DisplayName})]が登録されました。", SeverityLevel.Information);

                    var message = this.Request.CreateResponse(HttpStatusCode.Created, user);
                    message.Headers.Location = new Uri(this.Request.RequestUri + "/" + user.UserId);
                    return message;
                }
            }
            catch (Exception e)
            {
                Global.Telemetry.TrackException(e);
                return this.Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        /// <summary>
        /// ユーザー情報登録前の検証を行います。
        /// </summary>
        /// <param name="user">ユーザー情報</param>
        /// <returns>エラーメッセージ</returns>
        private string ValidateUserModel(Users user)
        {
            if (string.IsNullOrEmpty(user.UserId) || user.UserId.Length > 30)
            {
                return "Invalid User ID.";
            }

            if (string.IsNullOrEmpty(user.DisplayName) || user.DisplayName.Length > 30)
            {
                return "Invalid DisplayName.";
            }

            if (string.IsNullOrEmpty(user.Password) || user.Password.Length > 100)
            {
                return "Invalid Password.";
            }

            return null;
        }
    }
}

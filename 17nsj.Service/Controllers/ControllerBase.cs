//----------------------------------------------------------------------
// <copyright file="ControllerBase.cs" company="17NSJ PR Dept">
// Copyright (c) 17NSJ PR Dept. All rights reserved.
// </copyright>
// <summary>ControllerBaseクラス</summary>
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
    /// ControllerBaseクラス
    /// </summary>
    public abstract class ControllerBase : ApiController
    {
        /// <summary>
        /// ログインユーザIDを取得します。
        /// </summary>
        /// <value>ユーザID</value>
        public string UserId
        {
            get
            {
                return ClaimsPrincipal.Current.Claims.FirstOrDefault(p => p.Type == ClaimTypes.GivenName).Value;
            }
        }

        /// <summary>
        /// ログインユーザーが管理者権限を持っているかを判定します。
        /// </summary>
        /// <returns>管理者権限があり、かつ書き込み権限があればtrue</returns>
        protected bool IsAdmin()
        {
            using (Entities entitiies = new Entities())
            {
                var entity = entitiies.Users.FirstOrDefault(e => e.UserId == this.UserId);

                if (entity != null)
                {
                    return entity.IsAdmin;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// ログインユーザーがデータベースのデータを読み込み権限を持っているかを判定します。
        /// </summary>
        /// <returns>読み込みが許可もしくはシステム管理者であればTrue</returns>
        protected bool CanRead()
        {
            using (Entities entitiies = new Entities())
            {
                var entity = entitiies.Users.FirstOrDefault(e => e.UserId == this.UserId);

                if (entity != null)
                {
                    return entity.CanRead || entity.IsAdmin;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// ログインユーザーがデータベースに対して書き込み権限を持っているかを判定します。
        /// </summary>
        /// <returns>書き込みが許可もしくはシステム管理者であればTrue</returns>
        protected bool CanWrite()
        {
            using (Entities entitiies = new Entities())
            {
                var entity = entitiies.Users.FirstOrDefault(e => e.UserId == this.UserId);

                if (entity != null)
                {
                    return entity.CanWrite || entity.IsAdmin;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}

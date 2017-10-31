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
        /// ログインユーザーがデータベースのデータを読み込みできるかを判定します。
        /// </summary>
        /// <returns>読み込みが許可されていればTrue</returns>
        protected bool CanRead()
        {
            using (Entities entitiies = new Entities())
            {
                var entity = entitiies.Users.FirstOrDefault(e => e.UserId == this.UserId);

                if (entity != null)
                {
                    return entity.CanRead;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// ログインユーザーがデータベースに対して書き込みできるかを判定します。
        /// </summary>
        /// <returns>書き込みが許可されていればTrue</returns>
        protected bool CanWrite()
        {
            using (Entities entitiies = new Entities())
            {
                var entity = entitiies.Users.FirstOrDefault(e => e.UserId == this.UserId);

                if (entity != null)
                {
                    return entity.CanWrite;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}

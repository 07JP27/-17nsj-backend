//----------------------------------------------------------------------
// <copyright file="ValueController.cs" company="17NSJ PR Dept">
// Copyright (c) 17NSJ PR Dept. All rights reserved.
// </copyright>
// <summary>ValueControllerクラス</summary>
//----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace _17nsj.Service.Controllers
{
    /// <summary>
    /// ValueControllerクラス
    /// </summary>
    [Authorize]
    public class ValueController : ApiController
    {
        /// <summary>
        /// テストメソッド
        /// </summary>
        /// <returns>string配列</returns>
        public IEnumerable<string> Get()
        {
            var query = HttpUtility.UrlDecode(this.Request.RequestUri.Query);

            return new string[] { "value1", "value2" };
        }

        /// <summary>
        /// テストメソッド
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>"value"</returns>
        public string Get(int id)
        {
            id.ToString();
            return "value";
        }

        /// <summary>
        /// テストメソッド
        /// </summary>
        /// <param name="value">value</param>
        public void Post([FromBody]string value)
        {
            value.ToString();
        }

        /// <summary>
        /// テストメソッド
        /// </summary>
        /// <param name="id">id</param>
        /// <param name="value">value</param>
        public void Put(int id, [FromBody]string value)
        {
            id.ToString();
            value.ToString();
        }

        /// <summary>
        /// テストメソッド
        /// </summary>
        /// <param name="id">id</param>
        public void Delete(int id)
        {
            id.ToString();
        }
    }
}

//----------------------------------------------------------------------
// <copyright file="ApplicationOAuthProvider.cs" company="17NSJ PR Dept">
// Copyright (c) 17NSJ PR Dept. All rights reserved.
// </copyright>
// <summary>ApplicationOAuthProviderクラス</summary>
//----------------------------------------------------------------------

using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using _17nsj.DataAccess;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Owin;

namespace _17nsj.Service
{
    /// <summary>
    /// ApplicationOAuthProviderクラス
    /// </summary>
    public class ApplicationOAuthProvider : OAuthAuthorizationServerProvider
    {
        /// <summary>
        /// 通常認証
        /// </summary>
        /// <param name="context">context</param>
        /// <returns>task</returns>
        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
            return Task.FromResult(0);
        }

        /// <summary>
        /// grant_type=passwordの時の認証処理
        /// </summary>
        /// <param name="context">context</param>
        /// <returns>task</returns>
        public override Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            Users user;

            using (Entities entities = new Entities())
            {
                user = entities.Users.FirstOrDefault(e => e.UserId == context.UserName && e.IsAvailable == true);
            }

            if (user == null)
            {
                context.Rejected();
                return Task.FromResult(0);
            }

            if (user.Password == context.Password)
            {
                // context.Options.AuthenticationTypeを使ってClaimsIdentityを作る
                var identity = new ClaimsIdentity(context.Options.AuthenticationType);

                // Claimを追加。
                identity.AddClaims(new[]
                {
                    new Claim(ClaimTypes.GivenName, context.UserName),
                });
                context.Validated(identity);
            }
            else
            {
                context.Rejected();
            }

            return Task.FromResult(0);
        }
    }
}
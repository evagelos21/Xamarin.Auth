using System;
using System.Collections.Generic;
using Xamarin.Auth;

namespace Xamarin.Auth
{
    /// <summary>
    /// Request that is authenticated using an account retrieved from an <see cref="OAuth2Authenticator"/>.
    /// </summary>
    public class OAuth2Request : Request
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OAuth2Request"/> class.
        /// </summary>
        /// <param name='method'>The HTTP method.</param>
        /// <param name='url'>The URL.</param>
        /// <param name='parameters'>
        /// Parameters that will pre-populate the <see cref="Request.Parameters"/> property or <c>null</c>.
        /// </param>
        /// <param name='account'>The account used to authenticate this request.</param>
        public OAuth2Request(string method, Uri url, IDictionary<string, string> parameters, Account account)
            : base(method, url, parameters, account)
        {
            AccessTokenParameterName = "access_token";
        }

        /// <summary>
        /// Gets the OAuth2 prepared URL.
        /// </summary>
        /// <returns>The OAuth2 prepared URL.</returns>
        protected override Uri GetPreparedUrl()
        {
            return GetAuthenticatedUrl(Account, base.GetPreparedUrl(), AccessTokenParameterName);
        }

        /// <summary>
        /// Gets or sets the access token parameter name.
        /// </summary>
        /// <value><c>"access_token"</c> by default.</value>
        /// <remarks>
        /// Some providers, such as FourSquare do not use the somewhat-standard
        /// <c>"access_token"</c> name for their parameter.
        /// </remarks>
        public string AccessTokenParameterName { get; set; }

        /// <summary>
        /// Transforms an unauthenticated URL to an authenticated one.
        /// </summary>
        /// <returns>
        /// The authenticated URL.
        /// </returns>
        /// <param name='account'>The <see cref="Account"/> that's been authenticated.</param>
        /// <param name='unauthenticatedUrl'>The unauthenticated URL.</param>
        /// <param name='accessTokenParameterName'>The name of the access token parameter.</param>
        /// <seealso cref="AccessTokenParameterName"/>
        public static Uri GetAuthenticatedUrl(Account account, Uri unauthenticatedUrl, string accessTokenParameterName = "access_token")
        {
            if (account == null)
            {
                throw new ArgumentNullException("account");
            }
            if (!account.Properties.ContainsKey("access_token"))
            {
                throw new ArgumentException("OAuth2 account is missing required access_token property.", "account");
            }
            if (unauthenticatedUrl == null)
            {
                throw new ArgumentNullException("unauthenticatedUrl");
            }

            var url = unauthenticatedUrl.AbsoluteUri;

            if (url.Contains("?"))
            {
                url += "&" + accessTokenParameterName + "=" + account.Properties["access_token"];
            }
            else
            {
                url += "?" + accessTokenParameterName + "=" + account.Properties["access_token"];
            }

            return new Uri(url);
        }

        /// <summary>
        /// Gets an authenticated HTTP Authorization header.
        /// </summary>
        /// <returns>
        /// The authorization header.
        /// </returns>
        /// <param name='account'>The <see cref="Account"/> that's been authenticated.</param>
        public static string GetAuthorizationHeader(Account account)
        {
            if (account == null)
            {
                throw new ArgumentNullException("account");
            }
            if (!account.Properties.ContainsKey("access_token"))
            {
                throw new ArgumentException("OAuth2 account is missing required access_token property.", "account");
            }

            return "Bearer " + account.Properties["access_token"];
        }
    }
}

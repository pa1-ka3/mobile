using System;
using System.Collections.Generic;
using System.Text;

namespace PhotoQwrekster.Helpers
{
    /// <summary>
    /// Class containing details of the OAUth request token returned by Flickr.
    /// </summary>
    public class OAuthFlickrRequestToken
    {
        /// <summary>
        /// The request token string.
        /// </summary>
        public string Token { get; set; }
        /// <summary>
        /// The request token secret.
        /// </summary>
        public string TokenSecret { get; set; }


        /// Is token confirmed
        /// 
        public string TokenConfirmed { get; set; }

        /// <summary>
        /// Parses a URL parameter encoded string and returns a new <see cref="OAuthFlickrRequestToken"/>
        /// </summary>
        /// <param name="response">A URL parameter encoded string, e.g. "oauth_token=ABC&amp;oauth_token_secret=DEF".</param>
        /// <returns></returns>
        public static OAuthFlickrRequestToken ParseResponse(string response)
        {
            Dictionary<string, string> parameters = HelperUtility.StringToDictionary(response);
            OAuthFlickrRequestToken token = new OAuthFlickrRequestToken();
            token.Token = parameters["oauth_token"];
            token.TokenSecret = parameters["oauth_token_secret"];
            token.TokenConfirmed = parameters["oauth_callback_confirmed"];
            return token;
        }
    }
}

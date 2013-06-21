using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using RestSharp;
using PhotoQwrekster.Model;
using System.Threading;
using System.IO;
using Hammock.Authentication.OAuth;

namespace PhotoQwrekster.Helpers
{
    public class TwitterHelper
    {
        public static void UploadMedia(byte[] jpegMediaBytes, string caption)
        {
            // TwitterOAuthHelper.RefreshTokenCode(p => TryUploadMedia(new List<object> { jpegMediaBytes, caption }));
           // TryUploadMedia(new List<object> { jpegMediaBytes, caption });
            uploadPhoto(new List<object> { jpegMediaBytes, caption });
        }
        /*    ,Authenticator = RestSharp.Authenticators.OAuth1Authenticator.ForProtectedResource(TwitterConfigHelper.ApiKey,
            TwitterConfigHelper.ClientSecret,
            TwitterConfigHelper.AccessToken,
            TwitterOAuthHelper.oauth_token_secret)*/

        
                public static void TryUploadMedia(IList<object> obj)
                {
                    var client = new RestClient { Timeout = TwitterConfigHelper.RequestTimeout
            
                    };
                    var request = new RestRequest(Method.POST)
                    {
                        RequestFormat = DataFormat.Json,
                        Resource = TwitterConfigHelper.UploadUrl

                    };

                    string caption = (string)obj[1];

                    Dictionary<string, string> parameters = new Dictionary<string, string>();

                    parameters.Add("oauth_consumer_key", TwitterConfigHelper.ApiKey);
                    string oauthnonce = Guid.NewGuid().ToString("N");
                    parameters.Add("oauth_nonce", oauthnonce);
                    parameters.Add("oauth_signature_method", "HMAC-SHA1");
                    string oauthtimestamp = HelperUtility.GetUnixTimestamp();
                    parameters.Add("oauth_timestamp", oauthtimestamp);
                    parameters.Add("oauth_token", TwitterConfigHelper.AccessToken);
                    //parameters.Add("oauth_token_secret", TwitterOAuthHelper.oauth_token_secret);
                    parameters.Add("oauth_verifier", TwitterOAuthHelper.oauth_verifier);
                    parameters.Add("oauth_version", "1.0a");
                    parameters.Add("status", caption);

                    string sig = TwitterOAuthHelper.OAuthCalculateSignature("POST", TwitterConfigHelper.UploadUrl, parameters, TwitterOAuthHelper.oauth_token_secret);
                    string authorizationFormat = @"OAuth oauth_token=""{0}"", oauth_signature_method=""HMAC-SHA1"", oauth_signature=""{1}"", oauth_consumer_key=""{2}"", oauth_timestamp=""{3}"", oauth_nonce=""{4}"", oauth_version=""1.0""";
                    string authorizationString = string.Format(authorizationFormat, TwitterConfigHelper.AccessToken, sig, TwitterConfigHelper.ApiKey, oauthtimestamp, oauthnonce);

                    request.AddHeader("Authorization", authorizationString);

                    request.AddParameter("oauth_consumer_key", TwitterConfigHelper.ApiKey);
                    request.AddParameter("oauth_nonce", oauthnonce);
                    request.AddParameter("oauth_signature", sig);
                    request.AddParameter("oauth_signature_method", "HMAC-SHA1");
                    request.AddParameter("oauth_timestamp", oauthtimestamp);
                    request.AddParameter("oauth_token", TwitterConfigHelper.AccessToken);
                    //request.AddParameter("oauth_token_secret", TwitterOAuthHelper.oauth_token_secret);
                    request.AddParameter("oauth_verifier", TwitterOAuthHelper.oauth_verifier);
                    request.AddParameter("oauth_version", "1.0a");
       
                    request.AddParameter("status", caption, ParameterType.RequestBody);



                    //request.AddParameter(contentType, serialized, ParameterType.RequestBody);
            
                    request.AddFile("media[]",
                        (requestStream) =>
                        {
                            byte[] jpegMediaBytes = (byte[])obj[0];
                            using (MemoryStream ms = new MemoryStream(jpegMediaBytes))
                            {
                                ms.CopyTo(requestStream);
                                ms.Flush();
                                ms.Close();
                            }
                        },
                    caption, "image/jpeg");

                    client.ExecuteAsync(request, response =>
                    {
                        try
                        {
                            if (response.StatusCode == HttpStatusCode.OK)
                            {
                                //            var callback = obj[1] as Action<Task>;
                                //          if (callback != null) callback((Task)obj[0]);

                            }
                            MessageBox.Show(response.Content);
                            //    else
                            //MessageBox.Show(message);
                        }
                        catch (WebException)
                        {
                            //MessageBox.Show("There network problem: \n\n" + e.Message + "\n\nPlease try again later.");
                        }
                        catch (Exception)
                        {
                            // MessageBox.Show(message + "\n\n" + e.Message);

                        }
                    });
                }
                
        public static void uploadPhoto(IList<object> obj)//Stream photoStream, string photoName)
        {
            var credentials = new OAuthCredentials
                    {
                        Type = OAuthType.ProtectedResource,
                        SignatureMethod = OAuthSignatureMethod.HmacSha1,
                        ParameterHandling = OAuthParameterHandling.HttpAuthorizationHeader,
                        ConsumerKey = TwitterConfigHelper.ApiKey,
                        ConsumerSecret = TwitterConfigHelper.ClientSecret,
                        Token = TwitterConfigHelper.AccessToken,
                        TokenSecret = TwitterOAuthHelper.oauth_token_secret,
                        Version = "1.0a"
                    };
            Hammock.RestClient restClient = new Hammock.RestClient
       {
           Authority = "https://api.twitter.com",
           HasElevatedPermissions = true,
           Credentials = credentials,
           Method = Hammock.Web.WebMethod.Post
       };
            Hammock.RestRequest restRequest = new Hammock.RestRequest
            {
                Path = "/1.1/statuses/update_with_media.json"
            };
            string caption = (string)obj[1];
            restRequest.AddParameter("status", caption);

            byte[] jpegMediaBytes = (byte[])obj[0];
            Stream stream = new MemoryStream(jpegMediaBytes);
            restRequest.AddFile("media[]", caption, stream, "image/jpg");



            restClient.BeginRequest(restRequest, new Hammock.RestCallback(PostTweetRequestCallback));
        }

        private static void PostTweetRequestCallback(Hammock.RestRequest request, Hammock.RestResponse response, object obj)
        {
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                //Success code
            }
          //  MessageBox.Show(response.Content);
        }

    }
}

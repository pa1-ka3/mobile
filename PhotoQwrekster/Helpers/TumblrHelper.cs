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
using System.Text;
using PhotoQwrekster.ViewModel;

namespace PhotoQwrekster.Helpers
{
    public class TumblrHelper
    {
        public static void UploadMedia(byte[] jpegMediaBytes, string caption)
        {
            // TumblrOAuthHelper.RefreshTokenCode(p => TryUploadMedia(new List<object> { jpegMediaBytes, caption }));
       //      TryUploadMedia(new List<object> { jpegMediaBytes, caption });
            uploadPhoto(new List<object> { jpegMediaBytes, caption });
         
        }
        /*    ,Authenticator = RestSharp.Authenticators.OAuth1Authenticator.ForProtectedResource(TumblrConfigHelper.ApiKey,
            TumblrConfigHelper.ClientSecret,
            TumblrConfigHelper.AccessToken,
            TumblrOAuthHelper.oauth_token_secret)*/

        /*
        public static void TryUploadMedia(IList<object> obj)
        {
            var client = new RestClient
            {
                Timeout = TumblrConfigHelper.RequestTimeout

            };
            var request = new RestRequest(Method.POST)
            {
                RequestFormat = DataFormat.Json,
                Resource = TumblrConfigHelper.UploadUrl

            };

            string caption = (string)obj[1];

            Dictionary<string, string> parameters = new Dictionary<string, string>();

            parameters.Add("oauth_consumer_key", TumblrConfigHelper.ApiKey);
            string oauthnonce = Guid.NewGuid().ToString("N");
            parameters.Add("oauth_nonce", oauthnonce);
            parameters.Add("oauth_signature_method", "HMAC-SHA1");
            string oauthtimestamp = HelperUtility.GetUnixTimestamp();
            parameters.Add("oauth_timestamp", oauthtimestamp);
            parameters.Add("oauth_token", TumblrConfigHelper.AccessToken);
            //parameters.Add("oauth_token_secret", TumblrOAuthHelper.oauth_token_secret);
            parameters.Add("oauth_verifier", TumblrOAuthHelper.oauth_verifier);
            parameters.Add("oauth_version", "1.0");
            //parameters.Add("status", caption);

            string sig = TumblrOAuthHelper.OAuthCalculateSignature("POST", TumblrConfigHelper.UploadUrl, parameters, TumblrOAuthHelper.oauth_token_secret);
            string authorizationFormat = @"OAuth oauth_token=""{0}"", oauth_signature_method=""HMAC-SHA1"", oauth_signature=""{1}"", oauth_consumer_key=""{2}"", oauth_timestamp=""{3}"", oauth_nonce=""{4}"", oauth_version=""1.0""";
            string authorizationString = string.Format(authorizationFormat, TumblrConfigHelper.AccessToken, sig, TumblrConfigHelper.ApiKey, oauthtimestamp, oauthnonce);

            request.AddHeader("Authorization", authorizationString);

            request.AddParameter("oauth_consumer_key", TumblrConfigHelper.ApiKey);
            request.AddParameter("oauth_nonce", oauthnonce);
            request.AddParameter("oauth_signature", sig);
            request.AddParameter("oauth_signature_method", "HMAC-SHA1");
            request.AddParameter("oauth_timestamp", oauthtimestamp);
            request.AddParameter("oauth_token", TumblrConfigHelper.AccessToken);
            //request.AddParameter("oauth_token_secret", TumblrOAuthHelper.oauth_token_secret);
            request.AddParameter("oauth_verifier", TumblrOAuthHelper.oauth_verifier);
            request.AddParameter("oauth_version", "1.0");

            request.AddParameter("status", caption, ParameterType.GetOrPost);
            request.AddParameter("type", "photo", ParameterType.GetOrPost);



            //request.AddParameter(contentType, serialized, ParameterType.RequestBody);

        
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
        */

        public static void TryUploadMedia(IList<object> obj)
        {
            var client = new RestClient { Timeout = TumblrConfigHelper.RequestTimeout };
            var request = new RestRequest(Method.POST)
            {
                RequestFormat = DataFormat.Json,
                Resource = TumblrConfigHelper.UploadUrl

            };

            string caption = (string)obj[1];
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            parameters.Add("oauth_consumer_key", TumblrConfigHelper.ApiKey);
            string oauthnonce = Guid.NewGuid().ToString("N");
            parameters.Add("oauth_nonce", oauthnonce);
            parameters.Add("oauth_signature_method", "HMAC-SHA1");
            string oauthtimestamp = HelperUtility.GetUnixTimestamp();
            parameters.Add("oauth_timestamp", oauthtimestamp);
            parameters.Add("oauth_token", TumblrOAuthHelper.oauth_token);
            parameters.Add("oauth_version", "1.0a");
            parameters.Add("caption", caption);
            parameters.Add("type", "photo");

            string sig = TumblrOAuthHelper.OAuthCalculateSignature("POST", TumblrConfigHelper.UploadUrl, parameters, TumblrOAuthHelper.oauth_token_secret);
            string authorizationFormat = @"OAuth oauth_consumer_key=""{0}"",oauth_nonce=""{1}"",oauth_signature=""{2}"",oauth_signature_method=""{3}"",oauth_timestamp=""{4}"",oauth_token=""{5}"",oauth_version=""{6}""";
            string authorizationString = string.Format(authorizationFormat,
                Uri.EscapeDataString(TumblrConfigHelper.ApiKey),
                Uri.EscapeDataString(oauthnonce),
                Uri.EscapeDataString(sig), 
                Uri.EscapeDataString("HMAC-SHA1"),
                Uri.EscapeDataString(oauthtimestamp),
                Uri.EscapeDataString(TumblrOAuthHelper.oauth_token), 
                Uri.EscapeDataString("1.0a"));

            request.AddHeader("Authorization", authorizationString);

    /*        request.AddParameter("oauth_consumer_key", Uri.EscapeDataString(TumblrConfigHelper.ApiKey));
            request.AddParameter("oauth_nonce", Uri.EscapeDataString(oauthnonce));
            request.AddParameter("oauth_signature", Uri.EscapeDataString(sig));
            request.AddParameter("oauth_signature_method", Uri.EscapeDataString("HMAC-SHA1"));
            request.AddParameter("oauth_timestamp", Uri.EscapeDataString(oauthtimestamp));
            request.AddParameter("oauth_token", Uri.EscapeDataString(TumblrOAuthHelper.oauth_token));
            request.AddParameter("oauth_version", Uri.EscapeDataString("1.0a"));
      */      

            
            request.AddParameter("caption", caption);
            request.AddParameter("type", "photo");

            //request.AddParameter("body", "karamba chu");
            //request.AddParameter("type", "text");

            request.AddFile("data",
                (requestStream) =>
                {
                    byte[] jpegMediaBytes = (byte[])obj[0];                    
                    string dataString = System.Web.HttpUtility.UrlEncode(jpegMediaBytes);
                    byte[] byteArray = Encoding.UTF8.GetBytes(dataString);
                  
                    using (MemoryStream ms = new MemoryStream(byteArray))
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
                    }
                    MessageBox.Show(response.Content);
                    
                }
                catch (WebException)
                {
                    
                }
                catch (Exception)
                {
                    

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
                ConsumerKey = TumblrConfigHelper.ApiKey,
                ConsumerSecret = TumblrConfigHelper.ClientSecret,
                Token = TumblrConfigHelper.AccessToken,
                TokenSecret = TumblrOAuthHelper.oauth_token_secret,
                Version = "1.0a"
            };
            Hammock.RestClient restClient = new Hammock.RestClient
            {
                Authority = "http://api.tumblr.com",
                HasElevatedPermissions = true,
                Credentials = credentials,
                Method = Hammock.Web.WebMethod.Post                 
            };
            Hammock.RestRequest restRequest = new Hammock.RestRequest
            {
                Path = "/v2/blog/pavelkaparykha.tumblr.com/post"
            };
            string caption = (string)obj[1];
            restRequest.AddParameter("caption", "caption");            
            restRequest.AddParameter("type", "photo");


            byte[] jpegMediaBytes = (byte[])obj[0];
            string dataString = System.Web.HttpUtility.UrlEncode(jpegMediaBytes);
            byte[] byteArray = Encoding.UTF8.GetBytes(dataString);

            Stream stream = new MemoryStream(byteArray);            
            restRequest.AddFile("data", caption, stream);//, "image/jpg");
            
            
            
           // restRequest.AddParameter("body", "zaremba @");
            //restRequest.AddParameter("type", "text");


            restClient.BeginRequest(restRequest, new Hammock.RestCallback(PostTweetRequestCallback));
        }

        private static void PostTweetRequestCallback(Hammock.RestRequest request, Hammock.RestResponse response, object obj)
        {
            string result;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                result = ImagePublished;
            }
            else
                result = NoSuccess;

            Runner.Invoke(() =>
            {
                INavigationService navigation = App.Container.Resolve<INavigationService>();
                navigation.NavigateTo(new Uri(ViewModelLocator.ShareInNet + "?result=" + result, UriKind.RelativeOrAbsolute));
            });
        }

        private const string ImagePublished = "image published to tumblr";
        private const string NoSuccess = "no success at upload to tumblr";

    }
}

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
using PhotoQwrekster.ViewModel;

namespace PhotoQwrekster.Helpers
{
    public class FlickrHelper
    {
        public static void UploadMedia(byte[] jpegMediaBytes, string caption)
        {
           // FlickrOAuthHelper.RefreshTokenCode(p => TryUploadMedia(new List<object> { jpegMediaBytes, caption }));
            TryUploadMedia(new List<object> { jpegMediaBytes, caption });
        }


        public static void TryUploadMedia(IList<object> obj)
        {   
            var client = new RestClient { Timeout = FlickrConfigHelper.RequestTimeout };
            var request = new RestRequest(Method.POST)
            {
            //    RequestFormat = DataFormat.Json,
                Resource = FlickrConfigHelper.UploadUrl
                
            };

            Dictionary<string, string> parameters = new Dictionary<string, string>();

            parameters.Add("oauth_consumer_key", FlickrConfigHelper.ApiKey);
            string oauthnonce = Guid.NewGuid().ToString("N");
            parameters.Add("oauth_nonce", oauthnonce);
            parameters.Add("oauth_signature_method", "HMAC-SHA1");
            string oauthtimestamp = HelperUtility.GetUnixTimestamp();
            parameters.Add("oauth_timestamp", oauthtimestamp);            
            parameters.Add("oauth_token", FlickrOAuthHelper.oauth_token);
            //parameters.Add("oauth_token_secret", FlickrOAuthHelper.oauth_token_secret);
            parameters.Add("oauth_verifier", FlickrOAuthHelper.oauth_verifier);
            parameters.Add("oauth_version", "1.0");

            string sig = FlickrOAuthHelper.OAuthCalculateSignature("POST", FlickrConfigHelper.UploadUrl, parameters, FlickrOAuthHelper.oauth_token_secret);
            string authorizationFormat = @"OAuth oauth_token=""{0}"", oauth_signature_method=""HMAC-SHA1"", oauth_signature=""{1}"", oauth_consumer_key=""{2}"", oauth_timestamp=""{3}"", oauth_nonce=""{4}"", oauth_version=""1.0"", oauth_verifier=""{5}""";
            string authorizationString = string.Format(authorizationFormat, FlickrOAuthHelper.oauth_token, sig, FlickrConfigHelper.ApiKey, oauthtimestamp, oauthnonce, FlickrOAuthHelper.oauth_verifier);

       //     request.AddHeader("Authorization", authorizationString);

            request.AddParameter("oauth_consumer_key", FlickrConfigHelper.ApiKey);
            request.AddParameter("oauth_nonce", oauthnonce);
            request.AddParameter("oauth_signature", sig);
            request.AddParameter("oauth_signature_method", "HMAC-SHA1");
            request.AddParameter("oauth_timestamp", oauthtimestamp);            
            request.AddParameter("oauth_token", FlickrOAuthHelper.oauth_token);
            //request.AddParameter("oauth_token_secret", FlickrOAuthHelper.oauth_token_secret);
            request.AddParameter("oauth_verifier", FlickrOAuthHelper.oauth_verifier);
            request.AddParameter("oauth_version", "1.0");
            


            //request.AddParameter(contentType, serialized, ParameterType.RequestBody);
            string caption = (string)obj[1];
            request.AddFile("photo",
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
                    string result;
                    if (response.StatusCode == HttpStatusCode.OK)
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

        private const string ImagePublished = "image published to flickr";
        private const string NoSuccess = "no success at upload to flickr";

    }
}

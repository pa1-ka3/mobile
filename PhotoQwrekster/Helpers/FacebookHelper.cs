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

namespace PhotoQwrekster.Helpers
{
    public class FacebookHelper
    {
        public static void UploadMedia(byte[] jpegMediaBytes, string caption)
        {
            FacebookOAuthHelper.RefreshTokenCode(p => TryUploadMedia(new List<object> { jpegMediaBytes, caption }));
        }


        public static void TryUploadMedia(IList<object> obj)//byte[] jpegMediaBytes, string caption)
        {
            //          const string message = "Some error while trying to delete the task.";
            var client = new RestClient { Authenticator = new OAuth2UriQueryParameterAuthenticator(FacebookConfigHelper.AccessToken), Timeout = FacebookConfigHelper.RequestTimeout };
            var request = new RestRequest(Method.POST)
            {
                RequestFormat = DataFormat.Json,
                Resource = "https://graph.facebook.com/me/photos?access_token=" + FacebookConfigHelper.AccessToken
            };

            client.UserAgent = "Facebook";

            string contentType = @"application/json";
            InstagramFirstPhase td = new InstagramFirstPhase() { device_timestamp = GetUnixTimestamp(), lat = "53.889254", lng = "27.439363" };
            string serialized = JsonConvert.SerializeObject(td, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

       //     request.AddParameter(contentType, serialized, ParameterType.RequestBody);
            string caption = (string)obj[1];
            request.AddFile("source",
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

        private static string GetUnixTimestamp()
        {
            return (DateTime.UtcNow - new DateTime(1970, 1, 1).ToUniversalTime()).TotalSeconds.ToString();
        }
       

    }
}

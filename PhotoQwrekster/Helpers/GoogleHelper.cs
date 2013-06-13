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
using RestSharp;
using Newtonsoft.Json;
using System.IO;
using PhotoQwrekster.Model;

namespace PhotoQwrekster.Helpers
{
    public class GoogleHelper
    {
        public static void UploadMedia(byte[] jpegMediaBytes, string caption)
        {
            GoogleOAuthHelper.RefreshTokenCode(p => TryUploadMedia(new List<object> { jpegMediaBytes, caption }));
        }


        public static void TryUploadMedia(IList<object> obj)//byte[] jpegMediaBytes, string caption)
        {
            //          const string message = "Some error while trying to delete the task.";
            var client = new RestClient { Authenticator = new OAuth2UriQueryParameterAuthenticator(GoogleConfigHelper.AccessToken), Timeout = GoogleConfigHelper.RequestTimeout };
            var request = new RestRequest(Method.POST)
            {
                RequestFormat = DataFormat.Json,
                Resource = "https://picasaweb.google.com/data/feed/api/user/default/albumid/default?access_token=" + GoogleConfigHelper.AccessToken
                
            };

            request.JsonSerializer.ContentType = "multipart/related";
            request.AddHeader("Accept", "application/json");
            
            client.UserAgent = "Google";

            string contentType = @"application/json";
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

        

    }
}

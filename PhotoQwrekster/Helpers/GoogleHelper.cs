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
using Newtonsoft.Json.Linq;
using System.Xml;
using System.Xml.Linq;
using PhotoQwrekster.ViewModel;

namespace PhotoQwrekster.Helpers
{
    public class GoogleHelper
    {
        public static void UploadMedia(byte[] jpegMediaBytes, string caption, Action<string> callbackAction)
        {
            MediaBytes = jpegMediaBytes;
            callback = callbackAction;
            GoogleOAuthHelper.RefreshTokenCode(p => TryUploadMedia(new List<object> { jpegMediaBytes, caption }));
        }

        private static Action<string> callback;

        private static byte[] mediaBytes;

        public static byte[] MediaBytes
        {
            get { return mediaBytes; }
            set { mediaBytes = value; }
        }

        public static void TryUploadMedia(IList<object> obj)//byte[] jpegMediaBytes, string caption)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri("https://picasaweb.google.com/data/feed/api/user/default/albumid/default?access_token=" + GoogleConfigHelper.AccessToken));
            request.Method = "POST";
            request.ContentType = "image/jpeg";

            
            request.Headers[HttpRequestHeader.Authorization] = "OAuth " + GoogleConfigHelper.AccessToken;            
            request.Headers["Slug"] = (string)obj[1];

            request.BeginGetRequestStream(new AsyncCallback(GetRequestStreamCallback), request);

        }

        private static void GetRequestStreamCallback(IAsyncResult asynchronousResult)
        {
            HttpWebRequest request = (HttpWebRequest)asynchronousResult.AsyncState;

            // End the operation
            Stream postStream = request.EndGetRequestStream(asynchronousResult);

            
            // Convert the string into a byte array. 
            byte[] byteArray = MediaBytes;

            // Write to the request stream.
            postStream.Write(byteArray, 0, MediaBytes.Length);
            postStream.Close();

            // Start the asynchronous operation to get the response
            request.BeginGetResponse(new AsyncCallback(GetResponseCallback), request);
        }

        private static void GetResponseCallback(IAsyncResult asynchronousResult)
        {
            HttpWebRequest request = (HttpWebRequest)asynchronousResult.AsyncState;

            // End the operation
            HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(asynchronousResult);
            Stream streamResponse = response.GetResponseStream();
            StreamReader streamRead = new StreamReader(streamResponse);
            string responseString = streamRead.ReadToEnd();
            string result = ParseResponse(responseString);
            
            //Console.WriteLine(responseString);
            // Close the stream object
            streamResponse.Close();
            streamRead.Close();

            // Release the HttpWebResponse
            response.Close();

            /*if (callback != null)
            {
                callback.Invoke(result);
            }*/
            Runner.Invoke(() =>
            {

                INavigationService navigation = App.Container.Resolve<INavigationService>();
                navigation.NavigateTo(new Uri(ViewModelLocator.ShareInNet + "?result=" + result, UriKind.RelativeOrAbsolute));
            });
        }

        private const string ImagePublished = "image published to picasa dropbox";
        private static string ParseResponse(string responseString)
        {
            
            XDocument doc = XDocument.Parse(responseString);
            string json = Newtonsoft.Json.JsonConvert.SerializeXNode(doc);
            var o = JObject.Parse(json);
            JToken t = o["entry"];
            JToken published = t.SelectToken("published");
            JToken content = o["entry"].SelectToken("content");
            
            if (published != null || content != null)
            {   
                JToken src = content.SelectToken("@src");
                string source = (src != null) ? src.ToString() : "";
                return ImagePublished + " " + source;
                
            }
            else return "no success at upload";
        }
    }
}

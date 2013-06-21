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
            MediaBytes = jpegMediaBytes;
            GoogleOAuthHelper.RefreshTokenCode(p => TryUploadMedia(new List<object> { jpegMediaBytes, caption }));
        }

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
            request.Headers["Slug"] = "test";

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
            Console.WriteLine(responseString);
            // Close the stream object
            streamResponse.Close();
            streamRead.Close();

            // Release the HttpWebResponse
            response.Close();
            //allDone.Set();
        }
    }
}

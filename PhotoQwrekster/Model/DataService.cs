using System;
using System.IO;

namespace PhotoQwrekster.Model
{
    public class DataService : IDataService
    {
        public void GetData(Action<DataItem, Exception> callback)
        {
            // Use this to connect to the actual data service

            var item = new DataItem("Welcome to MVVM Light");
            callback(item, null);
        }
        
        private Stream capturedStream;

        public Stream CapturedStream
        {
            get { return capturedStream; }
            set { capturedStream = value; }
        }

        public void SetCapturedImageStream(System.IO.Stream capturedStream)
        {
            CapturedStream = capturedStream;
        }


        private byte[] capturedImageByte;

        public byte[] CapturedImageByte
        {
            get { return capturedImageByte; }
            set { capturedImageByte = value; }
        }



        public void SetCupturedImageByte(byte[] dest1)
        {
            CapturedImageByte = dest1;
        }


        public Stream GetCapturedStream()
        {
            return CapturedStream;
        }


        public byte[] GetCapturedImageByte()
        {
            return CapturedImageByte;
        }
    }
}
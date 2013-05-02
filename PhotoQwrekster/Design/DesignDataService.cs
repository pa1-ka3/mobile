using System;
using PhotoQwrekster.Model;

namespace PhotoQwrekster.Design
{
    public class DesignDataService : IDataService
    {
        public void GetData(Action<DataItem, Exception> callback)
        {
            // Use this to create design time data

            var item = new DataItem("Welcome to MVVM Light [design]");
            callback(item, null);
        }


        public void SetCapturedImageStream(System.IO.Stream capturedStream)
        {
            throw new NotImplementedException();
        }

        public void SetCupturedImageByte(byte[] dest1)
        {
            throw new NotImplementedException();
        }

        public System.IO.Stream GetCapturedStream()
        {
            throw new NotImplementedException();
        }


        public byte[] GetCapturedImageByte()
        {
            throw new NotImplementedException();
        }
    }
}
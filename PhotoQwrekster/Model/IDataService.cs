using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using System.Collections.ObjectModel;
using Microsoft.Phone.Controls;

namespace PhotoQwrekster.Model
{
    public interface IDataService
    {
        void GetData(Action<DataItem, Exception> callback);

        void SetCapturedImageStream(System.IO.Stream capturedStream);

        void SetCupturedImageByte(byte[] dest1);

        System.IO.Stream GetCapturedStream();

        byte[] GetCapturedImageByte();

        void InitPropertiesFromStorage();

        System.Windows.Size GetResolutionSetting();

        void SaveResolutionSettings(System.Windows.Size res);        

        void SaveFlashModeSettings(Microsoft.Devices.FlashMode flashMode);

        Microsoft.Devices.FlashMode GetFlashModeSettings();

        bool GetTransformModeSettings();

        void SaveTransformModeSettings(bool transformMode);

        WriteableBitmap GetBitmapImageSource();

        void SetBitmapImageSource(WriteableBitmap bitmap);

        void SetImageName(string imageName);

        string GetImageName();

        ObservableCollection<ActionDescriptionModel> GetActionsList();

        void MakeScreenshot(PhoneApplicationPage page);
    }
}

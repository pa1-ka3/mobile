using System;
using System.IO;
using System.Windows;
using Microsoft.Devices;

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


        private Size _cameraResolution;

        public Size CameraResolution
        {
            get { return _cameraResolution; }
            set
            {
                _cameraResolution = value;
                var settings = ((App)App.Current).SettingsData.Settings;
                settings["CameraResolution"] = value;
            }
        }

        public Size GetResolutionSetting()
        {
            return CameraResolution;
        }

        public void SaveResolutionSettings(System.Windows.Size res)
        {
            CameraResolution = res;
        }

        public void InitPropertiesFromStorage()
        {            
            var settings = ((App)App.Current).SettingsData.Settings;
            _cameraResolution = settings.Contains("CameraResolution") ? (Size)(settings["CameraResolution"]) : new Size(0, 0);
            _cameraFlashMode = settings.Contains("CameraFlashMode") ? (FlashMode)(settings["CameraFlashMode"]) : FlashMode.Auto;
            _transformationMode = settings.Contains("TransformationMode") ? (bool)(settings["TransformationMode"]) : false; 
        }


        private FlashMode _cameraFlashMode;

        public FlashMode CameraFlashMode
        {
            get { return _cameraFlashMode; }
            set
            {
                _cameraFlashMode = value;
                var settings = ((App)App.Current).SettingsData.Settings;
                settings["CameraFlashMode"] = value;
            }
        }


        public void SaveFlashModeSettings(FlashMode flashMode)
        {
            CameraFlashMode = flashMode;
        }


        public FlashMode GetFlashModeSettings()
        {
            return CameraFlashMode;
        }


        private bool _transformationMode;

        public bool TransformationMode
        {
            get { return _transformationMode; }
            set
            {
                _transformationMode = value;
                var settings = ((App)App.Current).SettingsData.Settings;
                settings["TransformationMode"] = value;
            }
        }
        


        public bool GetTransformModeSettings()
        {
            return TransformationMode;
        }

        public void SaveTransformModeSettings(bool transformMode)
        {
            TransformationMode = transformMode;
        }
    }
}
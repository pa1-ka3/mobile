using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;

// Directives
using Microsoft.Devices;
using System.IO;
using System.IO.IsolatedStorage;
using Microsoft.Xna.Framework.Media;
using System.Windows.Media.Imaging;
using PhotoQwrekster.ViewModel;


using GalaSoft.MvvmLight;
using PhotoQwrekster.Model;
using GalaSoft.MvvmLight.Command;



namespace PhotoQwrekster.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm/getstarted
    /// </para>
    /// </summary>
    public class CameraViewModel : ViewModelBase
    {

        private readonly IDataService _dataService;

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public CameraViewModel(IDataService dataService)
        {
            _dataService = dataService;
            _dataService.GetData(
                (item, error) =>
                {
                    if (error != null)
                    {
                        // Report error here
                        return;
                    }
                                        
                });

            
        }            

        #region ButtonsCommands


        private RelayCommand _changeFlashClicked;

        /// <summary>
        /// Gets the ChangeFlash_Clicked.
        /// </summary>
        public RelayCommand ChangeFlashClick
        {
            get
            {
                return _changeFlashClicked
                    ?? (_changeFlashClicked = new RelayCommand(
                                          () =>
                                          {
                                              changeFlash_Clicked();
                                          }));
            }
        }


        private RelayCommand _changeResClicked;

        /// <summary>
        /// Gets the ChangeResClick.
        /// </summary>
        public RelayCommand ChangeResClick
        {
            get
            {
                return _changeResClicked
                    ?? (_changeResClicked = new RelayCommand(
                                          () =>
                                          {
                                              changeRes_Clicked();
                                          }));
            }
        }


        public void skip_clicked()
        {
            INavigationService navigation = App.Container.Resolve<INavigationService>();
            Runner.Invoke(() =>
            {
                navigation.NavigateTo(new Uri(ViewModelLocator.ImageProcessing, UriKind.RelativeOrAbsolute));
            });
        }

        #endregion
        
        #region Labels


        /// <summary>
        /// The <see cref="TextDebug" /> property's name.
        /// </summary>
        public const string TextDebugPropertyName = "TextDebug";

        private string _textDebug = string.Empty;

        public string TextDebug
        {
            get { return _textDebug; }
            set
            {

                if (_textDebug == value)
                {
                    return;
                }

                _textDebug = value;
                
                Runner.Invoke(() =>
                {
                    RaisePropertyChanged(TextDebugPropertyName);
                });
                
            }
        }

        /// <summary>
        /// The <see cref="ResolutionLabel" /> property's name.
        /// </summary>
        public const string ResolutionLabelPropertyName = "ResolutionLabel";

        private string _resolutionLabel = string.Empty;

        /// <summary>
        /// Sets and gets the ResolutionLabel property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string ResolutionLabel
        {
            get
            {
                return _resolutionLabel;
            }

            set
            {
                if (_resolutionLabel == value)
                {
                    return;
                }

                _resolutionLabel = value;
                Runner.Invoke(() =>
                {
                    RaisePropertyChanged(ResolutionLabelPropertyName);
                });
            }
        }


        /// <summary>
        /// The <see cref="FlashButtonContent" /> property's name.
        /// </summary>
        public const string FlashButtonContentPropertyName = "FlashButtonContent";

        private string _flashButtonContent = string.Empty;

        /// <summary>
        /// Sets and gets the FlashButtonContent property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string FlashButtonContent
        {
            get
            {
                return _flashButtonContent;
            }

            set
            {
                if (_flashButtonContent == value)
                {
                    return;
                }

                _flashButtonContent = value;
                Runner.Invoke(() =>
                {
                    RaisePropertyChanged(FlashButtonContentPropertyName);
                });
            }
        }

        /// <summary>
        /// The <see cref="ResButtonContent" /> property's name.
        /// </summary>
        public const string ResButtonContentPropertyName = "ResButtonContent";

        private string _resButtonContent = string.Empty;

        /// <summary>
        /// Sets and gets the ResButtonContent property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string ResButtonContent
        {
            get
            {
                return _resButtonContent;
            }

            set
            {
                if (_resButtonContent == value)
                {
                    return;
                }

                _resButtonContent = value;
                Runner.Invoke(() =>
                {
                    RaisePropertyChanged(ResButtonContentPropertyName);
                });
            }
        }

        #endregion

        #region FocusBrackets
        /// <summary>
        /// The <see cref="FocusBracketsCanvasLeft" /> property's name.
        /// </summary>
        public const string FocusBracketsCanvasLeftPropertyName = "FocusBracketsCanvasLeft";

        private double _focusBracketsCanvasLeft = 0;

        /// <summary>
        /// Sets and gets the FocusBracketsCanvasLeft property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double FocusBracketsCanvasLeft
        {
            get
            {
                return _focusBracketsCanvasLeft;
            }

            set
            {
                if (_focusBracketsCanvasLeft == value)
                {
                    return;
                }

                _focusBracketsCanvasLeft = value;
                Runner.Invoke(() =>
                {
                    RaisePropertyChanged(FocusBracketsCanvasLeftPropertyName);
                });
            }
        }

        /// <summary>
        /// The <see cref="FocusBracketsCanvasTop" /> property's name.
        /// </summary>
        public const string FocusBracketsCanvasRightPropertyName = "FocusBracketsCanvasTop";

        private double _focusBracketsCanvasTop = 0;

        /// <summary>
        /// Sets and gets the FocusBracketsCanvasRight property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double FocusBracketsCanvasTop
        {
            get
            {
                return _focusBracketsCanvasTop;
            }

            set
            {
                if (_focusBracketsCanvasTop == value)
                {
                    return;
                }

                _focusBracketsCanvasTop = value;
                Runner.Invoke(() =>
                {
                    RaisePropertyChanged(FocusBracketsCanvasRightPropertyName);
                });
            }
        }
                /// <summary>
        /// The <see cref="FocusBracketsVisibility" /> property's name.
        /// </summary>
        public const string FocusBracketsVisibilityPropertyName = "FocusBracketsVisibility";

        private Visibility _focusBracketsVisibility = Visibility.Collapsed;

        /// <summary>
        /// Sets and gets the FocusBracketsVisibility property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public Visibility FocusBracketsVisibility
        {
            get
            {
                return _focusBracketsVisibility;
            }

            set
            {
                if (_focusBracketsVisibility == value)
                {
                    return;
                }

                _focusBracketsVisibility = value;
                Runner.Invoke(() =>
                {
                    RaisePropertyChanged(FocusBracketsVisibilityPropertyName);
                });
            }
        }
        #endregion

     
        MediaLibrary library = new MediaLibrary();

        // Holds the current resolution index.
        int currentResIndex = 0;

        PhotoCamera cam = null;

        internal PhotoCamera Initialize()
        {
             // Check to see if the camera is available on the device.
            if ((PhotoCamera.IsCameraTypeSupported(CameraType.Primary) == true) ||
                 (PhotoCamera.IsCameraTypeSupported(CameraType.FrontFacing) == true))
            {
                // Initialize the camera, when available.
                if (PhotoCamera.IsCameraTypeSupported(CameraType.FrontFacing))
                {
                    // Use front-facing camera if available.
                    cam = new Microsoft.Devices.PhotoCamera(CameraType.FrontFacing);
                }
                else
                {
                    // Otherwise, use standard camera on back of device.
                    cam = new Microsoft.Devices.PhotoCamera(CameraType.Primary);
                }

                // Event is fired when the PhotoCamera object has been initialized.
                cam.Initialized += new EventHandler<Microsoft.Devices.CameraOperationCompletedEventArgs>(cam_Initialized);

                // Event is fired when the capture sequence is complete.
                cam.CaptureCompleted += new EventHandler<CameraOperationCompletedEventArgs>(cam_CaptureCompleted);

                // Event is fired when the capture sequence is complete and an image is available.
                cam.CaptureImageAvailable += new EventHandler<Microsoft.Devices.ContentReadyEventArgs>(cam_CaptureImageAvailable);

                // Event is fired when the capture sequence is complete and a thumbnail image is available.
                //cam.CaptureThumbnailAvailable += new EventHandler<ContentReadyEventArgs>(cam_CaptureThumbnailAvailable);

                     // The event is fired when auto-focus is complete.
                cam.AutoFocusCompleted += new EventHandler<CameraOperationCompletedEventArgs>(cam_AutoFocusCompleted);

                

                // The event is fired when the shutter button receives a half press.
                CameraButtons.ShutterKeyHalfPressed += OnButtonHalfPress;

                // The event is fired when the shutter button receives a full press.
                CameraButtons.ShutterKeyPressed += OnButtonFullPress;

                // The event is fired when the shutter button is released.
                CameraButtons.ShutterKeyReleased += OnButtonRelease;                

                return cam;

            }
            else
            {
                // The camera is not supported on the device.
               // this.Dispatcher.BeginInvoke(delegate()
               // {
                    // Write message.
                    TextDebug = "A Camera is not available on this device.";
                //});

                    return null;

                
            }
        }

        internal void ReleaseResources()
        {
            if (cam != null)
            {
                // Dispose camera to minimize power consumption and to expedite shutdown.
                cam.Dispose();

                // Release memory, ensure garbage collection.
                cam.Initialized -= cam_Initialized;
                cam.CaptureCompleted -= cam_CaptureCompleted;
                cam.CaptureImageAvailable -= cam_CaptureImageAvailable;
               // cam.CaptureThumbnailAvailable -= cam_CaptureThumbnailAvailable;
                cam.AutoFocusCompleted -= cam_AutoFocusCompleted;
                CameraButtons.ShutterKeyHalfPressed -= OnButtonHalfPress;
                CameraButtons.ShutterKeyPressed -= OnButtonFullPress;
                CameraButtons.ShutterKeyReleased -= OnButtonRelease;
            }
        }

        // Update the UI if initialization succeeds.
        void cam_Initialized(object sender, Microsoft.Devices.CameraOperationCompletedEventArgs e)
        {
            if (e.Succeeded)
            {
                _dataService.InitPropertiesFromStorage();

                SetCameraResolutionFromSettings();

                SetCameraFlashModeFromSettings();

                // Write message.
                TextDebug = "Camera initialized.";

                
            }
        }
        

        private void SetCameraResolutionFromSettings()
        {
            Size res = _dataService.GetResolutionSetting();
            IEnumerable<Size> resList = cam.AvailableResolutions;
            int resCount = resList.Count<Size>();

            if (resList.Contains<Size>(res))
            {
                for (int i = 0; i < resCount; i++)
                {
                    if (res == resList.ElementAt<Size>(i))
                    {
                        currentResIndex = i;
                        break;
                    }
                }
            }
            else
            {
                res = resList.ElementAt<Size>(0);
                currentResIndex = 0;
            }

            cam.Resolution = res;
            ResButtonContent = string.Format("{0}x{1}", res.Width, res.Height);

        }

        private void SetCameraFlashModeFromSettings()
        {
            // Set flash button image.
            cam.FlashMode = _dataService.GetFlashModeSettings();

            switch (cam.FlashMode)
            {
                case FlashMode.On:
                    FlashButtonContent = "Flash.png";
                    break;
                case FlashMode.Auto:
                    FlashButtonContent = "FlashA.png";
                    break;
                case FlashMode.Off:
                    FlashButtonContent = "NoFlash.png";
                    break;
                case FlashMode.RedEyeReduction:
                    FlashButtonContent = "NoFlash.png";
                    break;
            }
        }


        public bool IsCameraPresent()
        {
            if (cam != null) 
                return true;
            else
                return false;
        }

      

        public void ShutterButton_Click()
        {
            if (cam != null)
            {
                try
                {
                    // Start image capture.
                    cam.CaptureImage();
                }
                catch (Exception ex)
                {
                    // Cannot capture an image until the previous capture has completed.
                    TextDebug = ex.Message;
                }
            }
        }

        void cam_CaptureCompleted(object sender, CameraOperationCompletedEventArgs e)
        {
            INavigationService navigation = App.Container.Resolve<INavigationService>();
             Runner.Invoke(() =>
             {
                 navigation.NavigateTo(new Uri(ViewModelLocator.ImageProcessing, UriKind.RelativeOrAbsolute));
             });
        }

        // Informs when full resolution picture has been taken, saves to local media library and isolated storage.
        void cam_CaptureImageAvailable(object sender, Microsoft.Devices.ContentReadyEventArgs e)
        {
            string fileName = "PQ-"+DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".jpg";

            try
            {
                TextDebug = "Captured image available, saving picture.";

                // Save picture to the library camera roll.
                library.SavePictureToCameraRoll(fileName, e.ImageStream);

                TextDebug = "Picture has been saved to camera roll.";



                // Set the position of the stream back to start
                    e.ImageStream.Seek(0, SeekOrigin.Begin);
                    byte[] dest1 = new byte[e.ImageStream.Length];
                    e.ImageStream.Read(dest1, 0, (int)e.ImageStream.Length);
                    _dataService.SetCupturedImageByte(dest1);
               
            }
            finally
            {
                // Close image stream                
                 e.ImageStream.Close();

            }

        }

       

        private Stream capturedStream;

        public Stream CapturedStream
        {
            get { return capturedStream; }
            set { capturedStream = value; }
        }

        // Informs when thumbnail picture has been taken, saves to isolated storage
        // User will select this image in the pictures application to bring up the full-resolution picture. 
  /*      public void cam_CaptureThumbnailAvailable(object sender, ContentReadyEventArgs e)
        {
            string fileName = savedCounter + "_th.jpg";

            try
            {
                // Write message to UI thread.
                TextDebug = "Captured image available, saving thumbnail.";
                
                // Save thumbnail as JPEG to isolated storage.
                using (IsolatedStorageFile isStore = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    using (IsolatedStorageFileStream targetStream = isStore.OpenFile(fileName, FileMode.Create, FileAccess.Write))
                    {
                        // Initialize the buffer for 4KB disk pages.
                        byte[] readBuffer = new byte[4096];
                        int bytesRead = -1;

                        // Copy the thumbnail to isolated storage. 
                        while ((bytesRead = e.ImageStream.Read(readBuffer, 0, readBuffer.Length)) > 0)
                        {
                            targetStream.Write(readBuffer, 0, bytesRead);
                        }
                    }
                }

                // Write message to UI thread.
                TextDebug = "Thumbnail has been saved to isolated storage.";

            }
            finally
            {
                // Close image stream
                e.ImageStream.Close();
            }
        }
*/
        // Activate a flash mode.
        // Cycle through flash mode options when the flash button is pressed.
        private void changeFlash_Clicked()
        {
            // Holds the current flash mode.
            string currentFlashMode = string.Empty;


            switch (cam.FlashMode)
            {
                case FlashMode.Off:
                    if (cam.IsFlashModeSupported(FlashMode.On))
                    {
                        // Specify that flash should be used.
                        cam.FlashMode = FlashMode.On;
                        FlashButtonContent ="Flash.png";
                        currentFlashMode = "Flash mode: On";
                    }
                    break;
                case FlashMode.On:
                    if (cam.IsFlashModeSupported(FlashMode.RedEyeReduction))
                    {
                        // Specify that the red-eye reduction flash should be used.
                        cam.FlashMode = FlashMode.RedEyeReduction;
                        FlashButtonContent = "Fl:RER";
                        currentFlashMode = "Flash mode: RedEyeReduction";
                    }
                    else if (cam.IsFlashModeSupported(FlashMode.Auto))
                    {
                        // If red-eye reduction is not supported, specify automatic mode.
                        cam.FlashMode = FlashMode.Auto;
                        FlashButtonContent = "FlashA.png";
                        currentFlashMode = "Flash mode: Auto";
                    }
                    else
                    {
                        // If automatic is not supported, specify that no flash should be used.
                        cam.FlashMode = FlashMode.Off;
                        FlashButtonContent = "NoFlash.png";
                        currentFlashMode = "Flash mode: Off";
                    }
                    break;
                case FlashMode.RedEyeReduction:
                    if (cam.IsFlashModeSupported(FlashMode.Auto))
                    {
                        // Specify that the flash should be used in the automatic mode.
                        cam.FlashMode = FlashMode.Auto;
                        FlashButtonContent = "FlashA.png";
                        currentFlashMode = "Flash mode: Auto";
                    }
                    else
                    {
                        // If automatic is not supported, specify that no flash should be used.
                        cam.FlashMode = FlashMode.Off;
                        FlashButtonContent = "NoFlash.png";
                        currentFlashMode = "Flash mode: Off";
                    }
                    break;
                case FlashMode.Auto:
                    if (cam.IsFlashModeSupported(FlashMode.Off))
                    {
                        // Specify that no flash should be used.
                        cam.FlashMode = FlashMode.Off;
                        FlashButtonContent = "NoFlash.png";
                        currentFlashMode = "Flash mode: Off";
                    }
                    break;
            }

            _dataService.SaveFlashModeSettings(cam.FlashMode);

            // Display current flash mode.            
            TextDebug = currentFlashMode;
        }

        // Provide auto-focus in the viewfinder.
        public void focus_Clicked()
        {
            if (cam.IsFocusSupported == true)
            {
                //Focus when a capture is not in progress.
                try
                {
                    cam.Focus();
                }
                catch (Exception focusError)
                {
                    // Cannot focus when a capture is in progress.
                    TextDebug = focusError.Message;

                }
            }
            else
            {
                // Write message to UI.
                TextDebug = "Camera does not support programmable auto focus.";
            }
        }

        void cam_AutoFocusCompleted(object sender, CameraOperationCompletedEventArgs e)
        {
            
                // Write message to UI.
                TextDebug = "Auto focus has completed.";

                // Hide the focus brackets.
                FocusBracketsVisibility = Visibility.Collapsed;

            
        }

        

        private void changeRes_Clicked()
        {
            // Variables
            IEnumerable<Size> resList = cam.AvailableResolutions;
            int resCount = resList.Count<Size>();
            Size res;

            // Poll for available camera resolutions.
            for (int i = 0; i < resCount; i++)
            {
                res = resList.ElementAt<Size>(i);
            }

            // Set the camera resolution.
            res = resList.ElementAt<Size>((currentResIndex + 1) % resCount);
            cam.Resolution = res;
            currentResIndex = (currentResIndex + 1) % resCount;

            // Update the UI.
            TextDebug = String.Format("Setting capture resolution: {0}x{1}", res.Width, res.Height);
            ResButtonContent = res.Width + "x" + res.Height;
            _dataService.SaveResolutionSettings(res);
        }

        // Provide auto-focus with a half button press using the hardware shutter button.
        private void OnButtonHalfPress(object sender, EventArgs e)
        {
            if (cam != null)
            {
                // Focus when a capture is not in progress.
                try
                {
                    TextDebug = "Half Button Press: Auto Focus";
                    cam.Focus();
                }
                catch (Exception focusError)
                {
                    // Cannot focus when a capture is in progress.
                    TextDebug = focusError.Message;
                }
            }
        }

        // Capture the image with a full button press using the hardware shutter button.
        private void OnButtonFullPress(object sender, EventArgs e)
        {
            if (cam != null)
            {
                cam.CaptureImage();
            }
        }

        // Cancel the focus if the half button press is released using the hardware shutter button.
        private void OnButtonRelease(object sender, EventArgs e)
        {

            if (cam != null)
            {
                cam.CancelFocus();
            }
        }




        internal void focusTapped(Point tapLocation, double viewfinderCanvasWidth, double viewfinderCanvasHeight)
        {
            // Provide touch focus in the viewfinder.

            if (cam != null)
            {
                if (cam.IsFocusAtPointSupported == true)
                {
                    try
                    {
                        // Determine location of tap.
                        //Point tapLocation = e.GetPosition(viewfinderCanvas);

                        // Position focus brackets with estimated offsets.
                        //focusBrackets.SetValue(Canvas.LeftProperty, tapLocation.X - 30);
                        FocusBracketsCanvasLeft = tapLocation.X - 30;
                        FocusBracketsCanvasTop = tapLocation.Y - 28;
                        //focusBrackets.SetValue(Canvas.TopProperty, tapLocation.Y - 28);

                        // Determine focus point.
                        double focusXPercentage = tapLocation.X / viewfinderCanvasWidth;
                        double focusYPercentage = tapLocation.Y / viewfinderCanvasHeight;

                        // Show focus brackets and focus at point
                        FocusBracketsVisibility = Visibility.Visible;
                        cam.FocusAtPoint(focusXPercentage, focusYPercentage);

                        // Write a message to the UI.
                        TextDebug = String.Format("Camera focusing at point: {0:N2} , {1:N2}", focusXPercentage, focusYPercentage);
                    }
                    catch (Exception focusError)
                    {
                        // Cannot focus when a capture is in progress.

                        // Write a message to the UI.
                        TextDebug = focusError.Message;
                        // Hide focus brackets.
                        FocusBracketsVisibility = Visibility.Collapsed;

                    }
                }
                else
                {
                    // Write a message to the UI.                    
                    TextDebug = "Camera does not support FocusAtPoint().";
                }
            }
        }

        private RelayCommand _saveSize;
        /// <summary>
        /// Gets the SaveSize.
        /// </summary>
        public RelayCommand SaveSize
        {
            get
            {
                return _saveSize
                    ?? (_saveSize = new RelayCommand(
                                          () =>
                                          {
                                              Size ns = new Size(123, 456);
                                              _dataService.SaveFlashModeSettings(FlashMode.On);
                                              _dataService.SaveResolutionSettings(ns);
                                              
                                          }));
            }
        }


        private RelayCommand _loadSize;

        /// <summary>
        /// Gets the LoadSize.
        /// </summary>
        public RelayCommand LoadSize
        {
            get
            {
                return _loadSize
                    ?? (_loadSize = new RelayCommand(
                                          () =>
                                          {
                                              _dataService.InitPropertiesFromStorage();
                                              FlashMode fm = _dataService.GetFlashModeSettings();
                                              Size s = _dataService.GetResolutionSetting();
                                              int i = 8;
                                              
                                          }));
            }
        }
    }

}
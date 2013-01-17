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
using Microsoft.Devices;
using System.Reflection;
using Microsoft.Phone.Info;
using Windows.Phone.Media.Capture;
using System.Windows.Navigation;

namespace one_click_video
{
    public partial class MainPage : PhoneApplicationPage
    {
        //private Windows.Foundation.Size _max_s;
        //private int[] _arr;
        //private WriteableBitmap _oB;


        // Konstruktor
        public MainPage()
        {
            InitializeComponent();

            // Beispielcode zur Lokalisierung der ApplicationBar
            //BuildLocalizedApplicationBar();

            OrientationChanged += MainPage_OrientationChanged;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            ActivateCamera();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (_dev != null)
            {
                _dev.Dispose();
                _dev = null;
            }

            base.OnNavigatedFrom(e);
        }

        private void MainPage_OrientationChanged(object sender, OrientationChangedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(this.Orientation);
            if (this.Orientation == PageOrientation.LandscapeLeft)
            {
                _videoBrush.RelativeTransform = new RotateTransform() { CenterX = 0.5, CenterY = 0.5, Angle = 90 - _dev.SensorRotationInDegrees };
            }
            else if (this.Orientation == PageOrientation.LandscapeRight)
            {
                _videoBrush.RelativeTransform = new RotateTransform() { CenterX = 0.5, CenterY = 0.5, Angle = 90 + _dev.SensorRotationInDegrees };
            }
            else if(this.Orientation == PageOrientation.PortraitUp)
            {
                _videoBrush.RelativeTransform = new RotateTransform() { CenterX = 0.5, CenterY = 0.5, Angle = _dev.SensorRotationInDegrees };
            }
        }

        private AudioVideoCaptureDevice _dev;
        private VideoBrush _videoBrush;

        async void ActivateCamera()
        {
            if (_dev == null)
            {
                if (AudioVideoCaptureDevice.AvailableSensorLocations.Contains(CameraSensorLocation.Back))
                {
                    _dev = await AudioVideoCaptureDevice.OpenAsync(CameraSensorLocation.Back, AudioVideoCaptureDevice.GetAvailableCaptureResolutions(CameraSensorLocation.Back).Last());

                    _videoBrush = new VideoBrush();

                    _videoBrush.SetSource(_dev);
                    MainPage_OrientationChanged(null, null);

                    this.videoRect.Fill = _videoBrush;
                }
                

                //CameraStreamSource source = new CameraStreamSource(_dev, _dev.PreviewResolution);
                //MyCameraMediaElement.SetSource(_source);
                
            }
            

            //_dev.SetProperty(KnownCameraGeneralProperties.SpecifiedCaptureOrientation, 90);
            //_dev.SetProperty(KnownCameraGeneralProperties.EncodeWithOrientation, 90);

            

            //IReadOnlyList<Windows.Foundation.Size>  cap_res = AudioVideoCaptureDevice.GetAvailableCaptureResolutions(CameraSensorLocation.Back);
            //_max_s = new Windows.Foundation.Size(0, 0);
            
            //foreach (Windows.Foundation.Size s in cap_res)
            //{
            //    if (s.Height * s.Width > _max_s.Height * _max_s.Width)
            //    {
            //        _max_s = s;
            //    }
            //}

            //_arr = new int[(int)(_max_s.Height * _max_s.Width)];
            //_oB = new WriteableBitmap(int)_max_s.Width, (int)_max_s.Height);

            //IAsyncOperation<AudioVideoCaptureDevice> dev_async_op = AudioVideoCaptureDevice.OpenAsync(CameraSensorLocation.Back, _max_s);
            //dev_async_op.Completed = (IAsyncOperation<AudioVideoCaptureDevice> dev, Windows.Foundation.AsyncStatus status) =>
            //    {
            //        _dev = dev.GetResults();
            //        _dev.PreviewFrameAvailable += _dev_PreviewFrameAvailable;
            //    };

            

                //OpenAsync(CameraSensorLocation.Back, new Windows.Foundation.Size(10,10));

        }

        //private void _dev_PreviewFrameAvailable(ICameraCaptureDevice sender, object args)
        //{
        //    sender.GetPreviewBufferArgb(_arr);

        //    this.Dispatcher.BeginInvoke(() =>
        //    {
        //        for (int i = 0; i < _arr.Length; ++i)
        //        {
        //            _oB.Pixels[i] = _arr[i];
        //        }

        //        this.img.Source = _oB;
        //    }
        //    );
           

        //    //sender.GetPreviewBufferArgb
        //}


        // Beispielcode zur Erstellung einer lokalisierten ApplicationBar
        //private void BuildLocalizedApplicationBar()
        //{
        //    // ApplicationBar der Seite einer neuen Instanz von ApplicationBar zuweisen
        //    ApplicationBar = new ApplicationBar();

        //    // Eine neue Schaltfläche erstellen und als Text die lokalisierte Zeichenfolge aus AppResources zuweisen.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Ein neues Menüelement mit der lokalisierten Zeichenfolge aus AppResources erstellen
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}
    }
}
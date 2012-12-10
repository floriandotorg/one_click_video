using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Devices;
using Windows.Phone.Media.Capture;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Threading;
using Microsoft.Phone;

namespace one_click_video
{
    public partial class MainPage : PhoneApplicationPage
    {
        private AudioVideoCaptureDevice _dev;
        //private Windows.Foundation.Size _max_s;
        //private int[] _arr;
        //private WriteableBitmap _oB;


        // Konstruktor
        public MainPage()
        {
            InitializeComponent();

            // Beispielcode zur Lokalisierung der ApplicationBar
            //BuildLocalizedApplicationBar();

            Loaded += MainPage_Loaded;
        }

        async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            Windows.Foundation.Size resolution = new Windows.Foundation.Size(640, 480);
            _dev = await AudioVideoCaptureDevice.OpenAsync(CameraSensorLocation.Back, resolution);

            //_dev.SetProperty(KnownCameraGeneralProperties.SpecifiedCaptureOrientation, 90);
            //_dev.SetProperty(KnownCameraGeneralProperties.EncodeWithOrientation, 90);

            _dev.SetProperty(KnownCameraAudioVideoProperties.VideoTorchPower, AudioVideoCaptureDevice.GetSupportedPropertyRange(CameraSensorLocation.Back, KnownCameraAudioVideoProperties.VideoTorchPower).Min);
            _dev.SetProperty(KnownCameraAudioVideoProperties.VideoTorchMode, VideoTorchMode.On);

            Windows.Foundation.Size actualResolution = _dev.PreviewResolution;

            CameraStreamSource source = new CameraStreamSource(_dev, actualResolution);
            MyCameraMediaElement.SetSource(source);

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
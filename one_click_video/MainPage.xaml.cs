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
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.Foundation;
using Microsoft.Phone.Tasks;

namespace one_click_video
{
    public partial class MainPage : PhoneApplicationPage
    {
        private AudioVideoCaptureDevice _dev;
        private VideoBrush _videoBrush;
        private IRandomAccessStream _sst;
        private string _path;

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

            //CameraCaptureTask cameraCaptureTask = new CameraCaptureTask();
            //cameraCaptureTask.Show();

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

        async void ActivateCamera()
        {
            if (_dev == null)
            {
                if (AudioVideoCaptureDevice.AvailableSensorLocations.Contains(CameraSensorLocation.Back))
                {
                    _dev = await AudioVideoCaptureDevice.OpenAsync(CameraSensorLocation.Back, AudioVideoCaptureDevice.GetAvailableCaptureResolutions(CameraSensorLocation.Back).First());

                    _videoBrush = new VideoBrush();

                    _videoBrush.SetSource(_dev);
                    MainPage_OrientationChanged(null, null);

                    this.videoRect.Fill = _videoBrush;

                    StorageFolder localFolder = ApplicationData.Current.LocalFolder;
                    StorageFile storageFile = await localFolder.CreateFileAsync("CameraMovie.mp4", CreationCollisionOption.ReplaceExisting);
                    _path = storageFile.Path;

                    _sst = await storageFile.OpenAsync(FileAccessMode.ReadWrite);
                    await _dev.StartRecordingToStreamAsync(_sst);
                }
            }
        }

        private void Button_Click_1(object sender, System.Windows.RoutedEventArgs e)
        {
        	if (_dev != null)
            {
                _dev.StopRecordingAsync().Completed = (IAsyncAction action, Windows.Foundation.AsyncStatus status) =>
                    {
                        _dev.Dispose();
                        _dev = null;


                        new MediaPlayerLauncher()
                        {
                            Media = new Uri(_path, UriKind.Relative),
                        }.Show();
                    };
            }
        }

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
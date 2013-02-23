using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Devices;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.Phone.Media.Capture;
using Windows.Foundation;
using WP7Contrib.View.Transitions.Animation;

namespace one_click_video
{
    public partial class RecordingPage : AnimatedBasePage
    {
        private AudioVideoCaptureDevice _dev;
        private VideoBrush _videoBrush;
        private IRandomAccessStream _sst;
        private System.Windows.Threading.DispatcherTimer _dt;
        private DateTime _recStartTime;

        public RecordingPage()
        {
            InitializeComponent();

            AnimationContext = LayoutRoot;

            OrientationChanged += RecordingPage_OrientationChanged;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            _dt = new System.Windows.Threading.DispatcherTimer();
            _dt.Interval = new TimeSpan(0, 0, 0, 1, 0);
            _dt.Tick += dt_Tick;

            ActivateCamera();
        }

        void dt_Tick(object sender, EventArgs e)
        {
            TimeSpan duration = DateTime.Now - _recStartTime + TimeSpan.FromSeconds(1);
            this.RecTimer.Text = duration.ToString(@"mm\:ss");
        }

        void CameraButtons_ShutterKeyPressed(object sender, EventArgs e)
        {
            _dt.Stop();

            StopCamera();
        }

        async void StopCamera()
        {
            await _dev.StopRecordingAsync();
            _dev.Dispose();
            _dev = null;

            await _sst.FlushAsync();
            _sst.Dispose();
            _sst = null;

            NavigationService.Navigate(new Uri("/PlayPage.xaml", UriKind.Relative));
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.timer.Tap -= CameraButtons_ShutterKeyPressed;
            CameraButtons.ShutterKeyPressed -= CameraButtons_ShutterKeyPressed;
            
            if (_dev != null)
            {
                _dev.Dispose();
                _dev = null;
            }

            base.OnNavigatedFrom(e);
        }

        protected override void OnOrientationChanged(OrientationChangedEventArgs e)
        {
            if (e.Orientation == PageOrientation.LandscapeLeft)
            {
                base.OnOrientationChanged(e);
            }
        }

        private void RecordingPage_OrientationChanged(object sender, OrientationChangedEventArgs e)
        {
            if (_videoBrush != null)
            {
                if (this.Orientation == PageOrientation.LandscapeLeft)
                {
                    _videoBrush.RelativeTransform = new RotateTransform() { CenterX = 0.5, CenterY = 0.5, Angle = 90 - _dev.SensorRotationInDegrees };
                }
                else if (this.Orientation == PageOrientation.LandscapeRight)
                {
                    _videoBrush.RelativeTransform = new RotateTransform() { CenterX = 0.5, CenterY = 0.5, Angle = 90 + _dev.SensorRotationInDegrees };
                }
                else if (this.Orientation == PageOrientation.PortraitUp)
                {
                    _videoBrush.RelativeTransform = new RotateTransform() { CenterX = 0.5, CenterY = 0.5, Angle = _dev.SensorRotationInDegrees };
                }
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
                    RecordingPage_OrientationChanged(null, null);

                    this.videoRect.Fill = _videoBrush;

                    StorageFolder localFolder = ApplicationData.Current.LocalFolder;
                    StorageFile storageFile = await localFolder.CreateFileAsync("CameraMovie.mp4", CreationCollisionOption.ReplaceExisting);

                    _sst = await storageFile.OpenAsync(FileAccessMode.ReadWrite);
                    await _dev.StartRecordingToStreamAsync(_sst);

                    ////////////////////////////////////////////////////////////////////
                    this.RecIconStarting.Visibility = Visibility.Collapsed;
                    _recStartTime = DateTime.Now;
                    dt_Tick(null, null);
                    _dt.Start();

                    this.timer.Tap += CameraButtons_ShutterKeyPressed;
                    CameraButtons.ShutterKeyPressed += CameraButtons_ShutterKeyPressed;
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
                };
            }
        }

        protected override AnimatorHelperBase GetAnimation(AnimationType animationType, Uri toOrFrom)
        {
            if (animationType == AnimationType.NavigateForwardOut)
            {
                return new SlideDownAnimator { RootElement = LayoutRoot };
            }

            if (animationType == AnimationType.NavigateBackwardOut)
            {
                return new SlideDownAnimator { RootElement = LayoutRoot };
            }

            if (animationType == AnimationType.NavigateForwardIn)
            {
                return new SlideUpAnimator { RootElement = LayoutRoot };
            }

            return new SlideUpAnimator { RootElement = this.LayoutRoot };
        }
    }
}
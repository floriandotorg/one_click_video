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
        private string _path;

        public RecordingPage()
        {
            InitializeComponent();

            AnimationContext = LayoutRoot;

            OrientationChanged += RecordingPage_OrientationChanged;
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

        private void RecordingPage_OrientationChanged(object sender, OrientationChangedEventArgs e)
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
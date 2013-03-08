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
using System.Windows.Media.Imaging;
using System.IO.IsolatedStorage;
using System.IO;

namespace one_click_video
{
    public partial class RecordingPage : AnimatedBasePage
    {
        private AudioVideoCaptureDevice _dev;
        private VideoBrush _videoBrush;
        private IRandomAccessStream _sst;
        private System.Windows.Threading.DispatcherTimer _dt;
        private DateTime _recStartTime;
        private string lastVideo;

        public RecordingPage()
        {
            InitializeComponent();

            AnimationContext = LayoutRoot;
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
            StopCamera(true);
        }

        private void StopCamera(bool play)
        {
            _dt.Stop();

            using (var storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (StreamWriter writeFile = new StreamWriter(new IsolatedStorageFileStream(lastVideo + ".metadata", FileMode.Create, FileAccess.Write, storage)))
                {
                    TimeSpan duration = DateTime.Now - _recStartTime + TimeSpan.FromSeconds(1);
                    writeFile.WriteLine(duration.TotalSeconds.ToString());
                    writeFile.Close();
                }
            }

            AsyncStopCamera(play);
        }

        async void AsyncStopCamera(bool play)
        {
            await _dev.StopRecordingAsync();
            _dev.Dispose();
            _dev = null;

            await _sst.FlushAsync();
            _sst.Dispose();
            _sst = null;

            if (play)
            {
                NavigationService.Navigate(new Uri("/PlayPage.xaml?file=" + lastVideo, UriKind.Relative));
            }
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            TileUtil.UpdateTile();

            this.timerButton.Tap -= CameraButtons_ShutterKeyPressed;
            CameraButtons.ShutterKeyPressed -= CameraButtons_ShutterKeyPressed;
            
            if (_dev != null)
            {
                StopCamera(false);
            }

            base.OnNavigatingFrom(e);
        }

        protected override void OnOrientationChanged(OrientationChangedEventArgs e)
        {
            if (e.Orientation == PageOrientation.LandscapeLeft)
            {
                base.OnOrientationChanged(e);
            }
        }

        async void ActivateCamera()
        {
            if (_dev == null)
            {
                if (AudioVideoCaptureDevice.AvailableSensorLocations.Contains(CameraSensorLocation.Back))
                {
                    _dev = await AudioVideoCaptureDevice.OpenAsync(CameraSensorLocation.Back, AudioVideoCaptureDevice.GetAvailableCaptureResolutions(CameraSensorLocation.Back).First());

                    await _dev.SetPreviewResolutionAsync(AudioVideoCaptureDevice.GetAvailablePreviewResolutions(CameraSensorLocation.Back).First());

                    _videoBrush = new VideoBrush();
                    _videoBrush.SetSource(_dev);
                    this.videoRect.Fill = _videoBrush;

                    lastVideo = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");

                    //////////Thumbnail
                    using (var storage = IsolatedStorageFile.GetUserStoreForApplication())
                    {
                        using (var file = storage.CreateFile(lastVideo + ".jpg"))
                        {
                            WriteableBitmap bmap = new WriteableBitmap((int)_dev.PreviewResolution.Width, (int)_dev.PreviewResolution.Height);
                            _dev.GetPreviewBufferArgb(bmap.Pixels);
                            bmap.SaveJpeg(file, bmap.PixelWidth, bmap.PixelHeight, 0, 90);
                        }
                    }

                    ///////////Video
                    StorageFolder localFolder = ApplicationData.Current.LocalFolder;
                    StorageFile videoFile = await localFolder.CreateFileAsync(lastVideo + ".mp4", CreationCollisionOption.ReplaceExisting);

                    _sst = await videoFile.OpenAsync(FileAccessMode.ReadWrite);
                    await _dev.StartRecordingToStreamAsync(_sst);

                    ////////////////////////////////////////////////////////////////////
                    this.RecIconStarting.Visibility = Visibility.Collapsed;
                    _recStartTime = DateTime.Now;
                    dt_Tick(null, null);
                    _dt.Start();

                    this.timerButton.Tap += CameraButtons_ShutterKeyPressed;
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
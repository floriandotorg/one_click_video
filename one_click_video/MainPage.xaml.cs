using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Devices;
using WP7Contrib.View.Transitions.Animation;
using System.Windows.Media.Imaging;
using System.IO.IsolatedStorage;

namespace one_click_video
{
    public partial class MainPage : AnimatedBasePage
    {
        private VideoCamera _videoCamera;
        private VideoCameraVisualizer _videoCameraVisualizer;
        private VideoBrush _videoBrush;
        private System.Windows.Threading.DispatcherTimer _dt;
        private DateTime _recStartTime;

        // Konstruktor
        public MainPage()
        {
            AnimationContext = LayoutRoot;

            InitializeComponent();

            _videoCamera = new VideoCamera();

            // Event is fired when the video camera object has been initialized.
            _videoCamera.Initialized += VideoCamera_Initialized;

            // Add the photo camera to the video source
            _videoCameraVisualizer = new VideoCameraVisualizer();

            _videoBrush = ((_videoCameraVisualizer.InnerObject() as UserControl).Content as Rectangle).Fill as VideoBrush;
            _videoBrush.RelativeTransform = new RotateTransform() { CenterX = 0.5, CenterY = 0.5, Angle = 0 };
            _videoBrush.Stretch = Stretch.Fill;
            this.videoRect.Fill = _videoBrush;

            _videoCameraVisualizer.SetSource(_videoCamera);
        }

        protected override void OnOrientationChanged(OrientationChangedEventArgs e)
        {
            if (e.Orientation == PageOrientation.LandscapeLeft)
            {
                base.OnOrientationChanged(e);
            }
        }

        private void VideoCamera_Initialized(object sender, EventArgs e)
        {
            _videoCamera.RecordingStarted += VideoCamera_RecordingStarted;
            _videoCamera.ThumbnailSavedToDisk += VideoCamera_ThumbnailSavedToDisk;
            _videoCamera.StartRecording();
        }

        private void VideoCamera_RecordingStarted(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(new Action( () =>
                {
                    _videoCamera.ShutterPressed += ShutterPressed;

                    this.RecIconStarting.Visibility = Visibility.Collapsed;

                    _recStartTime = DateTime.Now;
                    dt_Tick(null, null);
                    _dt = new System.Windows.Threading.DispatcherTimer();
                    _dt.Interval = new TimeSpan(0, 0, 0, 1, 0);
                    _dt.Tick += dt_Tick;
                    _dt.Start();
                }));
        }

        void dt_Tick(object sender, EventArgs e)
        {
            TimeSpan duration = DateTime.Now - _recStartTime;
            this.RecTimer.Text = duration.ToString(@"mm\:ss");
        }

        private void ShutterPressed(object sender, object e)
        {
            _dt.Stop();
            _videoCamera.StopRecording();
        }


        private void VideoCamera_ThumbnailSavedToDisk(object sender, ContentReadyEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                string path = System.IO.Path.GetFileNameWithoutExtension(e.RelativePath);
                path = path.Remove(path.Length - 3, 3) + ".mp4";

                BitmapImage image = new BitmapImage();
                using (var isoStore = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    using (var stream = isoStore.OpenFile(e.RelativePath, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                    {
                        image.SetSource(stream);
                    }
                }

                this.videoRect.Fill = new ImageBrush() { ImageSource = image };
                //NavigationService.Navigate(new Uri("/PlayPage.xaml?video=" + path, UriKind.Relative));
                _videoCamera.AddMediaToCameraRoll(path, e.RelativePath);
            }));
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
                return new SlideDownAnimator { RootElement = LayoutRoot };
            }

            return new SlideUpAnimator { RootElement = this.LayoutRoot };
        }
    }
}
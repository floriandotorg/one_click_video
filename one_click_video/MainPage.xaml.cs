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
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using System.Windows.Resources;
using utility;

namespace one_click_video
{
    public partial class MainPage : AnimatedBasePage
    {
        private VideoCamera _videoCamera;
        private VideoCameraVisualizer _videoCameraVisualizer;
        private VideoBrush _videoBrush;
        private System.Windows.Threading.DispatcherTimer _dt;
        private DateTime _recStartTime;
        private bool first = true;

        // Konstruktor
        public MainPage()
        {
            AnimationContext = LayoutRoot;

            InitializeComponent();

            using (var storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                storage.DeletePath("");
            }

            StartVideo();
        }

        private void StartVideo()
        {
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

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            this.prog.Visibility = Visibility.Collapsed;
            this.progbar.IsIndeterminate = false;

            base.OnNavigatedTo(e);

            if (!first)
            {
                StartVideo();
            }
            first = false;
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
            Dispatcher.BeginInvoke(new Action(() =>
                {
                    _videoCamera.ShutterPressed += ShutterPressed;
                    this.timerButton.Tap += ShutterPressed;

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
            TimeSpan duration = DateTime.Now - _recStartTime + TimeSpan.FromSeconds(1);
            this.RecTimer.Text = duration.ToString(@"mm\:ss");
        }

        private void ShutterPressed(object sender, object e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                this.progbar.IsIndeterminate = true;
                this.prog.Visibility = Visibility.Visible;

                this.RecTimer.Text = "--:--";
                this.RecIconStarting.Visibility = Visibility.Visible;
            }));

            _dt.Stop();
            _videoCamera.StopRecording();
        }

        private void VideoCamera_ThumbnailSavedToDisk(object sender, ContentReadyEventArgs e)
        {
            string path = System.IO.Path.GetFileNameWithoutExtension(e.RelativePath);
            path = path.Remove(path.Length - 3, 3) + ".mp4";

            using (var storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                storage.CopyFile(path, "lastVideo.mp4", true);
            }

            _videoCamera.AddMediaToCameraRoll(path, e.RelativePath);

            Dispatcher.BeginInvoke(new Action(() =>
            {
                NavigationService.Navigate(new Uri("/PlayPage.xaml", UriKind.Relative));
            }));
        }
    }
}
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

namespace one_click_video
{
    public partial class MainPage : PhoneApplicationPage
    {
        private VideoCamera _videoCamera;
        private VideoCameraVisualizer _videoCameraVisualizer;
        private VideoBrush _videoBrush;

        // Konstruktor
        public MainPage()
        {  
            InitializeComponent();

            _videoCamera = new VideoCamera();

            // Event is fired when the video camera object has been initialized.
            _videoCamera.Initialized += VideoCamera_Initialized;
            _videoCamera.ShutterPressed += Button_Click_1;

            // Add the photo camera to the video source
            _videoCameraVisualizer = new VideoCameraVisualizer();

            _videoBrush = ((_videoCameraVisualizer.InnerObject() as UserControl).Content as Rectangle).Fill as VideoBrush;
            _videoBrush.RelativeTransform = new RotateTransform() { CenterX = 0.5, CenterY = 0.5, Angle = 0 };
            _videoBrush.Stretch = Stretch.Fill;
            this.videoRect.Fill = _videoBrush;

            _videoCameraVisualizer.SetSource(_videoCamera);
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
                    this.stopButton.IsEnabled = true; 
                }));
        }

        private void ShutterPressed(object sender, object e)
        {
            _videoCamera.StopRecording();
        }

        private void Button_Click_1(object sender, object e)
        {
            _videoCamera.StopRecording();
        }

        private void VideoCamera_ThumbnailSavedToDisk(object sender, ContentReadyEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                string path = System.IO.Path.GetFileNameWithoutExtension(e.RelativePath);
                path = path.Remove(path.Length - 3, 3) + ".mp4";
                _videoCamera.AddMediaToCameraRoll(path, e.RelativePath);
            }));
        }
    }
}
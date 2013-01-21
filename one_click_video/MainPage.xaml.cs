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

            // Add the photo camera to the video source
            _videoCameraVisualizer = new VideoCameraVisualizer();

            _videoBrush = ((_videoCameraVisualizer.InnerObject() as UserControl).Content as Rectangle).Fill as VideoBrush;
            _videoBrush.Stretch = Stretch.Fill;
            this.videoRect.Fill = _videoBrush;
            RecordingPage_OrientationChanged(null, null);

            _videoCameraVisualizer.SetSource(_videoCamera);

            OrientationChanged += RecordingPage_OrientationChanged;        
        }

        private void RecordingPage_OrientationChanged(object sender, object e)
        {
            if (this.Orientation == PageOrientation.LandscapeLeft)
            {
                _videoBrush.RelativeTransform = new RotateTransform() { CenterX = 0.5, CenterY = 0.5, Angle = 0 };
                this.videoRect.Width = 640;
                this.videoRect.Height = 480;
                this.videoRect.Margin = new Thickness(80,0,48,0);
            }
            else if (this.Orientation == PageOrientation.LandscapeRight)
            {
                _videoBrush.RelativeTransform = new RotateTransform() { CenterX = 0.5, CenterY = 0.5, Angle = 180 };
                this.videoRect.Width = 640;
                this.videoRect.Height = 480;
                this.videoRect.Margin = new Thickness(80, 0, 48, 0);
            }
            else if (this.Orientation == PageOrientation.PortraitUp)
            {
                _videoBrush.RelativeTransform = new RotateTransform() { CenterX = 0.5, CenterY = 0.5, Angle = 90 };
                this.videoRect.Width = 480;
                this.videoRect.Height = 640;
                this.videoRect.Margin = new Thickness(0,45, 0, 48);
            }
        }

        private void VideoCamera_Initialized(object sender, EventArgs e)
        {
            
        }
    }
}
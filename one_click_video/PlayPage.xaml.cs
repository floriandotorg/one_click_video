using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using WP7Contrib.View.Transitions.Animation;
using System.IO.IsolatedStorage;
using System.Windows.Media;

namespace one_click_video
{
    public partial class PlayPage : AnimatedBasePage
    {
        private System.Windows.Threading.DispatcherTimer _dt;

        public PlayPage()
        {
            AnimationContext = LayoutRoot;

            InitializeComponent();

            this.button.Tap += ButtonTap;
            this.mediaElement.Tap += mediaElement_Tap;
            this.mediaElement.CurrentStateChanged += mediaElement_CurrentStateChanged;
            this.mediaElement.MediaEnded += mediaElement_MediaEnded;
        }

        void mediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            this.play.Visibility = Visibility.Visible;
            this.pause.Visibility = Visibility.Collapsed;
            this.controls.Opacity = 1;
            this.controls.Visibility = Visibility.Visible;
        }

        void mediaElement_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (this.controls.Visibility == Visibility.Visible)
            {
                this.controls_out.Begin();
            }
            else
            {
                this.controls_in.Begin();
            }
        }

        void mediaElement_CurrentStateChanged(object sender, RoutedEventArgs e)
        {
            if (this.mediaElement.CurrentState == System.Windows.Media.MediaElementState.Playing)
            {
                this.play.Visibility = Visibility.Collapsed;
                this.pause.Visibility = Visibility.Visible;
            }
            else if (this.mediaElement.CurrentState == System.Windows.Media.MediaElementState.Paused)
            {
                this.play.Visibility = Visibility.Visible;
                this.pause.Visibility = Visibility.Collapsed;
            }   
        }

        void ButtonTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (this.mediaElement.CurrentState == System.Windows.Media.MediaElementState.Playing)
            {
                this.mediaElement.Pause();
            }
            else if (this.mediaElement.CurrentState == System.Windows.Media.MediaElementState.Paused)
            {
                this.mediaElement.Play();
            }
        }

        private void timeline_ManipulationCompleted(object sender, System.Windows.Input.ManipulationCompletedEventArgs e)
        {
            _dt.Start();
            this.mediaElement.Position = new TimeSpan(0, 0, 0, 0, (int)(this.mediaElement.NaturalDuration.TimeSpan.TotalMilliseconds * this.timeline.Value));
        }

        private void timeline_ManipulationStarted(object sender, System.Windows.Input.ManipulationStartedEventArgs e)
        {
            _dt.Stop();
        }

        void dt_Tick(object sender, EventArgs e)
        {
            if (this.mediaElement.NaturalDuration.HasTimeSpan && this.mediaElement.NaturalDuration.TimeSpan.TotalMilliseconds != 0)
            {
                this.timeline.Value = this.mediaElement.Position.TotalMilliseconds / this.mediaElement.NaturalDuration.TimeSpan.TotalMilliseconds;
                this.timer.Text = this.mediaElement.Position.ToString(@"m\:ss") + " / " + this.mediaElement.NaturalDuration.TimeSpan.ToString(@"m\:ss");
            }
            else
            {
                this.timeline.Value = .0;
                this.timer.Text = "0:00 / 0:00";
            }
        }

        protected override void OnOrientationChanged(OrientationChangedEventArgs e)
        {
            if (e.Orientation == PageOrientation.LandscapeLeft)
            {
                base.OnOrientationChanged(e);
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.play.Visibility = Visibility.Collapsed;
            this.pause.Visibility = Visibility.Visible;

            base.OnNavigatedTo(e);

            using (var isoStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (var stream = isoStore.OpenFile(NavigationContext.QueryString["file"] + ".mp4", System.IO.FileMode.Open, System.IO.FileAccess.Read))
                {
                    this.mediaElement.SetSource(stream);
                }
            }

            this.mediaElement.Play();

            dt_Tick(null, null);
            _dt = new System.Windows.Threading.DispatcherTimer();
            _dt.Interval = new TimeSpan(0, 0, 0, 0, 500);
            _dt.Tick += dt_Tick;
            _dt.Start();
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

        private void Button_Click_1(object sender, System.Windows.RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/InfoPage.xaml", UriKind.Relative));
        }

        private void button_ManipulationStarted(object sender, System.Windows.Input.ManipulationStartedEventArgs e)
        {
            this.elli.Fill = new SolidColorBrush(Color.FromArgb(255,255,255,255));
            this.pause1.Fill = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.pause1.Stroke = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.pause2.Fill = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.pause2.Stroke = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.play.Fill = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            this.play.Stroke = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
        }

        private void button_ManipulationCompleted(object sender, System.Windows.Input.ManipulationCompletedEventArgs e)
        {
            this.elli.Fill = new SolidColorBrush(Color.FromArgb(0, 255, 255, 255));
            this.pause1.Fill = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
            this.pause1.Stroke = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
            this.pause2.Fill = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
            this.pause2.Stroke = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
            this.play.Fill = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
            this.play.Stroke = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
        }
    }
}
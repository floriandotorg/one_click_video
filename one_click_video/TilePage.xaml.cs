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

namespace one_click_video
{
    public partial class TilePage : AnimatedBasePage
    {
        private System.Windows.Threading.DispatcherTimer _dt = new System.Windows.Threading.DispatcherTimer();

        public TilePage()
        {
            InitializeComponent();

            AnimationContext = LayoutRoot;

            _dt.Interval = new TimeSpan(0, 0, 0, 0, 500);
            _dt.Tick += dt_Tick;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            ShellTile SecondaryTile = ShellTile.ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().Contains("RecordingPage"));

            if (SecondaryTile != null)
            {
                _dt.Start();
            }
        }

        void dt_Tick(object sender, EventArgs e)
        {
            _dt.Stop();
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }

        private void Button_Click_1(object sender, System.Windows.RoutedEventArgs e)
        {
            ShellTile SecondaryTile = ShellTile.ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().Contains("RecordingPage"));

            if (SecondaryTile == null)
            {
                FlipTileData tile = new FlipTileData()
                {
                    BackgroundImage = new Uri("RecordingTileImageMedium.png", UriKind.Relative),
                    SmallBackgroundImage = new Uri("RecordingTileImageSmall.png", UriKind.Relative),
                    WideBackgroundImage = new Uri("RecordingTileImageLarge.png", UriKind.Relative),
                    Title = "1-Click Video"
                };

                ShellTile.Create(new Uri("/RecordingPage.xaml", UriKind.Relative), tile, true);
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
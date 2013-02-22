using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace one_click_video
{
    public partial class TilePage : PhoneApplicationPage
    {
        public TilePage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            ShellTile SecondaryTile = ShellTile.ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().Contains("RecordingPage"));

            if (SecondaryTile != null)
            {
                NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
            }
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
    }
}
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

        private void Button_Click_1(object sender, System.Windows.RoutedEventArgs e)
        {
            ShellTile SecondaryTile = ShellTile.ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().Contains("RecordingPage"));

            if (SecondaryTile == null)
            {
                StandardTileData tile = new StandardTileData();

                tile.BackgroundImage = new Uri("RecordingTileImage.png", UriKind.Relative);
                tile.Title = "1-Click Video";

                ShellTile.Create(new Uri("/RecordingPage.xaml", UriKind.Relative), tile);
            }
        }
    }
}
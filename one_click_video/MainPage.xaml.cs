﻿using System;
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
using System.Reflection;
using Microsoft.Phone.Info;
using Windows.Phone.Media.Capture;
using System.Windows.Navigation;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.Foundation;
using Microsoft.Phone.Tasks;
using Microsoft.Phone.Shell;
using WP7Contrib.View.Transitions.Animation;
using Windows.Storage.FileProperties;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using System.Globalization;
using System.IO.IsolatedStorage;
using utility;
using System.IO;
using one_click_video.Resources;

namespace one_click_video
{
    public class ImageMetadata : DependencyObject
    {
        public string Data
        {
            get { return (string)GetValue(MyPropertyProperty); }
            set { SetValue(MyPropertyProperty, value); }
        }

        public static readonly DependencyProperty MyPropertyProperty =
            DependencyProperty.Register("Data", typeof(string), typeof(ImageMetadata), new PropertyMetadata(""));
    }

    public class ListBoxVideoItem
    {
        public string File { get; set; }
        public BitmapImage Thumbnail { get; set; }
        public string Duration { get; set; }
    }

    public partial class MainPage : AnimatedBasePage
    {
        private System.Windows.Threading.DispatcherTimer _dt = new System.Windows.Threading.DispatcherTimer();
        private ApplicationBar _applicationBar;
        private ApplicationBar _choosingApplicationBar;
        private bool _choosing = false;

        // Konstruktor
        public MainPage()
        {
            InitializeComponent();

            AnimationContext = LayoutRoot;

            _dt.Interval = new TimeSpan(0, 0, 0, 0, 500);
            _dt.Tick += dt_Tick;

            BuildLocalizedApplicationBars();
            ApplicationBar = _applicationBar;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            ShellTile SecondaryTile = ShellTile.ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().Contains("RecordingPage"));

            if (SecondaryTile == null)
            {
                _dt.Start();
            }
            else
            {
                createThumbnails();
            }
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            TileUtil.UpdateTile();
            base.OnNavigatingFrom(e);
        }

        private void createThumbnails()
        {
            var fileList = new List<ListBoxVideoItem>();

            using (var storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                foreach (var file in storage.GetFileNames())
                {
                    if (file.EndsWith(".mp4"))
                    {
                        string rawFilename = System.IO.Path.GetFileNameWithoutExtension(file);

                        var item = new ListBoxVideoItem() { File = rawFilename, Thumbnail = new BitmapImage() };

                        using (StreamReader reader = new StreamReader(storage.OpenFile(rawFilename + ".metadata", FileMode.Open, FileAccess.Read)))
                        {
                            double totalSeconds = 0;
                            Double.TryParse(reader.ReadLine(), out totalSeconds);
                            item.Duration = TimeSpan.FromSeconds(totalSeconds).ToString(@"m\:ss");
                        }

                        using (var imageFile = storage.OpenFile(rawFilename + ".jpg", System.IO.FileMode.Open, System.IO.FileAccess.Read))
                        {
                            item.Thumbnail.SetSource(imageFile);
                        }

                        fileList.Add(item);
                    }
                }
            }

            this.PictureGrid.ItemsSource = fileList;
        }

        void dt_Tick(object sender, EventArgs e)
        {
            _dt.Stop();
            NavigationService.Navigate(new Uri("/TilePage.xaml", UriKind.Relative));
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

        private string FilenameFromObject(object sender)
        {
            return (((Grid)sender).Resources["Metadata"] as ImageMetadata).Data;
        }

        void GridTap(object sender, System.Windows.Input.GestureEventArgs args)
        {
            if (_choosing)
            {
                Rectangle rect = FindChild.Do<Rectangle>((Grid)sender, "active");
                if (rect.Visibility == Visibility.Visible)
                {
                    rect.Visibility = Visibility.Collapsed;
                }
                else
                {
                    rect.Visibility = Visibility.Visible;
                }
            }
            else
            {
                NavigationService.Navigate(new Uri("/PlayPage.xaml?file=" + FilenameFromObject(sender), UriKind.Relative));
            }
        }

        private void BuildLocalizedApplicationBars()
        {
            string theme = "Light/";
            if ((Visibility)App.Current.Resources["PhoneDarkThemeVisibility"] == Visibility.Visible)
            {
                theme = "Dark/";
            }

            //_applicationBar
            {
                // ApplicationBar der Seite einer neuen Instanz von ApplicationBar zuweisen
                _applicationBar = new ApplicationBar();
                _applicationBar.BackgroundColor = Color.FromArgb(255, 0, 0, 0);
                _applicationBar.Opacity = .2;

                // Eine neue Schaltfläche erstellen und als Text die lokalisierte Zeichenfolge aus AppResources zuweisen.
                ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/" + theme + "appbar.manage.rest.png", UriKind.Relative));
                appBarButton.Text = AppResources.Select;
                appBarButton.Click += appBarButton_Click;
                _applicationBar.Buttons.Add(appBarButton);
            }

            //_choosingApplicationBar
            {
                _choosingApplicationBar = new ApplicationBar();
                _choosingApplicationBar.BackgroundColor = Color.FromArgb(255, 0, 0, 0);
                _choosingApplicationBar.Opacity = .2;

                ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/" + theme + "appbar.manage.rest.png", UriKind.Relative));
                appBarButton.Text = AppResources.SelectEnd;
                appBarButton.Click += appBarButton_Click_Choosing;
                _choosingApplicationBar.Buttons.Add(appBarButton);

                appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/" + theme + "delete.png", UriKind.Relative));
                appBarButton.Text = AppResources.Delete;
                appBarButton.Click += appBarButtonDelete_Click;
                _choosingApplicationBar.Buttons.Add(appBarButton);

                ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.SelectAll);
                appBarMenuItem.Click += appBarMenuItem_Click;
                _choosingApplicationBar.MenuItems.Add(appBarMenuItem);
            }
        }

        void appBarButtonDelete_Click(object sender, EventArgs e)
        {
            using (var isoStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                foreach (var item in this.PictureGrid.Items)
                {
                    var container = this.PictureGrid.ItemContainerGenerator.ContainerFromItem(item);

                    if (FindChild.Do<Rectangle>(container, "active").Visibility == Visibility.Visible)
                    {
                        Grid grid = FindChild.Do<Grid>(container, "ListBoxGrid");

                        string file = FilenameFromObject(grid);
                        isoStore.DeleteFile(file + ".jpg");
                        isoStore.DeleteFile(file + ".mp4");
                        isoStore.DeleteFile(file + ".metadata");
                    }
                }
            }

            choosingOff();
            createThumbnails();
        }

        void appBarMenuItem_Click(object sender, EventArgs e)
        {
            foreach (var item in this.PictureGrid.Items)
            {
                FindChild.Do<Rectangle>(this.PictureGrid.ItemContainerGenerator.ContainerFromItem(item), "active").Visibility = Visibility.Visible;
            }
        }

        private void choosingOn()
        {
            foreach (var item in this.PictureGrid.Items)
            {
                FindChild.Do<Rectangle>(this.PictureGrid.ItemContainerGenerator.ContainerFromItem(item), "inactive").Visibility = Visibility.Visible;
            }

            _choosing = true;
            ApplicationBar = _choosingApplicationBar;
        }

        private void choosingOff()
        {
            foreach (var item in this.PictureGrid.Items)
            {
                var container = this.PictureGrid.ItemContainerGenerator.ContainerFromItem(item);
                FindChild.Do<Rectangle>(container, "active").Visibility = Visibility.Collapsed;
                FindChild.Do<Rectangle>(container, "inactive").Visibility = Visibility.Collapsed;
            }

            _choosing = false;
            ApplicationBar = _applicationBar;
        }

        void appBarButton_Click_Choosing(object sender, EventArgs e)
        {
            choosingOff();
        }

        void appBarButton_Click(object sender, EventArgs e)
        {
            choosingOn();
        }
    }
}
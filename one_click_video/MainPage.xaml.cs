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
using System.Windows;

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

    public class ThumbnailConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                var image = new BitmapImage();
                using (var isoStore = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    using (var file = isoStore.OpenFile((string)value, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                    {
                        image.SetSource(file);
                    }
                }
                return image;
            }
            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new System.NotImplementedException();
        }
    }

    public partial class MainPage : AnimatedBasePage
    {
        private System.Windows.Threading.DispatcherTimer _dt = new System.Windows.Threading.DispatcherTimer();

        // Konstruktor
        public MainPage()
        {
            InitializeComponent();

            AnimationContext = LayoutRoot;

            _dt.Interval = new TimeSpan(0, 0, 0, 0, 500);
            _dt.Tick += dt_Tick;

            BuildLocalizedApplicationBar();
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
                CreateThumbnails();
            }
        }

        private async void CreateThumbnails()
        {
            var fileList = new List<string>();

            foreach (var file in await ApplicationData.Current.LocalFolder.GetFilesAsync())
            {
                if (file.Name.EndsWith(".jpg"))
                {
                    fileList.Add(file.Name);
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
            return System.IO.Path.GetFileNameWithoutExtension((((Grid)sender).Resources["Metadata"] as ImageMetadata).Data);
        }

        void GridTap(object sender, System.Windows.Input.GestureEventArgs args)
        {
            NavigationService.Navigate(new Uri("/PlayPage.xaml?file=" + FilenameFromObject(sender), UriKind.Relative));
        }

        private void BuildLocalizedApplicationBar()
        {
            // ApplicationBar der Seite einer neuen Instanz von ApplicationBar zuweisen
            ApplicationBar = new ApplicationBar();
            ApplicationBar.BackgroundColor = Color.FromArgb(255, 0, 0, 0);
            ApplicationBar.Opacity = .2;

            string theme = "Light/";
            if ((Visibility)App.Current.Resources["PhoneDarkThemeVisibility"] == Visibility.Visible)
            {
                theme = "Dark/";
            }

            // Eine neue Schaltfläche erstellen und als Text die lokalisierte Zeichenfolge aus AppResources zuweisen.
            ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/" + theme + "appbar.manage.rest.png", UriKind.Relative));
            appBarButton.Text = "Auswählen";
            ApplicationBar.Buttons.Add(appBarButton);

            // Ein neues Menüelement mit der lokalisierten Zeichenfolge aus AppResources erstellen
            //ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
            //ApplicationBar.MenuItems.Add(appBarMenuItem);
        }
    }
}
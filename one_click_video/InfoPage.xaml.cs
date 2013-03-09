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
using Microsoft.Phone.Tasks;
using Microsoft.Phone.Info;
using WP7Contrib.View.Transitions.Animation;
using System.Reflection;
using System.Windows.Media.Imaging;

namespace one_click_video
{
    public partial class InfoPage : AnimatedBasePage
    {
        private string version_str;

        public InfoPage()
        {
            InitializeComponent();

            AnimationContext = LayoutRoot;

            // Get Version
            var versionAttrib = new AssemblyName(Assembly.GetExecutingAssembly().FullName);
            version_str = versionAttrib.Version.ToString().Substring(0, 3);

            this.version_text.Text = this.version_text.Text.Replace("0.0.0.0", version_str);

            if ((Visibility)App.Current.Resources["PhoneDarkThemeVisibility"] != Visibility.Visible)
            {
                this.floydLogo.Source = new BitmapImage(new Uri("floyd_light.png", UriKind.Relative));
            }
        }

        private void rate_click(object sender, System.Windows.RoutedEventArgs e)
        {
            MarketplaceReviewTask marketplaceReviewTask = new MarketplaceReviewTask();

            marketplaceReviewTask.Show();
        }

        private void contact_click(object sender, System.Windows.RoutedEventArgs e)
        {
            EmailComposeTask emailComposeTask = new EmailComposeTask();

            string result = "unknown device";
            object deviceName;
            if(DeviceExtendedProperties.TryGetValue("DeviceName", out deviceName))
            {
                result = deviceName.ToString(); 
            }

            emailComposeTask.Subject = AppResources.app_name + " Version " + version_str + " on " + result;
            emailComposeTask.To = "support.video@floyd-ug.de";

            emailComposeTask.Show();
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

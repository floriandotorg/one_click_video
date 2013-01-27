using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Globalization;

namespace one_click_video
{
    public class LocalizedStrings
    {
        public LocalizedStrings()
        {
        }

        private static one_click_video.AppResources localizedResources = new one_click_video.AppResources();

        public one_click_video.AppResources LocalizedResources { get { return localizedResources; } }
    }
}

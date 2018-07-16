using CustomVisionCompanion.UWP.Renderers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Xamarin.Forms;
using Xamarin.Forms.Platform.UWP;

[assembly: ExportRenderer(typeof(ActivityIndicator), typeof(ProgressRingActivityIndicator))]
namespace CustomVisionCompanion.UWP.Renderers
{
    public class ProgressRingActivityIndicator : ViewRenderer<ActivityIndicator, ProgressRing>
    {
        private Brush foregroundDefault;

        protected override void OnElementChanged(ElementChangedEventArgs<ActivityIndicator> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                if (Control == null)
                {
                    SetNativeControl(new ProgressRing { Width = Element.WidthRequest });
                    Control.Loaded += OnControlLoaded;
                }

                UpdateIsRunning();
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == ActivityIndicator.IsRunningProperty.PropertyName)
            {
                UpdateIsRunning();
            }

            else if (e.PropertyName == ActivityIndicator.ColorProperty.PropertyName)
            {
                UpdateColor();
            }
        }

        private void OnControlLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            foregroundDefault = Control.Foreground;
            UpdateColor();
        }

        private void UpdateColor()
        {
            var color = Element.Color;
            if (color == Color.Default)
            {
                Control.Foreground = foregroundDefault;
            }
            else
            {
                Control.Foreground = color.ToBrush();
            }
        }

        private void UpdateIsRunning()
        {
            Control.IsActive = Element.IsRunning;
        }
    }

    internal static class ConvertExtensions
    {
        public static Brush ToBrush(this Color color)
            => new SolidColorBrush(color.ToWindowsColor());

        public static Windows.UI.Color ToWindowsColor(this Color color)
            => Windows.UI.Color.FromArgb((byte)(color.A * 255), (byte)(color.R * 255), (byte)(color.G * 255), (byte)(color.B * 255));
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace NioTup.Lib.Helpers
{
    public class ButtonHelper
    {
        public static readonly DependencyProperty MouseOverColorProperty = DependencyProperty.RegisterAttached("MouseOverColor", typeof(SolidColorBrush), typeof(ButtonHelper), new UIPropertyMetadata(null));


        public static void GetMouseOverColor(DependencyObject obj, SolidColorBrush value)
        {
            obj.SetValue(MouseOverColorProperty, value);
        }

        public static void SetMouseOverColor(DependencyObject obj, SolidColorBrush value)
        {
            obj.SetValue(MouseOverColorProperty, value);
        }

        public static readonly DependencyProperty PressedColorProperty = DependencyProperty.RegisterAttached("PressedColor", typeof(SolidColorBrush), typeof(ButtonHelper), new UIPropertyMetadata(null));


        public static void GetPressedColor(DependencyObject obj, SolidColorBrush value)
        {
            obj.SetValue(PressedColorProperty, value);
        }

        public static void SetPressedColor(DependencyObject obj, SolidColorBrush value)
        {
            obj.SetValue(PressedColorProperty, value);
        }

        public static readonly DependencyProperty FocusedColorProperty = DependencyProperty.RegisterAttached("FocusedColor", typeof(SolidColorBrush), typeof(ButtonHelper), new UIPropertyMetadata(null));


        public static void GetFocusedColor(DependencyObject obj, SolidColorBrush value)
        {
            obj.SetValue(FocusedColorProperty, value);
        }

        public static void SetFocusedColor(DependencyObject obj, SolidColorBrush value)
        {
            obj.SetValue(FocusedColorProperty, value);
        }
    }
}

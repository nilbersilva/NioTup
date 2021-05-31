using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NioTup.Lib.Controls
{
    public class ToggleSwitch : ToggleButton, INotifyPropertyChanged
    {
        static ToggleSwitch()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ToggleSwitch), new FrameworkPropertyMetadata(typeof(ToggleSwitch)));
        }
       

        public event PropertyChangedEventHandler PropertyChanged;
        public void Notify([CallerMemberName] string prop = null)
        {
            if (string.IsNullOrWhiteSpace(prop)) return;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public static DependencyProperty SwitchTrackOnBackgroundProperty = DependencyProperty.Register("SwitchTrackOnBackground", typeof(Brush), typeof(ToggleSwitch));
        public Brush SwitchTrackOnBackground
        {
            get
            {
                return (Brush)GetValue(SwitchTrackOnBackgroundProperty);
            }
            set
            {
                SetValue(SwitchTrackOnBackgroundProperty, value);
                Notify();
            }
        }

        public static DependencyProperty SwitchTrackOffBackgroundProperty = DependencyProperty.Register("SwitchTrackOffBackground", typeof(Brush), typeof(ToggleSwitch));
        public Brush SwitchTrackOffBackground
        {
            get
            {
                return (Brush)GetValue(SwitchTrackOffBackgroundProperty);
            }
            set
            {
                SetValue(SwitchTrackOffBackgroundProperty, value);
                Notify();
            }
        }

        public static DependencyProperty SwitchTrackOnContentProperty = DependencyProperty.Register("SwitchTrackOnContent", typeof(UIElement), typeof(ToggleSwitch));
        public UIElement SwitchTrackOnContent
        {
            get
            {
                return (UIElement)GetValue(SwitchTrackOnContentProperty);
            }
            set
            {
                SetValue(SwitchTrackOnContentProperty, value);
                Notify();
                HasSwitchContent = value != null;
            }
        }

        public static DependencyProperty SwitchTrackOffContentProperty = DependencyProperty.Register("SwitchTrackOffContent", typeof(UIElement), typeof(ToggleSwitch));
        public UIElement SwitchTrackOffContent
        {
            get
            {
                return (UIElement)GetValue(SwitchTrackOffContentProperty);
            }
            set
            {
                SetValue(SwitchTrackOffContentProperty, value);
                Notify();
                HasSwitchContent = value != null;
            }
        }

        public static DependencyProperty HasSwitchContentProperty = DependencyProperty.Register("HasSwitchContent", typeof(bool), typeof(ToggleSwitch));

        public bool HasSwitchContent
        {
            get
            {
                return (bool)GetValue(HasSwitchContentProperty);
            }
            set
            {
                SetValue(HasSwitchContentProperty, value);
                Notify();
            }
        }

        public ToggleSwitch()
        {
            this.Checked += toggleSwitch_Checked;
            this.Unchecked += toggleSwitch_Checked;
        }

        private void toggleSwitch_Checked(object sender, RoutedEventArgs e)
        {
            if (IsChecked == true)
            {
                HasSwitchContent = SwitchTrackOnContent != null;
            }
            else
            {
                HasSwitchContent = SwitchTrackOffContent != null;
            }
        }
    }
}

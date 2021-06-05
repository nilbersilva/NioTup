using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace NioTupApp.Converters
{
    public class SetupRelativeImagePathConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 2 && values[0] != DependencyProperty.UnsetValue && values[1] != DependencyProperty.UnsetValue)
            {
                {
                    string ConfigPath = (string)values[0];
                    string imageNamePath = (string)values[1];

                    if (string.IsNullOrWhiteSpace(ConfigPath) || string.IsNullOrWhiteSpace(imageNamePath)) return null;

                    string returnPath =  Path.Combine(Path.GetDirectoryName(Path.GetFullPath(ConfigPath)), imageNamePath);
                    if (File.Exists(returnPath))
                    {
                        return new BitmapImage(new Uri(returnPath));
                    }
                    return null;
                }            
            }
            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}

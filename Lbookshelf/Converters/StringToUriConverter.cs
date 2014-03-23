using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Lbookshelf.Converters
{
    public class StringToUriConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var fragment = (string)value;
            var baseUri = (string)parameter;
            return new Uri(parameter + "#" + fragment, UriKind.Relative);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var uriString = ((Uri)value).ToString();
            return uriString.Substring(uriString.IndexOf('#') + 1);
        }
    }
}

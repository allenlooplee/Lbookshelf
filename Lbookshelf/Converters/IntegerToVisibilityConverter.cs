using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Lbookshelf.Converters
{
    /// <summary>
    /// 0 -> Collasped
    /// _ -> Visible
    /// If 'inverse' is provided via ConverterParameter, then the above result will be inversed.
    /// </summary>
    public class IntegerToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var flag = ((int)value) == 0;

            var inverse = "inverse".Equals(parameter as string, StringComparison.CurrentCultureIgnoreCase);
            if (inverse)
            {
                flag = !flag;
            }

            return flag ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

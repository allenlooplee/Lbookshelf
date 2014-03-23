using FirstFloor.ModernUI.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Lbookshelf.Converters
{
    public class StringArrayToLinksConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var fragments = value as IEnumerable<string>;
            var baseUri = (string)parameter;

            return new LinkCollection(
                fragments.Select(
                    c => new Link
                    {
                        DisplayName = c,
                        Source = new Uri(baseUri + "#" + c, UriKind.Relative)
                    }));
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

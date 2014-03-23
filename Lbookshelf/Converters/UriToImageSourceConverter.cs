using Lbookshelf.Business;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Lbookshelf.Converters
{
    public class UriToImageSourceConverter : IValueConverter
    {
        public UriToImageSourceConverter()
        {
            _defaultImageSource = new BitmapImage(new Uri(GetAbsolutePath(DefaultPropertyValues.Thumbnail), UriKind.Absolute));
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var imageUri = value as string;
            BitmapImage imageSource = _defaultImageSource;

            if (imageUri != DefaultPropertyValues.Thumbnail)
            {
                if (imageUri.StartsWith("Images"))
                {
                    imageUri = GetAbsolutePath(imageUri);
                }

                imageSource = new BitmapImage();
                imageSource.BeginInit();
                // Set CreateOptions to IgnoreImageCache so that Image refreshes
                // when the uri of the thumbnail stays the same.
                imageSource.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                // Set CacheOption to OnLoad so that image file will not be locked.
                imageSource.CacheOption = BitmapCacheOption.OnLoad;
                imageSource.UriSource = new Uri(imageUri, UriKind.Absolute);
                imageSource.EndInit();
            }

            return imageSource;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private BitmapImage _defaultImageSource;

        private string GetAbsolutePath(string relativePath)
        {
            return Path.Combine(Environment.CurrentDirectory, relativePath);
        }
    }
}

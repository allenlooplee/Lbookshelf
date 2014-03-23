using Lbookshelf.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interactivity;

namespace Lbookshelf.Utils
{
    public class ChangeThumbnailBehavior : Behavior<FrameworkElement>
    {
        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.MouseLeftButtonUp += ShowChooseThumbnailDialog;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            AssociatedObject.MouseLeftButtonUp -= ShowChooseThumbnailDialog;
        }

        private void ShowChooseThumbnailDialog(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DialogService.ShowOpenFileDialog(
                fileName =>
                {
                    // Note that when editing a book, we are actually
                    // editing a clone of the original book.
                    // The changes can be safely discarded if the user
                    // cancel the edit. So feel free to change the
                    // value of Thumbnail. The BookManager is responsible
                    // for copying the new thumbnail.
                    var book = (Book)AssociatedObject.DataContext;
                    book.Thumbnail = fileName;

                }, "Image files|*.jpg;*.png", ".jpg");
        }
    }
}

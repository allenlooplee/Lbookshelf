using Lbookshelf.Business;
using Lbookshelf.Models;
using Lbookshelf.Services;
using Lbookshelf.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interactivity;

namespace Lbookshelf.Utils
{
    public class FindBookInfoAction : TriggerAction<DependencyObject>
    {
        protected override void Invoke(object parameter)
        {
            var book = (Book)((FrameworkElement)AssociatedObject).DataContext;

            var findBookInfoViewModel = new FindBookInfoViewModel();
            findBookInfoViewModel.FindBookInfo(book.Title);

            DialogService.ShowDialog(
                "Find Book Info",
                new Uri("/Content/FindBookInfoControl.xaml", UriKind.Relative),
                findBookInfoViewModel,
                () =>
                {
                    if (findBookInfoViewModel.Status == WorkStatus.RanToCompletion)
                    {
                        book.MergeChanges(findBookInfoViewModel.SelectedBook);
                    }
                },
                new Size(600, 500));
        }
    }
}

using Lbookshelf.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interactivity;
using Lbookshelf.Utils;
using Lbookshelf.Business;
using System.IO;

namespace Lbookshelf.Utils
{
    public class EditBookAction : BookActionBase
    {
        protected override void TakeAction()
        {
            var changed = Target.Clone();

            DialogService.ShowDialog(
                "Edit Book Info",
                new Uri("/Content/EditBookControl.xaml", UriKind.Relative),
                changed,
                () =>
                {
                    try
                    {
                        BookManager.Instance.Update(Target, changed);
                    }
                    catch (BookOpenedException ex)
                    {
                        DialogService.ShowDialog(ex.Message, "Error");
                    }
                });
        }
    }
}

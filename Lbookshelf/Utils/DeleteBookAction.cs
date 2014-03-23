using Lbookshelf.Business;
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
    public class DeleteBookAction : BookActionBase
    {
        protected override void TakeAction()
        {
            if (DialogService.ShowDialog(
                String.Format("Are you sure you want to permanently delete {0}?", Target.Title),
                "Delete Book",
                MessageBoxButton.YesNo))
            {
                try
                {
                    BookManager.Instance.Remove(Target);
                }
                catch (BookOpenedException ex)
                {
                    DialogService.ShowDialog(ex.Message, "Error");
                }
            }
        }
    }
}

using Lbookshelf.Business;
using Lbookshelf.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interactivity;

namespace Lbookshelf.Utils
{
    public class OpenBookAction : BookActionBase
    {
        protected virtual void OpenBook(string path)
        {
            Process.Start(path);
        }

        protected override void TakeAction()
        {
            var path = Path.Combine(StorageManager.Instance.RootDirectory, Target.Category, Target.FileName);

            if (File.Exists(path))
            {
                OpenBook(path);
            }
            else
            {
                // Provide two options for the user via command links (action dialog):
                // 1. Remove the book from the database if it no longer exists.
                // 2. Relocate the file for the book if the user move it intentionally.

                DialogService.ShowDialog(
                    "File Not Found",
                    "The corresponding file is missing. You might have deleted it or moved it to another location manually.",
                    Tuple.Create<string, string, Action>(
                        "Remove the book from the database",
                        "If you have deleted the file accidentally or intentionally, then the book is no longer usable.",
                        () => BookManager.Instance.Remove(Target)),
                    Tuple.Create<string, string, Action>(
                        "Relocate the file and open the book",
                        "If you have moved the file to another location, then the location needs updating.",
                        () =>
                        {
                            // The dialog will start will start with the current directory set to library root.
                            // But we still need to handle if the user choose a file outside of the library.

                            DialogService.ShowOpenFileDialog(
                                fileName =>
                                {
                                    var directory = Path.GetDirectoryName(fileName);
                                    var changedBook = Target.Clone();
                                    changedBook.Category = Path.GetFileName(directory);
                                    changedBook.FileName = Path.GetFileName(fileName);

                                    BookManager.Instance.Update(Target, changedBook);
                                    OpenBook(fileName);
                                },
                                StorageManager.Instance.RootDirectory);
                        }));
            }
        }
    }
}

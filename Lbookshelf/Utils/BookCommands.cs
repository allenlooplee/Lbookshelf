﻿using Lbookshelf.Business;
using Lbookshelf.Models;
using Lbookshelf.Services;
using Lbookshelf.ViewModels;
using Microsoft.Expression.Interactivity.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Lbookshelf.Utils
{
    public static class BookCommands
    {
        static BookCommands()
        {
            OpenPinnedBookCommand = CreateCommand(OpenPinnedBook);

            OpenBookCommand = CreateCommand(OpenBook);

            // If the path doesn't exist, the File Explorer will open with default view.
            OpenBookInFileExplorerCommand = CreateCommand(
                book => Process.Start("explorer.exe", "/select, " + book.GetPath()));

            AddToBooklistCommand = CreateCommand(
                obj =>
                {
                    var book = (Book)obj;
                    var dimension = DimensionManager.Instance.SupportedDimensions.First(d => d.Key == "Booklist");

                    // The booklists that already contain the book will be filtered out.
                    var dataContext = Pair.Make(
                        //dimension.Elements.Where(g => g.Elements.All(e => e.Title != book.Title)).Select(g => g.Key),
                        dimension.Elements.Where(g => !g.Elements.Contains(book)).Select(g => g.Key),
                        "");

                    // Show a dialog for the user to select an existing booklist or create a new one.
                    DialogService.ShowDialog(
                        "Choose a booklist",
                        new Uri("/Content/ChooseBooklistControl.xaml", UriKind.Relative),
                        dataContext,
                        () =>
                        {
                            dimension.Add(dataContext.Item2, book);
                        },
                        new Size(350, 200));
                });

            RemoveFromBooklistCommand = CreateCommand(
                obj =>
                {
                    var book = (Book)obj;
                    var booklist = DimensionManager.Instance.SupportedDimensions.First(d => d.Key == "Booklist");

                    booklist.Remove(App.BrowseBooksViewModel.SelectedGroupKey, book);
                });

            EditBookCommand = CreateCommand(CreateGuardedAction(
                book =>
                {
                    var changed = book.Clone();

                    DialogService.ShowDialog(
                        "Edit Book Info",
                        new Uri("/Content/EditBookControl.xaml", UriKind.Relative),
                        changed,
                        () =>
                        {
                            try
                            {
                                // If the use choose another thumbnail, the file name of
                                // the new thumbnail must be different from the original
                                // one since the OpenFileDialog returns an absolute path
                                // and original one is a relative path.
                                var thumbnailChanged = book.Thumbnail != changed.Thumbnail;

                                BookManager.Instance.Update(book, changed);

                                // Refresh the thumbnail if it's been changed.
                                if (thumbnailChanged)
                                {
                                    book.RefreshThumbnail();
                                }
                            }
                            catch (BookOpenedException ex)
                            {
                                DialogService.ShowDialog(ex.Message, "Error");
                            }
                        });
                }));

            DeleteBookCommand = CreateCommand(CreateGuardedAction(
                book =>
                {
                    if (DialogService.ShowDialog(
                        String.Format("Are you sure you want to permanently delete {0}?", book.Title),
                        "Delete Book",
                        MessageBoxButton.YesNo))
                    {
                        try
                        {
                            BookManager.Instance.Remove(book);
                            
                            // We should also remove it from Home, pinned, recently added, recently opened, if any.
                            App.HomeViewModel.OnBookRemoved(book);
                        }
                        catch (BookOpenedException ex)
                        {
                            DialogService.ShowDialog(ex.Message, "Error");
                        }
                    }
                }));

            FindBookInfoCommand = CreateCommand(
                obj =>
                {
                    var book = (Book)obj;

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
                });

            ShowBookInfoCommand = CreateCommand(
                obj =>
                {
                    var original = (Book)obj;
                    var clone = original.Clone();
                    clone.Title = null;

                    DialogService.ShowDialog(
                        original.Title,
                        new Uri("/Content/BookInfoControl.xaml", UriKind.Relative),
                        clone,
                        () => { },
                        new Size(500, 500));
                });
        }

        public static ICommand OpenPinnedBookCommand { get; private set; }

        public static ICommand OpenBookCommand { get; private set; }

        public static ICommand OpenBookInFileExplorerCommand { get; private set; }

        public static ICommand AddToBooklistCommand { get; private set; }

        public static ICommand RemoveFromBooklistCommand { get; private set; }

        public static ICommand EditBookCommand { get; set; }

        public static ICommand DeleteBookCommand { get; set; }

        public static ICommand FindBookInfoCommand { get; private set; }

        public static ICommand ShowBookInfoCommand { get; private set; }

        private static ICommand CreateCommand(Action<Book> bookAction)
        {
            return new ActionCommand(obj => bookAction((Book)obj));
        }

        private static Action<Book> CreateGuardedAction(Action<Book> bookAction)
        {
            return new Action<Book>(book => StorageGuard(() => bookAction(book)));
        }

        private static void StorageGuard(Action bookAction)
        {
            if (!StorageManager.Instance.IsReady)
            {
                DialogService.ShowDialog(
                    "The drive your bookshelf resides in is not ready. Please plug in the removable drive, and try again.",
                    "Drive Not Ready");
            }
            else
            {
                bookAction();
            }
        }

        private static void OpenPinnedBook(Book book)
        {
            // Steps for openning a book from cache:
            // 1. Check if the file exists
            // 2. Open the book directly if it exists in cache
            // 3. Open the book in storage location if it doesn't exist in cache

            if (File.Exists(book.FileName))
            {
                Process.Start(book.FileName);
                App.HomeViewModel.OnBookOpened(book);
            }
            else
            {
                OpenBook(BookManager.Instance.Books.First(b => b.Id == book.Id));
            }
        }

        private static void OpenBook(Book book)
        {
            // Steps for openning a book from storage location:
            // 1. Check if the drive is ready
            // 2. Prompt the user when the drive is not ready
            // 3. Check if the file exists
            // 4. Prompt the user a list of actions to take when book not exists
            // 5. Open the book from its location

            StorageGuard(
                () =>
                {
                    var path = book.GetPath();

                    if (File.Exists(path))
                    {
                        Process.Start(path);
                        App.HomeViewModel.OnBookOpened(book);
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
                                () => BookManager.Instance.Remove(book)),
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
                                            var changedBook = book.Clone();
                                            changedBook.Category = Path.GetFileName(directory);
                                            changedBook.FileName = Path.GetFileName(fileName);

                                            BookManager.Instance.Update(book, changedBook);
                                            Process.Start(fileName);
                                            App.HomeViewModel.OnBookOpened(book);
                                        },
                                        StorageManager.Instance.RootDirectory);
                                }));
                    }
                });
        }
    }
}

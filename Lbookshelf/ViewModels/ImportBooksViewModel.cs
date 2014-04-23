using Lapps.Utils;
using Lbookshelf.Business;
using Lbookshelf.Models;
using Lbookshelf.Utils;
using Microsoft.Expression.Interactivity.Core;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Lapps.Utils.Collections;

namespace Lbookshelf.ViewModels
{
    public class ImportBooksViewModel : ObservableObject
    {
        public ImportBooksViewModel()
        {
            _sourcePaths = new List<string>();
            Books = new ObservableCollection<Book>();

            DeleteSelectedBookCommand = new ActionCommand(
                () =>
                {
                    var index = Books.IndexOf(SelectedBook);
                    Books.RemoveAt(index);
                    _sourcePaths.RemoveAt(index);
                });

            ChooseCommand = new ActionCommand(
                () =>
                {
                    DialogService.ShowOpenFileDialog(
                        async fileNames =>
                        {
                            IsPerformingAction = true;

                            foreach (var fileName in fileNames)
                            {
                                // We need to skip duplicate files.

                                if (!_sourcePaths.Contains(fileName))
                                {
                                    _sourcePaths.Add(fileName);
                                    Books.Add(await ReadPdfMetadataAsync(fileName));
                                }
                            }

                            IsPerformingAction = false;
                        });
                });

            ImportCommand = new ActionCommand(
                async () =>
                {
                    if (!StorageManager.Instance.IsReady)
                    {
                        DialogService.ShowDialog(
                            "The drive your bookshelf resides in is not ready. Please plug in the removable drive, and try again.",
                            "Drive Not Ready");
                    }
                    else
                    {
                        // We need to provide a CheckBox for users to choose to skip/overwrite
                        // if the books to be imported already exist in the library.

                        // Here're the steps:
                        // 1. Clear selected book because it might be removed from the import list.
                        // 2. Add each book asynchronously.
                        // 3. Skip any book if it already exists in the library.
                        // 4. Remove the imported book from the import list.

                        IsPerformingAction = true;

                        SelectedBook = null;

                        for (int i = Books.Count - 1; i >= 0; i--)
                        {
                            var book = Books[i];

                            if (!BookManager.Instance.Exists(book))
                            {
                                await BookManager.Instance.AddAsync(_sourcePaths[i], book);
                                App.HomeViewModel.OnBookAdded(book);

                                _sourcePaths.RemoveAt(i);
                                Books.RemoveAt(i);
                            }
                        }

                        IsPerformingAction = false;
                    }
                });

            ClearCommand = new ActionCommand(
                () =>
                {
                    SelectedBook = null;
                    Books.Clear();
                    _sourcePaths.Clear();
                });
        }

        private List<string> _sourcePaths;

        public ObservableCollection<Book> Books { get; set; }

        private Book _selectedBook;
        public Book SelectedBook
        {
            get { return _selectedBook; }
            set
            {
                if (_selectedBook != value)
                {
                    _selectedBook = value;
                    RaisePropertyChanged();
                }
            }
        }

        public ICommand DeleteSelectedBookCommand { get; private set; }

        public ICommand ChooseCommand { get; private set; }

        private bool _isPerformingAction;
        public bool IsPerformingAction
        {
            get { return _isPerformingAction; }
            set
            {
                if (_isPerformingAction != value)
                {
                    _isPerformingAction = value;
                    RaisePropertyChanged();
                }
            }
        }

        public ICommand ImportCommand { get; private set; }

        public ICommand ClearCommand { get; private set; }

        private async Task<Book> ReadPdfMetadataAsync(string fileName)
        {
            var book = new Book
            {
                Id = IdGenerator.Global.Next(),
                Title = Path.GetFileNameWithoutExtension(fileName),
                Authors = new[] { DefaultPropertyValues.Author },
                Category = DefaultPropertyValues.Category,
                Publisher = DefaultPropertyValues.Publisher,
                Thumbnail = DefaultPropertyValues.Thumbnail
            };

            var info = await Task.Run(() =>
                {
                    try
                    {
                        var pdf = PdfReader.Open(fileName, PdfDocumentOpenMode.InformationOnly);
                        return Tuple.Create(pdf.Info.Title, pdf.Info.Author);
                    }
                    catch (PdfReaderException)
                    {
                        return null;
                    }
                });

            if (info != null)
            {
                if (!String.IsNullOrWhiteSpace(info.Item1))
                {
                    book.Title = info.Item1;
                }

                if (!String.IsNullOrWhiteSpace(info.Item2))
                {
                    book.Authors = new[] { info.Item2 };
                }
            }

            return book;
        }
    }
}

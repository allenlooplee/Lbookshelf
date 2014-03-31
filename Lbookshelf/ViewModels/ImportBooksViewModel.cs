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
                            IsReadingPdfMetadata = true;

                            foreach (var fileName in fileNames)
                            {
                                _sourcePaths.Add(fileName);
                                Books.Add(await ReadPdfMetadataAsync(fileName));
                            }

                            IsReadingPdfMetadata = false;
                        });
                });

            ImportCommand = new ActionCommand(
                () =>
                {
                    if (!StorageManager.Instance.IsReady)
                    {
                        DialogService.ShowDialog(
                            "The drive your bookshelf resides in is not ready. Please plug in the removable drive, and try again.",
                            "Drive Not Ready");
                    }
                    else
                    {
                        // Books that already exist in the library will be skipped silently.
                        // But we need a graceful way to inform the user.
                        _sourcePaths.Zip(Books, (sourcePath, book) => Tuple.Create(sourcePath, book))
                        .Where(t => !BookManager.Instance.Exists(t.Item2))
                        .ForEach(
                            t =>
                            {
                                // Enable await for the completion of the underlying operations
                                BookManager.Instance.Add(t.Item1, t.Item2);

                                App.HomeViewModel.OnBookAdded(t.Item2);
                            });

                        CleanUp();
                    }
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

        private bool _isReadingPdfMetadata;
        public bool IsReadingPdfMetadata
        {
            get { return _isReadingPdfMetadata; }
            set
            {
                if (_isReadingPdfMetadata != value)
                {
                    _isReadingPdfMetadata = value;
                    RaisePropertyChanged();
                }
            }
        }

        public ICommand ImportCommand { get; private set; }

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

        private void CleanUp()
        {
            _sourcePaths.Clear();
            Books.Clear();
            SelectedBook = null;
        }
    }
}

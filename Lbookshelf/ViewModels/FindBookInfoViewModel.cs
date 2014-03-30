using Lapps.Utils;
using Lbookshelf.Business;
using Lbookshelf.Models;
using Lbookshelf.Services;
using Lbookshelf.Utils;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Lapps.Utils.Collections;

namespace Lbookshelf.ViewModels
{
    public class FindBookInfoViewModel : ObservableObject
    {
        public FindBookInfoViewModel()
        {
            Books = new ObservableCollection<Book>();

            _bookService = GetBookService();
            _bookService.PropertyChanged +=
                (o, e) =>
                {
                    if (e.PropertyName == "Status")
                    {
                        RaisePropertyChanged("Status");
                    }
                    else if (e.PropertyName == "Message")
                    {
                        RaisePropertyChanged("Message");
                    }
                };
        }

        public ObservableCollection<Book> Books { get; private set; }

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

        public WorkStatus Status
        {
            get { return _bookService.Status; }
        }

        public string Message
        {
            get { return _bookService.Message; }
        }

        public async void FindBookInfo(string title)
        {
            Books.AddRange(await _bookService.FindBookInfoAsync(title));
            SelectedBook = Books.FirstOrDefault();
        }

        private IBookService GetBookService()
        {
            if (String.IsNullOrWhiteSpace(SettingManager.Default.BookService) ||
                SettingManager.Default.BookService == "Google Books API")
            {
                return new GoogleBooks();
            }
            else
            {
                return new DoubanBooks();
            }
        }

        private IBookService _bookService;
    }
}

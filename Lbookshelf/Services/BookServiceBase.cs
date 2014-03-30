using Lapps.Utils;
using Lbookshelf.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Lbookshelf.Services
{
    public abstract class BookServiceBase : ObservableObject, IBookService
    {
        public BookServiceBase()
        {
            Status = WorkStatus.Created;
        }

        public string Name { get; protected set; }

        private WorkStatus _status;
        public WorkStatus Status
        {
            get { return _status; }
            protected set
            {
                if (_status != value)
                {
                    _status = value;
                    RaisePropertyChanged();
                }
            }
        }

        private string _message;
        public string Message
        {
            get { return _message; }
            protected set
            {
                if (_message != value)
                {
                    _message = value;
                    RaisePropertyChanged();
                }
            }
        }

        public async Task<IEnumerable<Book>> FindBookInfoAsync(string keyword)
        {
            Status = WorkStatus.Running;
            Message = String.Format("Contacting {0} for '{1}'...", Name, keyword);

            var http = new HttpClient();

            try
            {
                var results = await http.GetStringAsync(GetRequestUri(keyword));
                var books = ParseBookInfo(results);

                if (books.Length != 0)
                {
                    Status = WorkStatus.RanToCompletion;
                    Message = null;
                }
                else
                {
                    Status = WorkStatus.NoResults;
                    Message = String.Format("Oops! {0} doesn't know this book. Please try another book.", Name);
                }

                return books;
            }
            catch (HttpRequestException)
            {
                Status = WorkStatus.Faulted;
                Message = String.Format("Oops! {0} is temporarily unavailable. Please try again later.", Name);

                return new Book[0];
            }
            finally
            {
                http.Dispose();
            }
        }

        protected abstract string GetRequestUri(string keyword);

        protected abstract Book[] ParseBookInfo(string bookInfo);
    }
}

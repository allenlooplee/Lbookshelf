using Lbookshelf.Business;
using Lbookshelf.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lbookshelf.Utils;
using System.Net.Http;
using System.Web;

namespace Lbookshelf.Services
{
    public class GoogleBooks : BookServiceBase
    {
        public GoogleBooks()
        {
            Name = "Google Books API";
        }

        protected override string GetRequestUri(string keyword)
        {
            return String.Format("https://www.googleapis.com/books/v1/volumes?q={0}", HttpUtility.UrlEncode(keyword));
        }

        protected override Book[] ParseBookInfo(string bookInfo)
        {
            var json = JObject.Parse(bookInfo);
            var books = json["items"];

            if (books == null)
            {
                return new Book[0];
            }
            else
            {
                return books.Children()["volumeInfo"]
                    .Select(book => new Book
                    {
                        Isbn = TryGetIsbn13(book["industryIdentifiers"]),
                        Title = book["title"].ValueOrDefault<string>(),
                        Authors = book["authors"].ValuesOrDefault<string>().ToArray(),
                        Category = DefaultPropertyValues.Category,
                        Publisher = book["publisher"].ValueOrDefault<string>(),
                        PublishedDate = book["publishedDate"].ValueAsDateTime(),
                        Description = book["description"].ValueOrDefault<string>(),
                        Thumbnail = TryGetThumbnail(book["imageLinks"])
                    })
                    .ToArray();
            }
        }

        private string TryGetIsbn13(JToken jtoken)
        {
            string isbn13 = null;

            if (jtoken != null)
            {
                var found = jtoken.FirstOrDefault(i => i["type"].Value<string>() == "ISBN_13");
                if (found != null)
                {
                    isbn13 = found["identifier"].Value<string>();
                }
            }

            return isbn13;
        }

        private string TryGetThumbnail(JToken jtoken)
        {
            string thumbnail = DefaultPropertyValues.Thumbnail;

            if (jtoken != null)
            {
                thumbnail = jtoken["thumbnail"].Value<string>();
            }

            return thumbnail;
        }
    }
}

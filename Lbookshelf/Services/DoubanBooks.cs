using Lbookshelf.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Lbookshelf.Utils;
using Lbookshelf.Business;

namespace Lbookshelf.Services
{
    public class DoubanBooks : BookServiceBase
    {
        public DoubanBooks()
        {
            Name = "Douban Books API";
        }

        protected override string GetRequestUri(string keyword)
        {
            return String.Format("https://api.douban.com/v2/book/search?q={0}", HttpUtility.UrlEncode(keyword));
        }

        protected override Book[] ParseBookInfo(string bookInfo)
        {
            var json = JObject.Parse(bookInfo);
            var books = json["books"];

            if (books == null)
            {
                return new Book[0];
            }
            else
            {
                return books
                    .Select(book => new Book
                    {
                        Id = IdGenerator.Local.Next(),
                        Isbn = book["isbn13"].ValueOrDefault<string>(),
                        Title = book["title"].ValueOrDefault<string>(),
                        Authors = book["author"].ValuesOrDefault<string>().ToArray(),
                        Category = DefaultPropertyValues.Category,
                        Publisher = book["publisher"].ValueOrDefault<string>(),
                        PublishedDate = book["pubdate"].ValueAsDateTime(),
                        Description = CleanNewLineChars(book["summary"].ValueOrDefault<string>()),
                        Thumbnail = book["image"].ValueOrDefault<string>()
                    })
                    .ToArray();
            }
        }

        private string CleanNewLineChars(string text)
        {
            if (String.IsNullOrWhiteSpace(text))
            {
                return null;
            }
            else
            {
                return text.Replace("\n", "");
            }
        }
    }
}

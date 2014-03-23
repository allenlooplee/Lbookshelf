using Lbookshelf.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lbookshelf.Utils
{
    public static class ExtensionMethods
    {
        #region Collections

        public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            foreach (var item in collection)
            {
                action(item);
            }
        }

        public static IEnumerable<T> ToEnumerable<T>(this T value)
        {
            yield return value;
        }

        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                collection.Add(item);
            }
        }

        #endregion

        #region Models

        public static Book Clone(this Book source)
        {
            return new Book
            {
                Isbn = source.Isbn,
                Category = source.Category,
                Title = source.Title,
                Authors = source.Authors,
                Publisher = source.Publisher,
                PublishedDate = source.PublishedDate,
                Thumbnail = source.Thumbnail,
                Description = source.Description,
                FileName = source.FileName
            };
        }

        public static void MergeChanges(this Book original, Book changed)
        {
            original.Isbn = changed.Isbn;
            original.Category = changed.Category;
            original.Title = changed.Title;
            original.Authors = changed.Authors;
            original.Publisher = changed.Publisher;
            original.PublishedDate = changed.PublishedDate;
            original.Thumbnail = changed.Thumbnail;
            original.Description = changed.Description;

            // The FileName was determined when the book was imported.
            // Changes to it is not supported within the app.
        }

        #endregion

        #region Json

        public static T ValueOrDefault<T>(this JToken jtoken)
        {
            return jtoken != null ? jtoken.Value<T>() : default(T);
        }

        public static DateTime ValueAsDateTime(this JToken jtoken)
        {
            var text = jtoken.ValueOrDefault<string>();

            DateTime result;
            if (!DateTime.TryParse(text, out result))
            {
                result = new DateTime();
            }

            return result;
        }

        public static IEnumerable<T> ValuesOrDefault<T>(this JToken jtoken)
        {
            return jtoken != null ? jtoken.Values<T>() : Enumerable.Empty<T>();
        }

        #endregion
    }
}

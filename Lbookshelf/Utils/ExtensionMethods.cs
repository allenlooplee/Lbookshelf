using Lapps.Data;
using Lbookshelf.Business;
using Lbookshelf.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lbookshelf.Utils
{
    public static class ExtensionMethods
    {
        #region Collections

        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> collection)
        {
            return new ObservableCollection<T>(collection);
        }

        public static bool Remove<T>(this ICollection<T> collection, Func<T, bool> predicate)
        {
            var match = collection.FirstOrDefault(predicate);

            if (match != null)
            {
                return collection.Remove(match);
            }
            else
            {
                return false;
            }
        }

        public static int RemoveAll<T>(this ICollection<T> collection, Func<T, bool> predicate)
        {
            var matches = collection.Where(predicate).ToArray();

            foreach (var match in matches)
            {
                collection.Remove(match);
            }

            return matches.Length;
        }

        public static bool Remove<T>(this IDataCollection<T> collection, Func<T, bool> predicate)
        {
            var result = false;

            var match = collection.AsQueryable().FirstOrDefault(predicate);

            if (match != null)
            {
                collection.Remove(match);

                result = true;
            }

            return result;
        }

        public static int RemoveAll<T>(this IDataCollection<T> collection, Func<T, bool> predicate)
        {
            var matches = collection.AsQueryable().Where(predicate).ToArray();

            foreach (var match in matches)
            {
                collection.Remove(match);
            }

            return matches.Length;
        }

        #endregion

        #region Models

        public static Book Clone(this Book source)
        {
            return new Book
            {
                Id = source.Id,
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
            // The Id is ignore from merging.

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

        public static string GetPath(this Book book)
        {
            return Path.Combine(StorageManager.Instance.RootDirectory, book.Category, book.FileName);
        }

        public static bool IsCached(this Book book)
        {
            return book.FileName.StartsWith("Cache");
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

using Lbookshelf.Models;
using Lbookshelf.Utils;
using Lapps.Utils.Collections;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Lapps.Data;

namespace Lbookshelf.Business
{
    public class BookManager
    {
        private BookManager()
        {
            _books = App.DataStore.GetCollection<Book>(DataCollectionNames.Books);
        }

        private static BookManager _instance = new BookManager();
        public static BookManager Instance
        {
            get { return _instance; }
        }

        private IDataCollection<Book> _books;
        public IEnumerable<Book> Books
        {
            get { return _books.AsQueryable(); }
        }

        public bool Exists(Book book)
        {
            return Books.Contains(book);
        }

        // This method doesn't check if a book already exists, so do this before calling this method.
        public async void Add(string sourcePath, Book book)
        {
            EnsureAuthors(book);

            var fileNameNoExt = GenerateFileNameNoExt(book);

            book.FileName = fileNameNoExt + ".pdf";
            book.Thumbnail = await DownloadThumbnailAsync(book.Thumbnail, fileNameNoExt);

            StorageManager.Instance.Add(sourcePath, book.Category, book.FileName);
            _books.Insert(book);
            DimensionManager.Instance.Add(book);
        }

        // Thumbnail of the book will not be removed due to BitmapImage's exclusive use.
        // We'll need to provide a way for the user to clean up the thumbnail cache in settings.
        public void Remove(Book book)
        {
            StorageManager.Instance.Remove(book.Category, book.FileName);
            _books.Remove(book);
            DimensionManager.Instance.Remove(book);
        }

        public void Update(Book original, Book changed)
        {
            // Move the underlying file if category was changed.
            if (original.Category != changed.Category)
            {
                StorageManager.Instance.Move(original.Category, changed.Category, original.FileName);
            }

            // Get a list of changed group keys and their corresponding dimension before merging the changes of book.
            var changedGroupKeys = DimensionManager.Instance.ComputedDimensions
                .Select(d => Tuple.Create(d, d.GetGroupKey(original), d.GetGroupKey(changed)))
                .Where(t => t.Item2 != t.Item3)
                .ToArray();

            // Copy the new thumbnail and overwrite the existing one if the user has changed it.
            if (original.Thumbnail != changed.Thumbnail)
            {
                var destinationThumbnail = Path.Combine("Images", Path.ChangeExtension(original.FileName, ".jpg"));

                File.Copy(
                    changed.Thumbnail,  // This is actually a full path returned from OpenFileDialog.
                    Path.Combine(Environment.CurrentDirectory, destinationThumbnail),
                    true);

                changed.Thumbnail = destinationThumbnail;
            }

            // Merge changes to original book.
            original.MergeChanges(changed);

            EnsureAuthors(original);

            // Merge changes to database.
            _books.Update(original);

            // Move the book from one group to another for each changed group key in the corresponding dimension.
            changedGroupKeys.ForEach(t => t.Item1.ChangeGroup(t.Item2, t.Item3, original));
        }

        /// <summary>
        /// The file name consists of the title and the first author.
        /// </summary>
        private string GenerateFileNameNoExt(Book book)
        {
            return CleanInvalidFileNameChars(book.Title + " - " + book.Authors[0]);
        }

        // Any leading and trailing '.' char will also be removed.
        private string CleanInvalidFileNameChars(string fileName)
        {
            return String.Join("", fileName.Split(Path.GetInvalidFileNameChars())).Trim('.');
        }

        private async Task<string> DownloadThumbnailAsync(string remoteThumbnailUri, string fileNameWithoutExtension)
        {
            var localThumbnailUri = DefaultPropertyValues.Thumbnail;

            if (remoteThumbnailUri != DefaultPropertyValues.Thumbnail)
            {
                localThumbnailUri = Path.Combine("Images", fileNameWithoutExtension + ".jpg");

                if (!File.Exists(localThumbnailUri))
                {
                    using (var webClient = new WebClient())
                    {
                        // Looks like DownloadFileTaskAsync will overwrite existing file.
                        await webClient.DownloadFileTaskAsync(
                            remoteThumbnailUri,
                            Path.Combine(Environment.CurrentDirectory, localThumbnailUri));
                    }
                }
            }

            return localThumbnailUri;
        }

        /// <summary>
        /// This method makes sure that the author list contains at least one value,
        /// or it's in alphabetical order if there're more than one value.
        /// </summary>
        private void EnsureAuthors(Book book)
        {
            if (book.Authors == null || book.Authors.Length == 0)
            {
                book.Authors = new[] { DefaultPropertyValues.Author };
            }
            else if (book.Authors.Length > 1)
            {
                Array.Sort(book.Authors);
            }
        }
    }
}

using Lapps.Data;
using Lbookshelf.Models;
using Lbookshelf.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lapps.Utils.Collections;

namespace Lbookshelf.ViewModels
{
    public class HomeViewModel
    {
        public HomeViewModel()
        {
            RecentlyAdded = RecentlyAddedCollection.AsQueryable().Reverse().Take(NumberOfRecentItems).ToObservableCollection();

            RecentlyOpened = RecentlyOpenedCollection.AsQueryable().Reverse().Take(NumberOfRecentItems).ToObservableCollection();

            Pinned = new ObservableCollection<Book>(PinnedCollection.AsQueryable().Reverse());
        }

        public ObservableCollection<RecentItem> RecentlyAdded { get; private set; }
        private IDataCollection<RecentItem> RecentlyAddedCollection
        {
            get { return App.DataStore.GetCollection<RecentItem>(DataCollectionNames.RecentlyAdded); }
        }

        public ObservableCollection<RecentItem> RecentlyOpened { get; private set; }
        private IDataCollection<RecentItem> RecentlyOpenedCollection
        {
            get { return App.DataStore.GetCollection<RecentItem>(DataCollectionNames.RecentlyOpened); }
        }

        public ObservableCollection<Book> Pinned { get; private set; }
        public bool IsPinned(Book book)
        {
            return Pinned.Contains(book);
        }
        public void PinOrUnpin(Book book)
        {
            if (IsPinned(book))
            {
                Pinned.Remove(book);
                PinnedCollection.Remove(book);
            }
            else
            {
                Pinned.Insert(0, book);
                PinnedCollection.Insert(book);
            }
        }
        private IDataCollection<Book> PinnedCollection
        {
            get { return App.DataStore.GetCollection<Book>(DataCollectionNames.Pinned); }
        }

        /// <summary>
        /// The book will be added to the top. If the count of the list
        /// exceeds a specific number, it will be truncated to fit that number.
        /// It's not possible to add the same book, so there's no need to
        /// check whether the book already exists in the list.
        /// </summary>
        public void OnBookAdded(Book book)
        {
            var recentItem = new RecentItem
            {
                Book = book,
                Timestamp = DateTime.Now
            };

            RecentlyAdded.Insert(0, recentItem);

            if (RecentlyAdded.Count > NumberOfRecentItems)
            {
                RecentlyAdded.RemoveAt(NumberOfRecentItems);
            }

            RecentlyAddedCollection.Insert(recentItem);
        }

        /// <summary>
        /// If the book already exists in the list, it'll be removed first;
        /// then it'll be added back to the top. The list will be truncated
        /// if its count exceeds a specific number.
        /// </summary>
        public void OnBookOpened(Book book)
        {
            RecentlyOpened.Remove(item => item.Book == book);

            var recentItem = new RecentItem
            {
                Book = book,
                Timestamp = DateTime.Now
            };

            RecentlyOpened.Insert(0, recentItem);

            if (RecentlyOpened.Count > NumberOfRecentItems)
            {
                RecentlyOpened.RemoveAt(NumberOfRecentItems);
            }

            RecentlyOpenedCollection.Insert(recentItem);
        }

        public void OnBookRemoved(Book book)
        {
            RecentlyAdded.Remove(item => item.Book == book);
            RecentlyAddedCollection.Remove(item => item.Book == book);

            RecentlyOpened.Remove(item => item.Book == book);
            RecentlyOpenedCollection.RemoveAll(item => item.Book == book);

            if (Pinned.Remove(book))
            {
                PinnedCollection.Remove(book);
            }
        }

        private const int NumberOfRecentItems = 5;
    }
}

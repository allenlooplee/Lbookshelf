﻿using Lbookshelf.Models;
using Lbookshelf.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lbookshelf.ViewModels
{
    public class HomeViewModel
    {
        public HomeViewModel()
        {
            RecentlyAdded = new ObservableCollection<Book>(RecentlyAddedCollection.AsEnumerable());

            RecentlyOpened = new ObservableCollection<Book>(RecentlyOpenedCollection.AsEnumerable());

            Pinned = new ObservableCollection<Book>(PinnedCollection.AsEnumerable().Reverse());
        }

        public ObservableCollection<Book> RecentlyAdded { get; private set; }
        private Ldata.IDataCollection<Book> RecentlyAddedCollection
        {
            get { return App.DataStore.GetCollection<Book>(DataCollectionNames.RecentlyAdded); }
        }

        public ObservableCollection<Book> RecentlyOpened { get; private set; }
        private Ldata.IDataCollection<Book> RecentlyOpenedCollection
        {
            get { return App.DataStore.GetCollection<Book>(DataCollectionNames.RecentlyOpened); }
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
        private Ldata.IDataCollection<Book> PinnedCollection
        {
            get { return App.DataStore.GetCollection<Book>(DataCollectionNames.Pinned); }
        }

        public void OnBookAdded(Book book)
        {
            PushRecentItem(book, RecentlyAdded, RecentlyAddedCollection);
        }

        public void OnBookOpened(Book book)
        {
            PushRecentItem(book, RecentlyOpened, RecentlyOpenedCollection);
        }

        public void OnBookRemoved(Book book)
        {
            RemoveIfExists(book, RecentlyAdded, RecentlyAddedCollection);
            RemoveIfExists(book, RecentlyOpened, RecentlyOpenedCollection);
            RemoveIfExists(book, Pinned, PinnedCollection);
        }

        private const int NumberOfRecentItems = 5;

        private void PushRecentItem(Book recentItem, ObservableCollection<Book> recentItems, Ldata.IDataCollection<Book> recentItemCollection)
        {
            // Remove this recent item if it exists in the list.
            recentItems.Remove(recentItem);

            recentItems.Insert(0, recentItem);

            if (recentItems.Count > NumberOfRecentItems)
            {
                recentItems.RemoveAt(NumberOfRecentItems);
            }

            recentItemCollection.Drop();
            recentItemCollection.Insert(recentItems);
        }

        private void RemoveIfExists(Book item, ObservableCollection<Book> items, Ldata.IDataCollection<Book> itemCollection)
        {
            items.Remove(item);
            itemCollection.Remove(item);
        }
    }
}

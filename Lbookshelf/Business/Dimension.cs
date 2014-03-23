using Lbookshelf.Models;
using Lbookshelf.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lbookshelf.Business
{
    public abstract class Dimension : SortedObservableGroup<string, SortedObservableGroup<string, Book>>
    {
        public Dimension(string key)
        {
            Key = key;
            Elements = new SortedObservableCollection<SortedObservableGroup<string, Book>>((x, y) => x.Key.CompareTo(y.Key));
        }

        public event EventHandler GroupsChanged;
        protected void RaiseGroupsChanged()
        {
            if (GroupsChanged != null)
            {
                GroupsChanged(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Add a book to a specific group.
        /// If a new group is created before adding the book, this method will raise GroupsChanged event.
        /// </summary>
        public virtual void Add(string groupKey, Book book)
        {
            if (InternalAdd(groupKey, book))
            {
                RaiseGroupsChanged();
            }
        }

        public abstract void Add(Book book);

        /// <summary>
        /// Remove a book from a specific group.
        /// If an existing group is removed after removing the book, this method will raise GroupsChanged event.
        /// </summary>
        public virtual void Remove(string groupKey, Book book)
        {
            if (InternalRemove(groupKey, book))
            {
                RaiseGroupsChanged();
            }
        }

        public abstract void Remove(Book book);

        /// <summary>
        /// If the group doesn't exist and is created, this method will return true; otherwise, false.
        /// </summary>
        private bool FindOrCreate(string groupKey, out SortedObservableGroup<string, Book> group)
        {
            var found = Elements.FirstOrDefault(g => g.Key == groupKey);
            var groupCreated = false;

            if (found == null)
            {
                found = SortedObservableGroup.Make<string, Book>(groupKey);
                Elements.Add(found);
                groupCreated = true;
            }

            group = found;
            return groupCreated;
        }

        /// <summary>
        /// If the group is empty and removed, this method will return true; otherwise, false.
        /// </summary>
        private bool RemoveIfEmpty(SortedObservableGroup<string, Book> group)
        {
            var groupRemoved = false;

            if (group.Elements.Count == 0)
            {
                Elements.Remove(group);
                groupRemoved = true;
            }

            return groupRemoved;
        }

        /// <summary>
        /// Add a book to a specific group.
        /// If a new group is created before adding the book,
        /// this method will return true; otherwise, false.
        /// </summary>
        protected bool InternalAdd(string groupKey, Book book)
        {
            SortedObservableGroup<string, Book> group;
            var groupCreated = FindOrCreate(groupKey, out group);
            group.Elements.Add(book);

            return groupCreated;
        }

        /// <summary>
        /// Remove a book from a specific group.
        /// If an existing group is removed after removing the book,
        /// this method will return true; otherwise, false.
        /// </summary>
        protected bool InternalRemove(string groupKey, Book book)
        {
            var group = Elements.First(g => g.Key == groupKey);
            group.Elements.Remove(book);
            var groupRemoved = RemoveIfEmpty(group);

            return groupRemoved;
        }
    }
}

using Lbookshelf.Models;
using Lapps.Utils.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lbookshelf.Utils;
using Lapps.Data;

namespace Lbookshelf.Business
{
    public class BooklistDimension : Dimension
    {
        public BooklistDimension()
            : base("Booklist")
        {
            BooklistCollection.AsEnumerable().ForEach(booklist => Elements.Add(booklist));
        }

        /// <summary>
        /// Add a book to a booklist which will be created if not exists.
        /// </summary>
        public override void Add(string groupKey, Book book)
        {
            var insert = InternalAdd(groupKey, book);
            var group = Elements.First(g => g.Key == groupKey);

            if (insert)
            {
                BooklistCollection.Insert(group);
                RaiseGroupsChanged();
            }
            else
            {
                BooklistCollection.Update(group);
            }
        }

        /// <summary>
        /// By not specifying a booklist, the book will be added to all existing booklists.
        /// GroupsChanged event will not be raised, because no new booklist will be created.
        /// </summary>
        public override void Add(Book book)
        {
            Elements.ForEach(g => g.Elements.Add(book));
            BooklistCollection.Update(Elements);
        }

        /// <summary>
        /// Remove a book from a booklist which will be removed if then empty.
        /// </summary>
        public override void Remove(string groupKey, Book book)
        {
            var group = Elements.First(g => g.Key == groupKey);
            var remove = InternalRemove(groupKey, book);

            if (remove)
            {
                BooklistCollection.Remove(group);
                RaiseGroupsChanged();
            }
            else
            {
                BooklistCollection.Update(group);
            }
        }

        /// <summary>
        /// By not specifying a booklist, the book will be removed from all containing booklist.
        /// GroupsChanged event will be raised if any of the existing booklists is removed.
        /// </summary>
        public override void Remove(Book book)
        {
            // Remove the book and obtain the booklists from which the book was successfully removed.
            var groups = Elements.Where(g => g.Elements.Remove(book)).ToArray();

            // Update non-empty booklist to database.
            BooklistCollection.Update(groups.Where(g => g.Elements.Count > 0));

            // If there's any empty booklist, it'll be removed from the database and the dimension.
            var emptyGroups = groups.Where(g => g.Elements.Count == 0);
            if (emptyGroups.Count() > 0)
            {
                BooklistCollection.Remove(emptyGroups);
                emptyGroups.ForEach(g => Elements.Remove(g));
                RaiseGroupsChanged();
            }
        }

        private IDataCollection<SortedObservableGroup<string, Book>> BooklistCollection
        {
            get { return App.DataStore.GetCollection<SortedObservableGroup<string, Book>>(DataCollectionNames.Booklists); }
        }
    }
}

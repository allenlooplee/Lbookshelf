using Lbookshelf.Models;
using Lbookshelf.Utils;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lbookshelf.Business
{
    /// <summary>
    /// Computed dimension is a dimension that a group key can be computed from a book.
    /// When adding or removing a book, a computed dimension knows what group to add to or remove from.
    /// </summary>
    public abstract class ComputedDimension : Dimension
    {
        public ComputedDimension(string key)
            : base(key)
        {

        }

        public abstract string GetGroupKey(Book book);

        /// <summary>
        /// Add the book to the corresponding group, and raise GroupsChanged event if needed.
        /// </summary>
        public override void Add(Book book)
        {
            // The below code will not going to work when we decide to add a dimension for authors!
            // Don't worry! We have solution. We inspect the type of the property.
            // If it's a string, then we know the object will exist in only one group.
            // If it's a string array, then we know the object will exist in multiple groups.
            // For the latter case, we'll add the same object into multiple groups.
            // For the removal case, we'll remove the same object from multiple groups.

            Add(GetGroupKey(book), book);
        }

        /// <summary>
        /// Remove the book from the corresponding group, and raise GroupsChanged event if needed.
        /// </summary>
        public override void Remove(Book book)
        {
            // The below code will not going to work when we decide to add a dimension for authors!
            // The solution is written inside the Add method above.

            Remove(GetGroupKey(book), book);
        }

        /// <summary>
        /// Move the book from one group to another, and raise GroupsChanged event if needed.
        /// </summary>
        public virtual void ChangeGroup(string sourceGroupKey, string destinationGroupKey, Book book)
        {
            var sourceGroupRemoved = InternalRemove(sourceGroupKey, book);
            var destinationGroupCreated = InternalAdd(destinationGroupKey, book);

            if (sourceGroupRemoved || destinationGroupCreated)
            {
                RaiseGroupsChanged();
            }
        }
    }
}

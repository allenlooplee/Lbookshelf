using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace Lbookshelf.Utils
{
    public class SortedObservableCollection<T> : ObservableCollection<T>
    {
        public SortedObservableCollection()
            : this(null)
        {

        }

        public SortedObservableCollection(Comparison<T> comparison)
        {
            if (comparison != null)
            {
                _comparer = DelegateComparer.Make(comparison);
            }
            else
            {
                _comparer = Comparer<T>.Default;
            }
        }

        /// <summary>
        /// Insert the item to a position based on the sorting order.
        /// The input index parameter will be ignore.
        /// If the item to insert already exists in the collection, then it'll be ignore.
        /// </summary>
        protected override void InsertItem(int index, T item)
        {
            index = Array.BinarySearch<T>(Items.ToArray<T>(), item, _comparer);

            if (index < 0)
            {
                base.InsertItem(~index, item);
            }
        }

        private IComparer<T> _comparer;
    }
}

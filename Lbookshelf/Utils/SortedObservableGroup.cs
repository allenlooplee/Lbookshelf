using Lapps.Utils.Collections;
using Lbookshelf.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lbookshelf.Utils
{
    public class SortedObservableGroup<TKey, TElement>
    {
        public TKey Key { get; set; }

        public SortedObservableCollection<TElement> Elements { get; set; }
    }

    public static class SortedObservableGroup
    {
        public static SortedObservableGroup<TKey, TElement> Make<TKey, TElement>(TKey key, Comparison<TElement> comparison = null)
        {
            return Make(key, Enumerable.Empty<TElement>(), comparison);
        }

        public static SortedObservableGroup<TKey, TElement> Make<TKey, TElement>(IGrouping<TKey, TElement> grouping, Comparison<TElement> comparison = null)
        {
            return Make(grouping.Key, grouping, comparison);
        }

        public static SortedObservableGroup<TKey, TElement> Make<TKey, TElement>(TKey key, IEnumerable<TElement> elements, Comparison<TElement> comparison = null)
        {
            var sortedElements = new SortedObservableCollection<TElement>(comparison);
            sortedElements.AddRange(elements);

            return new SortedObservableGroup<TKey, TElement>
            {
                Key = key,
                Elements = sortedElements
            };
        }
    }
}

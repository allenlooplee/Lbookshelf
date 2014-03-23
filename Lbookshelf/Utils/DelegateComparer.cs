using System;
using System.Collections.Generic;

namespace Lbookshelf.Utils
{
    public class DelegateComparer<T> : IComparer<T>
    {
        public DelegateComparer(Comparison<T> comparison)
        {
            _comparison = comparison;
        }

        private Comparison<T> _comparison;

        public int Compare(T x, T y)
        {
            return _comparison(x, y);
        }
    }

    public static class DelegateComparer
    {
        public static DelegateComparer<T> Make<T>(Comparison<T> comparison)
        {
            return new DelegateComparer<T>(comparison);
        }
    }
}

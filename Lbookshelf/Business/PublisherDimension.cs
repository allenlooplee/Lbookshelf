using Lbookshelf.Models;
using Lbookshelf.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lbookshelf.Business
{
    public class PublisherDimension : ComputedDimension
    {
        public PublisherDimension()
            : base("Publisher")
        {
            BookManager.Instance.Books
            .GroupBy(b => b.Publisher)
            .Select(g => SortedObservableGroup.Make(g))
            .ForEach(Elements.Add);
        }

        public override string GetGroupKey(Book book)
        {
            return book.Publisher;
        }
    }
}

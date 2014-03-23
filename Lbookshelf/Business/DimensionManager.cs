using Lbookshelf.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lbookshelf.Utils;

namespace Lbookshelf.Business
{
    public class DimensionManager
    {
        private DimensionManager()
        {
            SupportedDimensions = new Dimension[]
            {
                new CategoryDimension(),
                new PublisherDimension(),
                new BooklistDimension()
            };

            ComputedDimensions = SupportedDimensions.OfType<ComputedDimension>().ToArray();
        }

        private static DimensionManager _instance = new DimensionManager();
        public static DimensionManager Instance
        {
            get { return _instance; }
        }

        public Dimension[] SupportedDimensions { get; private set; }

        public ComputedDimension[] ComputedDimensions { get; private set; }

        /// <summary>
        /// Add the book to the corresponding group of each computed dimension.
        /// </summary>
        public void Add(Book book)
        {
            ComputedDimensions.ForEach(d => d.Add(book));
        }

        /// <summary>
        /// Remove the book from the corresponding group(s) of each supported dimension.
        /// </summary>
        public void Remove(Book book)
        {
            SupportedDimensions.ForEach(d => d.Remove(book));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lbookshelf.Business
{
    public class BookOpenedException : ApplicationException
    {
        public BookOpenedException(string path, Exception innerException)
            : base(String.Format("This book is currently opened: {0}. Please close it and try again.", path), innerException)
        {
            
        }
    }
}

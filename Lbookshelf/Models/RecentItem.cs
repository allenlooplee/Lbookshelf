using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lbookshelf.Models
{
    public class RecentItem
    {
        public Book Book { get; set; }

        public DateTime Timestamp { get; set; }
    }
}

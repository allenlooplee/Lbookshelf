using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Lbookshelf.Utils
{
    public class FindBookInfoMenuItem : MenuItem
    {
        public FindBookInfoMenuItem()
        {
            Header = "Find book info";
            Items.Add(new MenuItem { Header = "Google Books API" });
            Items.Add(new MenuItem { Header = "Douban Books API" });
        }
    }
}

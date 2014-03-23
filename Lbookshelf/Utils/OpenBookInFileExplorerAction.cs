using Lbookshelf.Business;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interactivity;

namespace Lbookshelf.Utils
{
    public class OpenBookInFileExplorerAction : OpenBookAction
    {
        protected override void OpenBook(string path)
        {
            Process.Start("explorer.exe", "/select, " + path);
        }
    }
}

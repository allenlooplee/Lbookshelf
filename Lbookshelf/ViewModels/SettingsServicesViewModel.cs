using Microsoft.Expression.Interactivity.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Lbookshelf.Utils;

namespace Lbookshelf.ViewModels
{
    public class SettingsServicesViewModel
    {
        public SettingsServicesViewModel()
        {
            BookServices = new[] { "Google Books API", "Douban Books API" };

            if (!String.IsNullOrWhiteSpace(SettingManager.Default.BookService))
            {
                SelectedBookService = SettingManager.Default.BookService;
            }
            else
            {
                SelectedBookService = BookServices[0];
            }
        }

        public string[] BookServices { get; private set; }

        private string _selectedBookService;
        public string SelectedBookService
        {
            get { return _selectedBookService; }
            set
            {
                if (_selectedBookService != value)
                {
                    _selectedBookService = value;

                    SettingManager.Default.BookService = _selectedBookService;
                    SettingManager.Default.Save();
                }
            }
        }
    }
}

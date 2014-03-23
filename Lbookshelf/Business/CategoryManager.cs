using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lbookshelf.Utils;

namespace Lbookshelf.Business
{
    public class CategoryManager
    {
        private CategoryManager()
        {
            Categories = new ObservableCollection<string>();

            Directory.GetDirectories(LibraryManager.Instance.RootDirectory)
                .Select(Path.GetFileName)
                //.OrderBy(n => n)
                .ForEach(Categories.Add);
        }

        private static CategoryManager _instance = new CategoryManager();
        public static CategoryManager Instance
        {
            get { return _instance; }
        }

        public ObservableCollection<string> Categories { get; private set; }

        public void AddToSet(string category)
        {
            if (!Categories.Contains(category))
            {
                Directory.CreateDirectory(GetCategoryDirectory(category));
                Categories.Add(category);
            }
        }

        public void RemoveIfEmpty(string category)
        {
            var directory = GetCategoryDirectory(category);
            if (Directory.GetFiles(directory).Length == 0)
            {
                Directory.Delete(directory);
                Categories.Remove(category);
            }
        }

        private string GetCategoryDirectory(string category)
        {
            return Path.Combine(LibraryManager.Instance.RootDirectory, category);
        }
    }
}

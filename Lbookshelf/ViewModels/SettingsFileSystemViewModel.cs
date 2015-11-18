using Lbookshelf.Business;
using Microsoft.Expression.Interactivity.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Configuration;
using Lbookshelf.Utils;
using Lbookshelf.Models;
using Lapps.Utils;
using Lapps.Utils.Collections;

namespace Lbookshelf.ViewModels
{
    public class SettingsFileSystemViewModel : ObservableObject
    {
        public SettingsFileSystemViewModel()
        {
            CleanCommand = new ActionCommand(
                () =>
                {
                    var used = BookManager.Instance.Books.Select(b => Path.Combine(Environment.CurrentDirectory, b.Thumbnail));
                    var cached = Directory
                        .GetFiles(Path.Combine(Environment.CurrentDirectory, "Images"))
                        .Where(p => Path.GetFileName(p) != "DefaultThumbnail.jpg");
                    var disused = cached.Except(used).ToArray();

                    if (disused.Length > 0)
                    {
                        disused.ForEach(p => File.Delete(p));
                        DialogService.ShowDialog(String.Format("{0} disused thumbnails were removed.", disused.Length));
                    }
                    else
                    {
                        DialogService.ShowDialog("All thumbnails are in use.");
                    }
                });
        }

        public string RootDirectory
        {
            get { return StorageManager.Instance.RootDirectory; }
            set
            {
                StorageManager.Instance.RootDirectory = value;

                SettingManager.Default.RootDirectory = StorageManager.Instance.RootDirectory;
                SettingManager.Default.Save();
            }
        }

        public ICommand CleanCommand { get; private set; }
    }
}

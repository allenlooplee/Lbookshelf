using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections.ObjectModel;
using Lbookshelf.Utils;
using Microsoft.VisualBasic.FileIO;

namespace Lbookshelf.Business
{
    public class StorageManager
    {
        private StorageManager()
        {
            // The root directory will be set to My Documents\Books if it's not been set in the user settings.
            // Otherwise, it'll be set to the what's set in the user settings.
            if (String.IsNullOrWhiteSpace(SettingManager.Default.RootDirectory))
            {
                SettingManager.Default.RootDirectory =
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Books");
                SettingManager.Default.Save();
            }

            _rootDirectory = SettingManager.Default.RootDirectory;

            if (IsReady && !Directory.Exists(_rootDirectory))
            {
                Directory.CreateDirectory(_rootDirectory);
            }
        }

        private static StorageManager _instance = new StorageManager();
        public static StorageManager Instance
        {
            get { return _instance; }
        }

        private string _rootDirectory;
        // This property will ignore null or empty string.
        public string RootDirectory 
        {
            get { return _rootDirectory; }
            set
            {
                if (!String.IsNullOrWhiteSpace(value) && _rootDirectory != value)
                {
                    _rootDirectory = value;
                }
            }
        }

        public bool IsReady
        {
            get { return new DriveInfo(RootDirectory).IsReady; }
        }

        public string[] Categories
        {
            get
            {
                if (IsReady)
                {
                    return Directory.GetDirectories(RootDirectory).Select(Path.GetFileName).ToArray();
                }
                else
                {
                    return new string[0];
                }
            }
        }

        public async Task AddAsync(string sourcePath, string category, string fileName)
        {
            if (!IsReady)
            {
                // Do nothing because your drive is not ready.
                // You'll run into this case when you point
                // your library to a removable drive.
                return;
            }

            var destinationDirectory = AddToSet(category);
            var destinationPath = Path.Combine(destinationDirectory, fileName);
            await CopyAsync(sourcePath, destinationPath);
        }

        // If the target file doesn't not exist, Remove will do nothing regarding
        // to the file. The category will still be removed if it exists and is empty.
        public void Remove(string category, string fileName)
        {
            if (!IsReady)
            {
                // Do nothing because your drive is not ready.
                // You'll run into this case when you point
                // your library to a removable drive.
                return;
            }

            var directory = GetDirectory(category);
            var path = Path.Combine(directory, fileName);

            if (File.Exists(path))
            {
                try
                {
                    File.Delete(path);
                }
                catch (IOException ex)
                {
                    throw new BookOpenedException(path, ex);
                }
            }

            RemoveIfEmpty(directory);
        }

        // There're 4 scenarios to consider:
        // 1. The source file exists, but the destination file doesn't. (normal move)
        // 2. The source file doesn't exist, but the destination file does. (relocate)
        // 3. Both of them don't exist. (change category, but the underlying file is missing)
        // *4. Both of them exist. (file names conflict, overwrite?)
        //
        // This method handles the first two. For the first scenario, this method performs
        // a normal move. For the second, the method will do nothing. In both scenarios,
        // the source category will be removed in the end if it's empty. For the third
        // scenario, the user will be prompted to take action when (s)he tries to open it.
        public void Move(string sourceCategory, string destinationCategory, string fileName)
        {
            if (!IsReady)
            {
                // Do nothing because your drive is not ready.
                // You'll run into this case when you point
                // your library to a removable drive.
                return;
            }

            var sourceDirectory = GetDirectory(sourceCategory);
            var sourcePath = Path.Combine(sourceDirectory, fileName);
            var destinationDirectory = AddToSet(destinationCategory);
            var destinationPath = Path.Combine(destinationDirectory, fileName);

            if (File.Exists(sourcePath) && !File.Exists(destinationPath))
            {
                try
                {
                    File.Move(sourcePath, destinationPath);
                }
                catch (IOException ex)
                {
                    // If the destination cateogry was just created,
                    // it would be an empty category that should be removed.
                    RemoveIfEmpty(destinationDirectory);

                    throw new BookOpenedException(sourcePath, ex);
                }
            }

            RemoveIfEmpty(sourceDirectory);
        }

        private string GetDirectory(string category)
        {
            return Path.Combine(RootDirectory, category);
        }

        private string AddToSet(string category)
        {
            var directory = GetDirectory(category);

            if (!Directory.Exists(directory))
            {
                // Create the underlying directory when it doesn't exist.
                Directory.CreateDirectory(directory);
            }

            return directory;
        }

        private void RemoveIfEmpty(string directory)
        {
            if (Directory.Exists(directory) && Directory.GetFiles(directory).Length == 0)
            {
                // Delete the underlying directory when there's no file in it.
                Directory.Delete(directory);
            }
        }

        internal static async Task CopyAsync(string sourceFileName, string destinationFileName)
        {
            using (var sourceStream = File.Open(sourceFileName, FileMode.Open))
            using (var destinationStream = File.Create(destinationFileName))
            {
                await sourceStream.CopyToAsync(destinationStream);
            }
        }
    }
}

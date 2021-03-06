﻿using Lbookshelf.Models;
using Lbookshelf.Utils;
using Lbookshelf.ViewModels;
using Lapps.Data.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Lapps.Data;

namespace Lbookshelf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static JsonDataStore _dataStore;
        public static IDataStore DataStore
        {
            get
            {
                if (_dataStore == null)
                {
                    _dataStore = new JsonDataStore("db");
                    _dataStore.RegisterPartitionSelector<SortedObservableGroup<string, Book>>(DataCollectionNames.Booklists, obj => obj.Key);
                }

                return _dataStore;
            }
        }

        private static BrowseBooksViewModel _browseBooksViewModel;
        public static BrowseBooksViewModel BrowseBooksViewModel
        {
            get
            {
                if (_browseBooksViewModel == null)
                {
                    _browseBooksViewModel = new BrowseBooksViewModel();
                }

                return _browseBooksViewModel;
            }
        }

        private static HomeViewModel _homeViewModel;
        public static HomeViewModel HomeViewModel
        {
            get
            {
                if (_homeViewModel == null)
                {
                    _homeViewModel = new HomeViewModel();
                }

                return _homeViewModel;
            }
        }
    }
}

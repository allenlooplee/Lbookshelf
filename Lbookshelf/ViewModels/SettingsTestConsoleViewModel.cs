﻿using Lbookshelf.Models;
using Lbookshelf.Utils;
using Microsoft.Expression.Interactivity.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Lapps.Utils.Collections;

namespace Lbookshelf.ViewModels
{
    public class SettingsTestConsoleViewModel
    {
        public SettingsTestConsoleViewModel()
        {
            ApplyCommand = new ActionCommand(
                () =>
                {
                    var bookCollection = App.DataStore.GetCollection<Book>(DataCollectionNames.Books);
                    var books = bookCollection.AsQueryable().ToArray();
                    for (int i = 0; i < books.Length; i++)
                    {
                        books[i].Id = i + 1;
                    }
                    books.ForEach(bookCollection.Update);
                    DialogService.ShowDialog("Applied " + books.Length.ToString() + " books.");
                });
        }

        public ICommand ApplyCommand { get; private set; }
    }
}

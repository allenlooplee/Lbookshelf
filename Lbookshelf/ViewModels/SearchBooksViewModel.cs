using Lbookshelf.Business;
using Lbookshelf.Models;
using Microsoft.Expression.Interactivity.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Lbookshelf.Utils;

namespace Lbookshelf.ViewModels
{
    public class SearchBooksViewModel : ObservableObject
    {
        public SearchBooksViewModel()
        {
            RecentKeywords =
                new ObservableCollection<string>(
                    RecentKeywordCollection.AsEnumerable().Take(NumberOfRecentItems));

            SearchCommand = new ActionCommand(() => Search(Keywords));
            QuickSearchCommand = new ActionCommand(
                keywords =>
                {
                    Keywords = (string)keywords;
                    Search(Keywords);
                });
        }

        private string _keywords;
        public string Keywords
        {
            get { return _keywords; }
            set
            {
                if (_keywords != value)
                {
                    _keywords = value;
                    RaisePropertyChanged();
                }
            }
        }

        private Book[] _results;
        public Book[] Results
        {
            get { return _results; }
            set
            {
                if (_results != value)
                {
                    _results = value;
                    RaisePropertyChanged();
                }
            }
        }

        public ObservableCollection<string> RecentKeywords { get; private set; }

        public ICommand SearchCommand { get; private set; }

        public ICommand QuickSearchCommand { get; private set; }

        private void Search(string keywords)
        {
            if (!String.IsNullOrWhiteSpace(keywords))
            {
                Results =
                    BookManager.Instance.Books.Where(b => ContainsAll(b.Title, keywords.Split(' ')))
                    .OrderBy(b => b.Category)
                    .ThenBy(b => b.Title)
                    .ToArray();

                PushRecentKeywords(keywords);
            }
            else
            {
                Results = new Book[0];
            }
        }

        private bool ContainsAll(string title, string[] keywords)
        {
            return keywords.All(keyword => title.IndexOf(keyword, StringComparison.InvariantCultureIgnoreCase) >= 0);
        }

        //private bool ContainsAny(string title, string[] keywords)
        //{
        //    return keywords.Any(keyword => title.IndexOf(keyword, StringComparison.InvariantCultureIgnoreCase) >= 0);
        //}

        private void PushRecentKeywords(string keywords)
        {
            if (RecentKeywords.Contains(keywords))
            {
                RecentKeywords.Remove(keywords);
            }

            RecentKeywords.Insert(0, keywords);

            if (RecentKeywords.Count > NumberOfRecentItems)
            {
                RecentKeywords.RemoveAt(NumberOfRecentItems);
            }

            RecentKeywordCollection.Drop();
            RecentKeywordCollection.Insert(RecentKeywords);
        }

        private Ldata.IDataCollection<string> RecentKeywordCollection
        {
            get { return App.DataStore.GetCollection<string>(DataCollectionNames.RecentKeywords); }
        }

        private const int NumberOfRecentItems = 5;
    }
}

using Lapps.Utils;
using Lapps.Utils.Collections;
using Lbookshelf.Business;
using Lbookshelf.Models;
using Lbookshelf.Utils;
using Microsoft.Expression.Interactivity.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Lbookshelf.ViewModels
{
    public class BrowseBooksViewModel : ObservableObject
    {
        public BrowseBooksViewModel()
        {
            _dimensions = DimensionManager.Instance.SupportedDimensions;
            _dimensions.ForEach(d => d.GroupsChanged +=
                (o, e) =>
                {
                    // When you add/remove a book, multiple dimensions might trigger the GroupsChanged event.
                    // But only the one shows in the foreground needs updating, because the rest will always
                    // get updated when you switch to them (via a change to SelectedDimensionKey).
                    if (d == _selectedDimension)
                    {
                        RaisePropertyChanged("GroupKeys");

                        // When you remove a book that's the only book within a group, it'll also remove that
                        // group. We need to ensure the _selectedGroup no longer refers to that invalid group,
                        // and set the _selectedGroup to the first group of the dimension if there's one.
                        // If this is the last group of the dimension, _selectedGroup will be set to null.
                        // This also handles the edge case when you add a book for the first time!
                        if (!_selectedDimension.Elements.Contains(_selectedGroup))
                        {
                            _selectedGroup = _selectedDimension.Elements.FirstOrDefault();

                            RaisePropertyChanged("SelectedGroupKey");
                            RaisePropertyChanged("Books");
                        }
                    }
                });

            // There's always at least one dimension at hand.
            _selectedDimension = _dimensions.First();
            // At the very beginning, there's no group, so selected group will be set to null.
            _selectedGroup = _selectedDimension.Elements.FirstOrDefault();
        }

        private Dimension[] _dimensions;

        // Level 1: How do we group the books? By category or by publisher?
        public IEnumerable<string> DimensionKeys
        {
            get { return _dimensions.Select(d => d.Key); }
        }

        private Dimension _selectedDimension;
        public string SelectedDimensionKey
        {
            get { return _selectedDimension.Key; }
            set
            {
                if (_selectedDimension.Key != value)
                {
                    _selectedDimension = _dimensions.First(d => d.Key == value);
                    _selectedGroup = _selectedDimension.Elements.FirstOrDefault();
                    _selectedBook = null;

                    RaisePropertyChanged();
                    RaisePropertyChanged("GroupKeys");
                    RaisePropertyChanged("SelectedGroupKey");
                    RaisePropertyChanged("Books");
                    RaisePropertyChanged("SelectedBook");
                }
            }
        }

        // Level 2: What do we get after grouping?
        public IList<string> GroupKeys
        {
            get { return _selectedDimension.Elements.Select(g => g.Key).ToList(); }
        }

        private SortedObservableGroup<string, Book> _selectedGroup;
        public string SelectedGroupKey
        {
            get
            {
                // When you remove the last group of the dimension, _selectedGroup will
                // be set to null, so calling _selectedGroup.Key will throw.
                if (_selectedGroup != null)
                {
                    return _selectedGroup.Key;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (_selectedGroup.Key != value)
                {
                    _selectedGroup = _selectedDimension.Elements.First(g => g.Key == value);

                    RaisePropertyChanged();
                    RaisePropertyChanged("Books");
                }
            }
        }

        // Level 3: What are inside each group?
        public ObservableCollection<Book> Books
        {
            get
            {
                // When you remove the last group of the dimension, _selectedGroup will
                // be set to null, so calling _selectedGroup.Elements will throw.
                if (_selectedGroup != null)
                {
                    return _selectedGroup.Elements;
                }
                else
                {
                    return null;
                }
            }
        }

        private Book _selectedBook;
        public Book SelectedBook
        {
            get { return _selectedBook; }
            set
            {
                if (_selectedBook != value)
                {
                    _selectedBook = value;
                    RaisePropertyChanged();
                }
            }
        }
    }
}

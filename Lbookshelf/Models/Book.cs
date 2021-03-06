﻿using Lapps.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lbookshelf.Models
{
    public class Book : ObservableObject, IEquatable<Book>, IComparable<Book>
    {
        public int Id { get; set; }

        private string _isbn;
        // ISBN-13
        public string Isbn
        {
            get { return _isbn; }
            set
            {
                if (_isbn != value)
                {
                    _isbn = value;
                    RaisePropertyChanged();
                }
            }
        }

        private string _title;
        public string Title
        {
            get { return _title; }
            set
            {
                if (_title != value)
                {
                    _title = value;
                    RaisePropertyChanged();
                }
            }
        }

        private string[] _authors;
        public string[] Authors
        {
            get { return _authors; }
            set
            {
                if (_authors != value)
                {
                    _authors = value;
                    RaisePropertyChanged();
                }
            }
        }

        private string _category;
        public string Category
        {
            get { return _category; }
            set
            {
                if (_category != value)
                {
                    _category = value;
                    RaisePropertyChanged();
                }
            }
        }

        private string _publisher;
        public string Publisher
        {
            get { return _publisher; }
            set
            {
                if (_publisher != value)
                {
                    _publisher = value;
                    RaisePropertyChanged();
                }
            }
        }

        private DateTime _publishedDate;
        public DateTime PublishedDate
        {
            get { return _publishedDate; }
            set
            {
                if (_publishedDate != value)
                {
                    _publishedDate = value;
                    RaisePropertyChanged();
                }
            }
        }

        private string _description;
        public string Description
        {
            get { return _description; }
            set
            {
                if (_description != value)
                {
                    _description = value;
                    RaisePropertyChanged();
                }
            }
        }

        private string _thumbnail;
        public string Thumbnail
        {
            get { return _thumbnail; }
            set
            {
                if (_thumbnail != value)
                {
                    _thumbnail = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// When changing the thumbnail of a book, the file name of the thumbnail
        /// stays the same as before, but the content of the corresponding image
        /// might change completely. This method notify the change of the image
        /// even if the value of Thumbnail remains the same.
        /// </summary>
        public void RefreshThumbnail()
        {
            RaisePropertyChanged("Thumbnail");
        }

        private string _fileName;
        public string FileName
        {
            get { return _fileName; }
            set
            {
                if (_fileName != value)
                {
                    _fileName = value;
                    RaisePropertyChanged();
                }
            }
        }

        #region Value Equality

        /// <summary>
        /// Two books equal when they have the same title and the first alphabetical author.
        /// </summary>
        public bool Equals(Book other)
        {
            if (Object.ReferenceEquals(other, null))
            {
                return false;
            }

            if (Object.ReferenceEquals(this, other))
            {
                return true;
            }

            if (this.GetType() != other.GetType())
            {
                return false;
            }

            return this.Id == other.Id;
        }

        public static bool operator ==(Book lhs, Book rhs)
        {
            if (Object.ReferenceEquals(lhs, null))
            {
                if (Object.ReferenceEquals(rhs, null))
                {
                    return true;
                }

                return false;
            }

            return lhs.Equals(rhs);
        }

        public static bool operator !=(Book lhs, Book rhs)
        {
            return !(lhs == rhs);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as Book);
        }

        public override int GetHashCode()
        {
            return this.Id;
        }

        #endregion

        #region Value Comparison

        public int CompareTo(Book other)
        {
            return GetComparableText(this).CompareTo(GetComparableText(other));
        }

        private string GetComparableText(Book book)
        {
            return book.Title + " - " + book.Authors[0];
        }

        #endregion
    }
}

using Lbookshelf.Models;
using Microsoft.Expression.Interactivity.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace Lbookshelf.Utils
{
    public class BuildContextMenuBehavior : Behavior<FrameworkElement>
    {
        protected override void OnAttached()
        {
            base.OnAttached();

            if (!String.IsNullOrEmpty(TemplateName))
            {
                var book = (Book)AssociatedObject.DataContext;

                switch (TemplateName)
                {
                    case "Pinned":
                        AssociatedObject.ContextMenu = CreatePinnedContextMenu(book);
                        break;
                    case "Category":
                    case "Publisher":
                        AssociatedObject.ContextMenu = CreateComputedDimensionContextMenu(book);
                        break;
                    case "Booklist":
                        AssociatedObject.ContextMenu = CreateBooklistContextMenu(book);
                        break;
                    case "SearchResults":
                        AssociatedObject.ContextMenu = CreateSearchResultsContextMenu(book);
                        break;
                    default:
                        break;
                }
            }
        }

        public string TemplateName
        {
            get { return (string)GetValue(TemplateNameProperty); }
            set { SetValue(TemplateNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TemplateName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TemplateNameProperty =
            DependencyProperty.Register("TemplateName", typeof(string), typeof(BuildContextMenuBehavior), new PropertyMetadata(null));

        private ContextMenu CreatePinnedContextMenu(Book book)
        {
            return new ContextMenu
            {
                ItemsSource = new Control[]
                {
                    CreateOpenBookMenuItem(book),
                    CreateOpenBookInFileExplorerMenuItem(book),
                    new Separator(),
                    CreatePinOrUnpinMenuItem(book),
                    CreateAddToBooklistMenuItem(book)
                }
            };
        }
        
        private ContextMenu CreateComputedDimensionContextMenu(Book book)
        {
            return new ContextMenu
            {
                ItemsSource = new Control[]
                {
                    CreateOpenBookMenuItem(book),
                    CreateOpenBookInFileExplorerMenuItem(book),
                    new Separator(),
                    CreatePinOrUnpinMenuItem(book),
                    CreateAddToBooklistMenuItem(book),
                    new Separator(),
                    CreateEditBookMenuItem(book),
                    CreateDeleteBookMenuItem(book)
                }
            };
        }

        private ContextMenu CreateBooklistContextMenu(Book book)
        {
            return new ContextMenu
            {
                ItemsSource = new Control[]
                {
                    CreateOpenBookMenuItem(book),
                    CreateOpenBookInFileExplorerMenuItem(book),
                    new Separator(),
                    CreatePinOrUnpinMenuItem(book),
                    new Separator(),
                    CreateRemoveFromBooklistMenuItem(book)
                }
            };
        }

        private ContextMenu CreateSearchResultsContextMenu(Book book)
        {
            return new ContextMenu
            {
                ItemsSource = new Control[]
                {
                    CreateOpenBookMenuItem(book),
                    CreateOpenBookInFileExplorerMenuItem(book),
                    new Separator(),
                    CreatePinOrUnpinMenuItem(book),
                    CreateAddToBooklistMenuItem(book),
                    new Separator(),
                    CreateShowBookInfoMenuItem(book)
                }
            };
        }

        private MenuItem CreateOpenBookMenuItem(Book book)
        {
            return new MenuItem
            {
                Header = "Open",
                Command = BookCommands.OpenBookCommand,
                CommandParameter = book
            };
        }

        private MenuItem CreateOpenBookInFileExplorerMenuItem(Book book)
        {
            return new MenuItem
            {
                Header = "Open in File Explorer",
                Command = BookCommands.OpenBookInFileExplorerCommand,
                CommandParameter = book
            };
        }

        private MenuItem CreatePinOrUnpinMenuItem(Book book)
        {
            var menuItem = new MenuItem();

            menuItem.Header = App.HomeViewModel.IsPinned(book) ? "Unpin from Home" : "Pin to Home";
            menuItem.Command = new ActionCommand(
                () =>
                {
                    App.HomeViewModel.PinOrUnpin(book);
                    menuItem.Header = (string)menuItem.Header == "Pin to Home" ? "Unpin from Home" : "Pin to Home";
                });

            return menuItem;
        }

        private MenuItem CreateAddToBooklistMenuItem(Book book)
        {
            return new MenuItem
            {
                Header = "Add to booklist",
                Command = BookCommands.AddToBooklistCommand,
                CommandParameter = book
            };
        }

        private MenuItem CreateRemoveFromBooklistMenuItem(Book book)
        {
            return new MenuItem
            {
                Header = "Remove from booklist",
                Command = BookCommands.RemoveFromBooklistCommand,
                CommandParameter = book
            };
        }

        private MenuItem CreateEditBookMenuItem(Book book)
        {
            return new MenuItem
            {
                Header = "Edit",
                Command = BookCommands.EditBookCommand,
                CommandParameter = book
            };
        }

        private MenuItem CreateDeleteBookMenuItem(Book book)
        {
            return new MenuItem
            {
                Header = "Delete",
                Command = BookCommands.DeleteBookCommand,
                CommandParameter = book
            };
        }

        //private MenuItem CreateFindBookInfoMenuItem(Book book)
        //{
        //    return new MenuItem
        //    {
        //        Header = "Find book info",
        //        Command = BookCommands.FindBookInfoCommand,
        //        CommandParameter = book
        //    };
        //}

        private MenuItem CreateShowBookInfoMenuItem(Book book)
        {
            return new MenuItem
            {
                Header = "Properties",
                Command = BookCommands.ShowBookInfoCommand,
                CommandParameter = book
            };
        }
    }
}

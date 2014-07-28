using FirstFloor.ModernUI.Windows.Controls;
using Lbookshelf.Content;
using Microsoft.Expression.Interactivity.Core;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Lbookshelf.Utils
{
    public class DialogService
    {
        // This method is used to show dialog with custom content.
        public static bool? ShowDialog(string title, Uri uri, object dataContext, Action okAction, Size? size = null)
        {
            var dialog = new ModernDialog();

            // This settings are for EditBookControl.
            dialog.MaxWidth = 500;
            dialog.MinWidth = 500;
            dialog.SizeToContent = SizeToContent.Height;

            if (size.HasValue)
            {
                dialog.MinWidth = size.Value.Width;
                dialog.MaxWidth = size.Value.Width;

                dialog.MinWidth = size.Value.Height;
                dialog.MaxHeight = size.Value.Height;

                dialog.SizeToContent = SizeToContent.Manual;
            }

            dialog.Title = title;
            dialog.Content = App.LoadComponent(uri);

            var okButton = dialog.OkButton;
            okButton.Width = 80;
            okButton.Command = new ActionCommand(
                () =>
                {
                    if (okAction != null)
                    {
                        okAction();
                    }

                    dialog.DialogResult = true;
                    dialog.Close();
                });

            var cancelButton = dialog.CancelButton;
            cancelButton.Width = 80;
            cancelButton.Margin = new Thickness(6, 0, 0, 0);

            dialog.Buttons = new[] { okButton, cancelButton };
            dialog.DataContext = dataContext;

            return dialog.ShowDialog();
        }

        // This method is used to show action dialog with a list of action buttons.
        public static void ShowDialog(string title, string text, params Tuple<string, string, Action>[] actions)
        {
            var dialog = new ModernDialog();

            dialog.Title = title;
            dialog.Content = new ActionListControl();
            dialog.DataContext = Tuple.Create(
                text,
                actions.Select(action => Tuple.Create(
                    action.Item1,
                    action.Item2,
                    new ActionCommand(
                        () =>
                        {
                            if (action.Item3 != null)
                            {
                                action.Item3();
                            }

                            dialog.DialogResult = true;
                            dialog.Close();
                        }))));

            var cancelButton = dialog.CancelButton;
            cancelButton.Width = 80;
            //cancelButton.Content = "Cancel";
            dialog.Buttons = new[] { cancelButton };

            dialog.ShowDialog();
        }

        private static OpenFileDialog CreateOpenFileDialog(bool multiselect, string filter, string defaultExt, string initialDirectory)
        {
            return new OpenFileDialog
            {
                DefaultExt = defaultExt,
                Filter = filter,
                AddExtension = true,
                CheckFileExists = true,
                CheckPathExists = true,
                Multiselect = multiselect,
                InitialDirectory = initialDirectory
            };
        }

        public static void ShowOpenFileDialog(Action<string> okAction, string filter = "PDF documents|*.pdf", string defaultExt = ".pdf", string initialDirectory = "")
        {
            var openFileDialog = CreateOpenFileDialog(false, filter, defaultExt, initialDirectory);

            if (openFileDialog.ShowDialog() == true && okAction != null)
            {
                okAction(openFileDialog.FileName);
            }
        }

        public static void ShowOpenFileDialog(Action<string[]> okAction, string filter = "PDF documents|*.pdf", string defaultExt = ".pdf", string initialDirectory = "")
        {
            var openFileDialog = CreateOpenFileDialog(true, filter, defaultExt, initialDirectory);

            if (openFileDialog.ShowDialog() == true && okAction != null)
            {
                okAction(openFileDialog.FileNames);
            }
        }

        // This method is used to show simple message dialog.
        public static bool ShowDialog(string text, string title = "", MessageBoxButton button = MessageBoxButton.OK)
        {
            var dialogResult = ModernDialog.ShowMessage(text, title, button);
            return dialogResult == MessageBoxResult.OK || dialogResult == MessageBoxResult.Yes;
        }
    }
}

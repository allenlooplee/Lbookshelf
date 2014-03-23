using Lbookshelf.Business;
using Lbookshelf.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interactivity;

namespace Lbookshelf.Utils
{
    public abstract class BookActionBase : TriggerAction<DependencyObject>
    {
        public Book Target
        {
            get { return (Book)GetValue(TargetProperty); }
            set { SetValue(TargetProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Target.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TargetProperty =
            DependencyProperty.Register("Target", typeof(Book), typeof(BookActionBase), new PropertyMetadata(null));

        protected override void Invoke(object parameter)
        {
            if (!StorageManager.Instance.IsReady)
            {
                DialogService.ShowDialog(
                    "The drive your bookshelf resides in is not ready. Please plug in the removable drive, and try again.",
                    "Drive Not Ready");
            }
            else
            {
                TakeAction();
            }
        }

        protected abstract void TakeAction();
    }
}

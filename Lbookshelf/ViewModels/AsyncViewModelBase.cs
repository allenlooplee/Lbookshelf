using Lapps.Utils;
using Microsoft.Expression.Interactivity.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Lbookshelf.ViewModels
{
    public class AsyncViewModelBase : ObservableObject
    {
        private bool _isPerformingAction;
        public bool IsPerformingAction
        {
            get { return _isPerformingAction; }
            set
            {
                if (_isPerformingAction != value)
                {
                    _isPerformingAction = value;
                    RaisePropertyChanged();
                }
            }
        }

        private string _errorMessage;
        public string ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                if (_errorMessage != value)
                {
                    _errorMessage = value;
                    RaisePropertyChanged();
                }
            }
        }

        protected ICommand CreateAsyncCommand(Func<Task> asyncAction)
        {
            return CreateCommand(
                async () =>
                {
                    IsPerformingAction = true;

                    ErrorMessage = null;

                    try
                    {
                        await asyncAction();
                    }
                    catch (Exception ex)
                    {
                        ErrorMessage = ex.Message;
                    }

                    IsPerformingAction = false;
                });
        }

        protected ICommand CreateCommand(Action action)
        {
            return new ActionCommand(action);
        }
    }
}

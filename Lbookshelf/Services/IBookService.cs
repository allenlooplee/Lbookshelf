using Lbookshelf.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lbookshelf.Services
{
    public interface IBookService : INotifyPropertyChanged
    {
        string Name { get; }

        WorkStatus Status { get; }

        string Message { get; }

        Task<Book[]> FindBookInfoAsync(string keyword);
    }

    public enum WorkStatus
    {
        Created,
        Running,
        RanToCompletion,
        Faulted,
        NoResults
    }
}

using ReactiveUI;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Sikor.Services;
using Sikor.Model;
using Sikor.Container;
using System.Threading.Tasks;
using Avalonia.Threading;

namespace Sikor.Model
{
    public class SearchResults
    {
        public ObservableCollection<Issue> Issues { get; set; }

        public bool Offline { get; set; }

        public SearchResults()
        {
            Offline = false;
        }
    }
}
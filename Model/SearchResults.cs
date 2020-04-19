using System.Collections.ObjectModel;

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
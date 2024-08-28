using Mantis.Domain.Shared;
using Mantis.Domain.Clients.Models;
using System.Net;
using Mantis.Domain.Carriers.Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Syncfusion.Blazor.Navigations;

namespace Mantis.Domain.Shared.Services
{
    public class BreadcrumbService : INotifyPropertyChanged
    {
        private List<BreadcrumbItem> _items = new List<BreadcrumbItem>();

        public List<BreadcrumbItem> Items
        {
            get => _items;
            set
            {
                _items = value;
                OnPropertyChanged();
            }
        }

        public void UpdateBreadcrumb(List<BreadcrumbItem> items)
        {
            Items = items;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    public class NavigationService
    {
        private string _lastClientPage = "/Clients/List/11";

        public void SetLastClientPage(string url)
        {
            _lastClientPage = url;
        }

        public string GetLastClientPage()
        {
            return _lastClientPage;
        }
    }
}

using OrderManager.Data;
using OrderManager.Model;
using OrderManager.ConstValues;
using System.Collections.ObjectModel;
using Microsoft.IdentityModel.Tokens;
using OrderManager.ViewModel;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace OrderManager.Logic
{
    internal class Pagination
    {
        private OrderVM _orderVM;

        internal int _page;
        internal int _startIndex;

        private int _savedPage;
        private int _pageSize;
        private int _tableSize;
        private int _savedStartIndex;

        private string _lastSearchEntry;

        internal Pagination(OrderVM orderVM)
        {
            _orderVM = orderVM;
            _page = 1;
            _savedPage = 1;
            _startIndex = 0;
            _lastSearchEntry = "";
        }

        internal bool TryGetNextPage(out ObservableCollection<OrderModel> nextPage, bool jumpToLastPage = false, bool newEntry = false)
        {
            nextPage = null;
            using (DataHandler _context = new DataHandler(new DBInitialization()))
            {
                List<OrderModel> query;
                _tableSize = _context.GetTableSize<OrderModel>();

                // Check if current page is the last page. If there is a new entry, the page is getting reloaded.
                if (_tableSize <= _page * _pageSize)
                {
                    if (newEntry)
                    {
                        query = _context.GetData<OrderModel>(a => a.VorgaengeInAuftrag
                            .All(v => v.Vorgangsstatus != ConstStrings.VORGANGSSTATUS_ABGESCHLOSSEN), null, _startIndex, _pageSize).ToList();

                        if (query != null && query.Any())
                        {
                            nextPage = new ObservableCollection<OrderModel>(query);
                            return true;
                        }

                        else
                        {
                            throw new Exception("No query available");
                        }
                    }

                    else
                    {
                        return false;
                    }
                }

                else if (jumpToLastPage)
                {
                    // Last Page is full and user didn't create a new entry? -> Jump to that page.
                    // When user creates a new entry and last page is already full, skip one page
                    if (_tableSize % _pageSize == 0 && !newEntry)
                    {
                        _page = _tableSize / _pageSize;
                    }

                    else
                    {
                        _page = (_tableSize / _pageSize) + 1;
                    }
                    _startIndex = (_page - 1) * _pageSize;
                }

                else
                {
                    ++_page;
                    _startIndex = (_page - 1) * _pageSize;
                }

                query = _context.GetData<OrderModel>(a => a.VorgaengeInAuftrag
                    .All(v => v.Vorgangsstatus != ConstStrings.VORGANGSSTATUS_ABGESCHLOSSEN), null, _startIndex, _pageSize).ToList();

                if (query != null && query.Any())
                {
                    nextPage = new ObservableCollection<OrderModel>(query);
                    return true;
                }
            }
            return false;
        }

        internal bool TryGetPreviousPage(out ObservableCollection<OrderModel> previousPage, bool firstPage = false)
        {
            previousPage = null;

            if (_page <= 1)
            {
                return false;
            }

            using (DataHandler _context = new DataHandler(new DBInitialization()))
            {
                _tableSize = _context.GetTableSize<OrderModel>();

                if (firstPage)
                {
                    _page = 1;
                    _startIndex = 0;
                }

                else
                {
                    --_page;
                    _startIndex = (_page - 1) * _pageSize;
                }

                List<OrderModel> query = _context.GetData<OrderModel>(a => a.VorgaengeInAuftrag
                    .All(v => v.Vorgangsstatus != ConstStrings.VORGANGSSTATUS_ABGESCHLOSSEN), null, _startIndex, _pageSize).ToList();

                if (query != null && query.Any())
                {
                    previousPage = new ObservableCollection<OrderModel>(query);
                    return true;
                }

                else
                {
                    return false;
                    // throw new Exception("No query available");
                }
            }
        }

        internal ObservableCollection<OrderModel> RefreshView(string searchString = "", string selectedPageSize = "")
        {
            // TODO: Wird schon übergeben, man braucht nicht mit _orderVM.Searchkey zuweisen
            searchString = (_orderVM.SearchKey == null) ? "" : _orderVM.SearchKey;

            using (DataHandler _context = new DataHandler(new DBInitialization()))
            {
                _tableSize = _context.GetTableSize<OrderModel>();

                // User starts to search something
                if (searchString.Length > 0 && (_lastSearchEntry.Length == 0 || _lastSearchEntry == ""))
                { 
                    _savedPage = _page;
                    _savedStartIndex = _startIndex;
                    _page = 1;
                    _startIndex = 0;

                    _lastSearchEntry = searchString;                  
                }

                // User stops to search something
                else if (searchString.Length == 0 && (_lastSearchEntry.Length > 0 || _lastSearchEntry != ""))
                {
                    _page = _savedPage;
                    _startIndex = _savedStartIndex;

                    _lastSearchEntry = "";
                }

                // User changes pagesize and is not searching something
                else if (selectedPageSize != Convert.ToString(_pageSize) && searchString.IsNullOrEmpty() && _lastSearchEntry == searchString)
                {
                    _page = 1;
                    _startIndex = 0;
                }

                // For first initialization there needs to be a selectedPageSize passed, otherwise take the public SelectedPageSize
                if (selectedPageSize != "")
                {
                    _orderVM.SelectedPageSize = selectedPageSize;
                }

                // Parsing "all" as SelectedPageSize leads to else statement
                if (int.TryParse(_orderVM.SelectedPageSize, out _pageSize)) { }
                else
                {
                    _pageSize = ConstInts.MAXPAGESIZE;
                }

                // Last entry in page deleted? -> Go to previous page. TryGetPreviousPage is loading previous page
                if (_tableSize == (_page - 1) * _pageSize && TryGetPreviousPage(out ObservableCollection<OrderModel> lastPage))
                {
                    return lastPage;
                }

                // More than one page was deleted (in search mode) -> jump to page 1
                

                // TODO: Implement a loop which goes back more than one site when whole sites have been deleted
                /*else if (_tableSize <= (_page - 1) * _pageSize && TryGetPreviousPage(out ObservableCollection<OrderModel> _) != true)
                {
                    while (TryGetPreviousPage(out ObservableCollection<OrderModel> getPage) != true)
                    {
                        if (_page - 1 < 1)
                        {
                            break;
                        }
                    }
                    return getPage;
                }*/

                List<OrderModel> query = _context.GetData<OrderModel>(a => a.VorgaengeInAuftrag.All(v => v.Vorgangsstatus != ConstStrings.VORGANGSSTATUS_ABGESCHLOSSEN) &&
                    (a.Auftragnummer.StartsWith(searchString) || Convert.ToString(a.AuftragID).StartsWith(searchString)),
                    null, _startIndex, _pageSize).ToList();

                return new ObservableCollection<OrderModel>(query);
            }
        }
    }
}
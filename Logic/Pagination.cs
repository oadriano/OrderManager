using OrderManager.Data;
using OrderManager.Model;
using OrderManager.ConstValues;
using System.Collections.ObjectModel;
using Microsoft.IdentityModel.Tokens;
using OrderManager.ViewModel;

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

        internal Pagination(OrderVM orderVM)
        {
            _orderVM = orderVM;
            _page = 1;
            _startIndex = 0;
            _savedPage = _page;
            _savedStartIndex = _startIndex;
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
                        return false; // HIER IST DER FEHLER
                    }
                }

                if (jumpToLastPage)
                {
                    // Last Page is full and user didn't create an entry? -> Jump to that page.
                    // When user creates a new entry and last page is already full, the last page is the next one with that entry
                    if (_tableSize % _pageSize == 0 && !newEntry)
                    {
                        _page = (_tableSize / _pageSize);
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
                    throw new Exception("No query available");
                }
            }
        }
        // TODO: BUG => REFRESHVIEW WIRD ZWEIMAL AUFGERUFEN

        internal ObservableCollection<OrderModel> RefreshView(string searchString = "", string selectedPageSize = "")
        {
            searchString = (_orderVM.SearchKey == null) ? "" : _orderVM.SearchKey;
            // TODO: BUG => Wenn ich suche, wird nicht auf die vorherige Seite aktualisiert, sondern auf Seite 1

            using (DataHandler _context = new DataHandler(new DBInitialization()))
            {
                if (!searchString.IsNullOrEmpty())
                {
                    _savedPage = _page;
                    _savedStartIndex = _startIndex;

                    _page = 1;
                    _startIndex = 0;
                }

                else if (_orderVM.ActivatedFooter)
                {
                    _page = 1;
                    _startIndex = 0;
                }

                else if (!_orderVM.ActivatedFooter)
                {
                    _page = _savedPage;
                    _startIndex = _savedStartIndex;
                }


                _tableSize = _context.GetTableSize<OrderModel>();

                // For first initialization there needs to be a selectedPageSize passed, otherwise take the public SelectedPageSize
                if (selectedPageSize != "")
                {
                    _orderVM.SelectedPageSize = selectedPageSize;
                }

                if (int.TryParse(_orderVM.SelectedPageSize, out _pageSize)) { }
                else
                {
                    _pageSize = 1000;
                }

                // Last entry in page deleted? -> Go to previous page. TryGetPreviousPage is loading previous page
                if (_tableSize == (_page - 1) * _pageSize && TryGetPreviousPage(out ObservableCollection<OrderModel> lastPage))
                {
                    return lastPage;
                }

                List<OrderModel> query = _context.GetData<OrderModel>(a => a.VorgaengeInAuftrag.All(v => v.Vorgangsstatus != ConstStrings.VORGANGSSTATUS_ABGESCHLOSSEN) &&
                    (a.Auftragnummer.StartsWith(searchString) || Convert.ToString(a.AuftragID).StartsWith(searchString)),
                    null, _startIndex, _pageSize).ToList();

                return new ObservableCollection<OrderModel>(query);
            }
        }
    }
}
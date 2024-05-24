using System.Collections.ObjectModel;
using System.ComponentModel;
using OrderManager.Data;
using OrderManager.Model;
using OrderManager.Logic;
using System.Windows;
using Microsoft.IdentityModel.Tokens;
using System.Runtime.CompilerServices;
using System.Windows.Threading;

namespace OrderManager.ViewModel
{
    internal class OrderVM : INotifyPropertyChanged
    {
        private DBInitialization _initializedDB;
        private Pagination _pagination;
        private OrderModel _currentOrder;
        private OrderModel _selectedGridItem;
        private ObservableCollection<OrderModel> _currentOrderList;
        private DispatcherTimer dispatcherTimer;

        private string _btnText;
        private string _orderNumber;
        private string _orderStatus;
        private string _savedPageSize;
        private string _searchKey;
        private string _selectedPageSize;

        private bool _deleteOrderIsEnabled;
        private bool _activatedFooter;

        //private bool _searchInitiatedPageSizeChange;

        public RelayCommand NewOrderCommand { get; }
        public RelayCommand UpdateOrCreateOrderCommand { get; }
        public RelayCommand ShowMainOrderViewCommand { get; }
        public RelayCommand DeleteOrderCommand { get; }
        public RelayCommand FirstPageCommand { get; }
        public RelayCommand PreviousPageCommand { get; }
        public RelayCommand NextPageCommand { get; }
        public RelayCommand LastPageCommand { get; }



        public int Page
        {
            // HIER AUCH REFACTOREN
            get => _pagination._page;
            set
            {
                if (_pagination._page != value)
                {
                    _pagination._page = value;
                    OnPropertyChanged();
                }
            }
        }
        public bool ActivatedFooter
        {
            get => _activatedFooter;
            set
            {
                if (_activatedFooter != value)
                {
                    _activatedFooter = value;
                    OnPropertyChanged();
                }
            }
        }
        public string SearchKey
        {
            get => _searchKey;
            set
            {
                if (_searchKey != value)
                {
                    _searchKey = value;

                    dispatcherTimer.Stop();
                    dispatcherTimer.Start();
                }
            }
        }
        public string SelectedPageSize
        {
            get => _selectedPageSize;
            set
            {
                if (_selectedPageSize != value)
                {
                    _selectedPageSize = value;

                    CurrentOrderList = _pagination.RefreshView(SearchKey, _selectedPageSize);
                    
                    OnPropertyChanged(nameof(Page));
                    OnPropertyChanged();
                }
            }
        }
        public bool DeleteOrderIsEnabled
        {
            get => _deleteOrderIsEnabled;
            set
            {
                if (_deleteOrderIsEnabled != value)
                {
                    _deleteOrderIsEnabled = value;
                    OnPropertyChanged();
                }
            }
        }
        public string BtnText
        {
            get => _btnText;
            set
            {
                if (_btnText != value)
                {
                    _btnText = value;
                    OnPropertyChanged();
                }
            }
        }
        public string OrderNumber
        {
            get
            {
                return _orderNumber;
            }
            set
            {
                if (_orderNumber != value)
                {
                    _orderNumber = value;
                    OnPropertyChanged();
                }
            }
        }
        public string OrderStatus
        {
            get
            {
                if (_orderStatus != null)
                {
                    return _orderStatus;
                }
                return _orderStatus = "Neu";
            }
            set
            {
                if (_orderStatus != value)
                {
                    _orderStatus = value;
                    OnPropertyChanged();
                }
            }
        }
        public ObservableCollection<OrderModel> CurrentOrderList
        {
            get => _currentOrderList;
            set
            {
                if (_currentOrderList != value)
                {
                    _currentOrderList = value;
                    OnPropertyChanged();
                }
            }
        }
        public OrderModel SelectedGridItem
        {
            get => _selectedGridItem;
            set
            {
                if (_selectedGridItem != value)
                {
                    _selectedGridItem = value;

                    if (_selectedGridItem != null)
                    {
                        OrderNumber = _selectedGridItem.Auftragnummer;
                        OrderStatus = _selectedGridItem.Auftragsstatus;
                        DeleteOrderIsEnabled = true;
                    }

                    else
                    {
                        _orderNumber = null;
                        _orderStatus = null;
                        _deleteOrderIsEnabled = false;
                    }

                    _btnText = value == null ? "Create" : "Update";
                    OnPropertyChanged(nameof(SelectedGridItem), nameof(BtnText), nameof(OrderNumber), nameof(OrderStatus), nameof(DeleteOrderIsEnabled));
                }
            }
        }

        public OrderVM()
        {
            _pagination = new Pagination(this);

            BtnText = "Create";
            OrderStatus = "Neu";
            SelectedPageSize = "10";
            DeleteOrderIsEnabled = false;
            ActivatedFooter = true;

            NewOrderCommand = new RelayCommand(_ => NewOrder());
            DeleteOrderCommand = new RelayCommand(_ => DeleteOrder());
            UpdateOrCreateOrderCommand = new RelayCommand(_ => UpdateOrCreateOrder());
            //ShowMainOrderViewCommand = new RelayCommand(_ => ExecDefaultView());
            FirstPageCommand = new RelayCommand(_ => ExecFirstPage());
            PreviousPageCommand = new RelayCommand(_ => ExecPreviousPage());
            NextPageCommand = new RelayCommand(_ => ExecNextPage());
            LastPageCommand = new RelayCommand(_ => ExecLastPage());

            // DispatcherTimer setup
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = TimeSpan.FromSeconds(0.3);

            // Default VM at start
            CurrentOrderList = _pagination.RefreshView();
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            // Set the PageSize to "all" and deactivate footer buttons, when an entry is being searched. Pagination should be deactivated while searching.
            if (SearchKey.Length > 0 && ActivatedFooter)
            {
                _savedPageSize = SelectedPageSize;
                SelectedPageSize = "all";
                ActivatedFooter = false;
            }

            else if (!ActivatedFooter)
            {
                SelectedPageSize = _savedPageSize;
                ActivatedFooter = true;
            }

            //CurrentOrderList = _pagination.RefreshView(SearchKey, "all");
            OnPropertyChanged(nameof(Page));

            dispatcherTimer.Stop();
        }

        private void ExecFirstPage()
        {
            if (_pagination.TryGetPreviousPage(out ObservableCollection<OrderModel> firstPage, true) && firstPage != null)
            {
                CurrentOrderList = firstPage;
            }
            OnPropertyChanged(nameof(Page));
        }

        private void ExecPreviousPage()
        {
            if (_pagination.TryGetPreviousPage(out ObservableCollection<OrderModel> previousPage) && previousPage != null)
            {
                CurrentOrderList = previousPage;
                OnPropertyChanged(nameof(Page));
            }
        }

        private void ExecNextPage()
        {
            if (_pagination.TryGetNextPage(out ObservableCollection<OrderModel> nextPage) && nextPage != null)
            {
                CurrentOrderList = nextPage;
                OnPropertyChanged(nameof(Page));
            }
        }

        private void ExecLastPage()
        {
            if (_pagination.TryGetNextPage(out ObservableCollection<OrderModel> lastPage, true) && lastPage != null)
            {
                CurrentOrderList = lastPage;
            }
            OnPropertyChanged(nameof(Page));
        }

        private void UpdateOrCreateOrder()
        {
            if (SelectedGridItem == null)
            {
                CreateOrder();
            }

            else
            {
                UpdateOrder();
            }
        }

        private void NewOrder()
        {
            SelectedGridItem = null;
            FocusMessenger.RequestFocus("txtOrderNumber");
        }

        private void CreateOrder()
        {
            if (OrderNumber.IsNullOrEmpty())
            {
                MessageBox.Show("Please enter an order number");
                return;
            }

            if (OrderStatus.IsNullOrEmpty())
            {
                MessageBox.Show("Please select an order status");
                return;
            }

            _currentOrder = new OrderModel();

            _currentOrder.Auftragnummer = OrderNumber;
            _currentOrder.Auftragsstatus = OrderStatus;
            _currentOrder.DT_Created = DateTime.Now;
            _currentOrder.DT_Modified = DateTime.Now;

            using (_initializedDB = new DBInitialization())
            {
                _initializedDB.Auftraege.Add(_currentOrder);
                _initializedDB.SaveChanges();
            }

            //Always jump to the last page when creating a new order.
            if (_pagination.TryGetNextPage(out ObservableCollection<OrderModel> lastPage, true, true) && lastPage != null)
            {
                CurrentOrderList = lastPage;
            }
            OnPropertyChanged(nameof(Page));

            _currentOrder = null;
            OrderNumber = null;
            OrderStatus = "Neu";
        }

        private void UpdateOrder()
        {
            if (OrderNumber.IsNullOrEmpty())
            {
                MessageBox.Show("Please enter an order number");
                return;
            }

            if (OrderStatus.IsNullOrEmpty())
            {
                MessageBox.Show("Please select an order status");
                return;
            }

            _currentOrder = SelectedGridItem;

            if (_currentOrder != null)
            {
                using (DataHandler _context = new DataHandler(_initializedDB = new DBInitialization()))
                {
                    OrderModel orderToChange = _context.GetData<OrderModel>(a => a.AuftragID == _currentOrder.AuftragID).FirstOrDefault();

                    if (orderToChange != null)
                    {
                        orderToChange.Auftragnummer = OrderNumber;
                        orderToChange.Auftragsstatus = OrderStatus;
                        orderToChange.DT_Modified = DateTime.Now;

                        _currentOrder.Auftragnummer = OrderNumber;
                        _currentOrder.Auftragsstatus = OrderStatus;
                        _currentOrder.DT_Modified = DateTime.Now;

                        _initializedDB.SaveChanges();
                    }

                    else
                    {
                        MessageBox.Show("This order does not exist in the Database. The list will be refreshed.");
                        CurrentOrderList = _pagination.RefreshView();
                    }
                }
            }
            _currentOrder = null;
        }

        private void DeleteOrder()
        {
            _currentOrder = SelectedGridItem;
            if (_currentOrder != null)
            {
                using (DataHandler _context = new DataHandler(_initializedDB = new DBInitialization()))
                {
                    if (_context.GetData<OrderModel>(a => a.AuftragID == _currentOrder.AuftragID).Any())
                    {
                        _initializedDB.Remove(_currentOrder);
                        _initializedDB.SaveChanges();
                    }

                    else
                    {
                        MessageBox.Show("This order does not exist in the Database. The list will be refreshed.");
                    }
                }
            }

            else
            {
                MessageBox.Show("Please select an order.");
            }

            CurrentOrderList = _pagination.RefreshView();

            OnPropertyChanged(nameof(Page));

            _currentOrder = null;
            SelectedGridItem = null;
            OrderNumber = null;
            OrderStatus = "Neu";
        }

        #region PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        internal void OnPropertyChanged(params object[] propertyNames)
        {
            string[] propertyNamesArray = new string[propertyNames.Length];
            try
            {
                for (int i = 0; i < propertyNamesArray.Length; i++)
                {
                    propertyNamesArray[i] = Convert.ToString(propertyNames[i]);
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyNamesArray[i]));
                }
            }

            catch (Exception ex)
            {
                throw new Exception("Can not convert property to String.");
            }
        }

        internal void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
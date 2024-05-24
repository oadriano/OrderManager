using System.Windows;
using System.Windows.Controls;
using OrderManager.ViewModel;
using OrderManager.ConstValues;

namespace OrderManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class OrderWindow : Window
    {
        public OrderWindow()
        {
            InitializeComponent();
            FocusMessenger.FocusRequested += OnFocusRequested;
        }

        private void OnFocusRequested(string target)
        {
            if (target == "txtOrderNumber")
            {
                txtOrderNumber.Focus();
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            FocusMessenger.FocusRequested -= OnFocusRequested;
        }

        private void AutoGenerateOrder(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            string headername = e.Column.Header.ToString();

            if (headername == ConstStrings.AUFTRAG_HEADER_VORGAENGEINAUFTRAG)
            {
                e.Cancel = true;
            }

            if (headername == ConstStrings.AUFTRAG_HEADER_AUFTRAGID)
            {
                e.Column.Header = "Order ID";
            }
            else if (headername == ConstStrings.AUFTRAG_HEADER_AUFTRAGNUMMER)
            {
                e.Column.Header = "Order number";
            }
            else if (headername == ConstStrings.AUFTRAG_HEADER_AUFTRAGSSTATUS)
            {
                e.Column.Header = "Order status";
            }
            else if (headername == ConstStrings.AUFTRAG_HEADER_HINZUGEFUEGT)
            {
                e.Column.Header = "Created";
            }
            else if (headername == ConstStrings.AUFTRAG_HEADER_BEARBEITET)
            {
                e.Column.Header = "Modified";
            }
        }
    }
}
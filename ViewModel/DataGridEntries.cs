using OrderManager.Model;
using OrderManager.Data;
using OrderManager.ConstValues;
using System.Collections.ObjectModel;
using System.Linq.Expressions;

namespace OrderManager.ViewModel;

internal class DataGridEntries
{
    public ObservableCollection<OrderModel> _ObservableCollectionOrderMain { get; set; }
    public DataGridEntries()
    {
        ShowDefaultView();
    }
    public ObservableCollection<OrderModel> ShowDefaultView()
    {
        using (DataHandler _context = new DataHandler(new DBInitialization()))
        {
            List<OrderModel> query = _context.GetData<OrderModel>(a => a.VorgaengeInAuftrag.All(v => v.Vorgangsstatus != ConstStrings.VORGANGSSTATUS_ABGESCHLOSSEN), null, 0, 5).ToList();
            _ObservableCollectionOrderMain = new ObservableCollection<OrderModel>(query);
        }
        return _ObservableCollectionOrderMain;
    }
}

using System.Collections.ObjectModel;

namespace OrderManager.Model
{
    public class OrderStatusModel : ObservableCollection<String>
    {
        public OrderStatusModel()
        {
            Add("Neu");
            Add("Freigegeben");
        }
    }
}

using System.Collections.ObjectModel;

namespace OrderManager.Model
{
    public class PageSizeModel : ObservableCollection<string>
    {
        public PageSizeModel()
        {
            Add("5");
            Add("10");
            Add("20");
            Add("50");
            Add("all");
        }
    }
}

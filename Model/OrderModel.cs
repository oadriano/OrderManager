using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace OrderManager.Model
{
    public class OrderModel : INotifyPropertyChanged
    {
        private string _auftragnummer;
        private string _auftragsstatus;
        private DateTime _dtModified;

        [Key]
        public int AuftragID { get; set; }
        //public string Auftragnummer { get; set; }
        public string Auftragnummer
        {
            get => _auftragnummer;
            set
            {
                if(_auftragnummer != value)
                {
                    _auftragnummer = value;
                    OnPropertyChanged(nameof(Auftragnummer));
                }
            }
        }
        public string Auftragsstatus
        {
            get => _auftragsstatus;
            set
            {
                if (_auftragsstatus != value)
                {
                    _auftragsstatus = value;
                    OnPropertyChanged(nameof(Auftragsstatus));
                }
            }
        }
        public DateTime DT_Created { get; set; }
        public DateTime DT_Modified
        {
            get => _dtModified;
            set
            {
                if (_dtModified != value)
                {
                    _dtModified = value;
                    OnPropertyChanged(nameof(DT_Modified));
                }
            }
        }

        public virtual ICollection<ProcessModel> VorgaengeInAuftrag { get; set; }

        #region PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
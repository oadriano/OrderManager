using System.ComponentModel.DataAnnotations;

namespace OrderManager.Model
{
    public class ProcessModel
    {
        public int Sortiert { get; set; }
        public int AuftragID { get; set; }
        [Key]
        public int VorgangsID { get; set; }
        public string Vorgangsbezeichnung { get; set; }
        public string Vorgangsstatus { get; set; }
        public DateTime DT_Created { get; set; }
        public DateTime DT_Modified { get; set; }
        public virtual OrderModel AuftragInVorgang { get; set; }
    }
}
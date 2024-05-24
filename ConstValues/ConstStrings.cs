/// This class contains constant Strings for the Implementation in my project: OrderManager
namespace OrderManager.ConstValues
{
    public static class ConstStrings
    {
        // ConnectionString for SQL Connection ---------------------------------------------------------------------------
        public const string CONNECTIONSTRING = "";
        //----------------------------------------------------------------------------------------------------------------


        // Textboxes -----------------------------------------------------------------------------------------------------
        public const string MSG_VORGAENGE_ENTHALTEN = "Löschen nicht möglich, weil der Auftrag noch Vorgaenge enthält.";
        public const string MSG_AUFTRAG_AUSWAEHLEN = "Bitte wählen Sie einen Auftrag aus.";
        public const string MSG_VORGANG_AUSWAEHLEN = "Bitte wählen Sie einen Vorgang aus.";
        public const string MSG_AUFTRAG_ANLEGEN = "Auftrag anlegen";
        //----------------------------------------------------------------------------------------------------------------


        // Properties for order ------------------------------------------------------------------------------------------
        public const string AUFTRAG_HEADER_AUFTRAGID = "AuftragID";
        public const string AUFTRAG_HEADER_AUFTRAGNUMMER = "Auftragnummer";
        public const string AUFTRAG_HEADER_AUFTRAGSSTATUS = "Auftragsstatus";
        public const string AUFTRAG_HEADER_HINZUGEFUEGT = "DT_Created";
        public const string AUFTRAG_HEADER_BEARBEITET = "DT_Modified";
        public const string AUFTRAG_HEADER_VORGAENGEINAUFTRAG = "VorgaengeInAuftrag";
        //----------------------------------------------------------------------------------------------------------------


        // Properties for process ----------------------------------------------------------------------------------------
        public const string VORGANG_HEADER_SORTIERT = "Sortiert";
        public const string VORGANG_HEADER_AUFTRAGID = "AuftragID";
        public const string VORGANG_HEADER_VORGANGSID = "VorgangsID";
        public const string VORGANG_HEADER_VORGANGSBEZEICHNUNG = "Vorgangsbezeichnung";
        public const string VORGANG_HEADER_VORGANGSSTATUS = "Vorgangsstatus";
        public const string VORGANG_HEADER_HINZUGEFUEGT = "DT_Created";
        public const string VORGANG_HEADER_BEARBEITET = "DT_Modified";
        public const string VORGANG_HEADER_AUFTRAGINVORGANG = "AuftragInVorgang";
        //----------------------------------------------------------------------------------------------------------------


        // Status for order ----------------------------------------------------------------------------------------------
        public const string AUFTRAGSSTATUS_NEU = "Neu";
        public const string AUFTRAGSSTATUS_FREIGEGEBEN = "Freigegeben";
        //----------------------------------------------------------------------------------------------------------------


        // Status for process --------------------------------------------------------------------------------------------
        public const string VORGANGSSTATUS_OFFEN = "Offen";
        public const string VORGANGSSTATUS_BEGONNEN = "Begonnen";
        public const string VORGANGSSTATUS_ABGESCHLOSSEN = "Abgeschlossen";
        //----------------------------------------------------------------------------------------------------------------
    }
}
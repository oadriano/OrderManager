CREATE VIEW OffeneAuftraege AS
SELECT * FROM Auftraege as A
INNER JOIN Vorgaenge AS V ON A.AuftragID = V.AuftragID
WHERE Vorgangsstatus != 'Abgeschlossen'
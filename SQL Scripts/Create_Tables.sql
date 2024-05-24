CREATE TABLE Auftraege (
	AuftragID INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	Auftragnummer NVARCHAR(25),
	Auftragsstatus NVARCHAR(15) NOT NULL DEFAULT 'Neu' CHECK (Auftragsstatus IN ('Neu', 'Freigegeben')),
	DT_Created DATETIME2 DEFAULT GETDATE(),
    DT_Modified DATETIME2 DEFAULT GETDATE()
);

CREATE TABLE Vorgaenge (
	Sortiert INT,
	AuftragID INT NOT NULL,
	VorgangsID INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	Vorgangsbezeichnung NVARCHAR(255),
	Vorgangsstatus NVARCHAR(15) NOT NULL DEFAULT 'Offen' CHECK (Vorgangsstatus IN ('Offen', 'Begonnen', 'Abgeschlossen')),
	DT_Created DATETIME2 DEFAULT GETDATE(),
    DT_Modified DATETIME2 DEFAULT GETDATE(),
	FOREIGN KEY (AuftragID) REFERENCES Auftraege(AuftragID)
);

GO

-- Trigger für Aufträge

CREATE TRIGGER Auftraege_Insert
ON Auftraege
AFTER INSERT
AS
BEGIN
    UPDATE Auftraege
    SET DT_Created = GETDATE(), DT_Modified = GETDATE()
    FROM Auftraege
	WHERE Auftraege.AuftragID = (SELECT AuftragID FROM inserted)
END;

GO

CREATE TRIGGER Auftraege_Update
ON Auftraege
AFTER UPDATE
AS
BEGIN
    UPDATE Auftraege
    SET DT_Modified = GETDATE()
    FROM Auftraege
	WHERE Auftraege.AuftragID = (SELECT AuftragID FROM inserted)
END;

GO

CREATE TRIGGER tr_Auftraege_Delete
ON Auftraege
INSTEAD OF DELETE
AS
BEGIN
    IF EXISTS (SELECT AuftragID FROM Vorgaenge WHERE AuftragID IN (SELECT AuftragID FROM deleted))
    BEGIN
        ROLLBACK;
		RETURN;
    END;

    ELSE
    BEGIN
        DELETE FROM Auftraege WHERE AuftragID IN (SELECT AuftragID FROM deleted);
    END;
END;

GO

-- Trigger für Vorgänge

CREATE TRIGGER Vorgaenge_Insert
ON Vorgaenge
AFTER INSERT
AS
BEGIN

--=======================================================================================================
-- NICHT NÖTIG, DA SCHON MIT DEM FOREIGN KEY ABGEFANGEN, STEIGERT ABER INTEGRITÄT========================
--    IF NOT EXISTS (SELECT AuftragID FROM Auftraege WHERE AuftragID IN (SELECT AuftragID FROM inserted))
--    BEGIN
--      ROLLBACK;
--		RETURN;
--  END;
--=======================================================================================================

	UPDATE Vorgaenge
	SET DT_Created = GETDATE(), DT_Modified = GETDATE()
	FROM Vorgaenge V
	--WHERE Vorgaenge.VorgangsID = (SELECT VorgangsID FROM inserted)
	INNER JOIN inserted I ON V.VorgangsID = I.VorgangsID
END;

GO

CREATE TRIGGER Vorgaenge_Update
ON Vorgaenge
AFTER UPDATE
AS
BEGIN
	UPDATE Vorgaenge
	SET DT_Modified = GETDATE()
	FROM Vorgaenge V
	--WHERE Vorgaenge.VorgangsID = (SELECT VorgangsID FROM inserted)
	INNER JOIN inserted I ON V.VorgangsID = I.VorgangsID
END;

GO

--	Sortierung jeweils nach UPDATE, DELETE und INSERTED

CREATE TRIGGER InsertSortierungVorgaenge ON Vorgaenge
AFTER INSERT
AS

BEGIN
	;WITH CTE AS
	(
		SELECT VorgangsID, AuftragID,
			   --ROW_NUMBER() OVER (PARTITION BY AuftragID ORDER BY AuftragID, VorgangsID) AS Sortiert
			   ROW_NUMBER() OVER (PARTITION BY AuftragID ORDER BY VorgangsID) AS Sortiert
		FROM Vorgaenge
	)
	UPDATE Vorgaenge
	SET Sortiert = CTE.Sortiert
	FROM Vorgaenge
	INNER JOIN CTE ON Vorgaenge.VorgangsID = CTE.VorgangsID
END

GO

CREATE TRIGGER DeleteSortierungVorgaenge ON Vorgaenge
AFTER DELETE
AS

BEGIN
	;WITH CTE AS
	(
		SELECT VorgangsID, AuftragID,
			   ROW_NUMBER() OVER (PARTITION BY AuftragID ORDER BY AuftragID, VorgangsID) AS Sortiert
		FROM Vorgaenge
	)
	UPDATE Vorgaenge
	SET Sortiert = CTE.Sortiert
	FROM Vorgaenge
	INNER JOIN CTE ON Vorgaenge.VorgangsID = CTE.VorgangsID
END

GO

CREATE TRIGGER UpdateSortierungVorgaenge ON Vorgaenge
AFTER DELETE
AS

BEGIN
	;WITH CTE AS
	(
		SELECT VorgangsID, AuftragID,
			   ROW_NUMBER() OVER (PARTITION BY AuftragID ORDER BY AuftragID, VorgangsID) AS Sortiert
		FROM Vorgaenge
	)
	UPDATE Vorgaenge
	SET Sortiert = CTE.Sortiert
	FROM Vorgaenge
	INNER JOIN CTE ON Vorgaenge.VorgangsID = CTE.VorgangsID
END

GO
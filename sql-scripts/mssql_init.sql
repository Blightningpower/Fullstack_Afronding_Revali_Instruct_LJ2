-- Verwijder database indien al aanwezig
DROP DATABASE revali_db;

-- Maak database aan indien nog niet aanwezig (UTF8mb4 voor goede Unicode-ondersteuning)
CREATE DATABASE IF NOT EXISTS revali_db CHARACTER SET = 'utf8mb4' COLLATE = 'utf8mb4_unicode_ci';
USE revali_db;

-- Tabel: Users (gebruikersaccounts)
CREATE TABLE Users (
  Id INT AUTO_INCREMENT PRIMARY KEY COMMENT 'Primaire sleutel voor gebruiker',
  Username VARCHAR(100) NOT NULL UNIQUE COMMENT 'Unieke gebruikersnaam / inlognaam',
  PasswordHash VARCHAR(255) NOT NULL COMMENT 'BCrypt-hash van het wachtwoord',
  Role VARCHAR(50) NOT NULL COMMENT 'Rol van de gebruiker (bijv. revalidatiearts, huisarts)',
  DisplayName VARCHAR(200) COMMENT 'Weergavenaam voor UI',
  CreatedAt DATETIME(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) COMMENT 'Datum en tijd van aanmaken'
) ENGINE=InnoDB COMMENT='Tabel met gebruikersaccounts';

-- Tabel: Patients (patiënten)
CREATE TABLE Patients (
  Id INT AUTO_INCREMENT PRIMARY KEY COMMENT 'Primaire sleutel voor patiënt',
  FirstName VARCHAR(100) NOT NULL COMMENT 'Voornaam patiënt',
  LastName VARCHAR(100) NOT NULL COMMENT 'Achternaam patiënt',
  DateOfBirth DATE NOT NULL COMMENT 'Geboortedatum patiënt',
  Notes TEXT COMMENT 'Vrijveld voor aanvullende aantekeningen (medische notities etc.)',
  CreatedAt DATETIME(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) COMMENT 'Datum en tijd van aanmaken'
) ENGINE=InnoDB COMMENT='Tabel met patiëntgegevens';

-- Tabel: RevalidationSessions (consulten / sessies)
CREATE TABLE RevalidationSessions (
  Id INT AUTO_INCREMENT PRIMARY KEY COMMENT 'Primaire sleutel voor sessie',
  PatientId INT NOT NULL COMMENT 'FK naar patiënt',
  SessionDate DATETIME(6) NOT NULL COMMENT 'Datum en tijd van sessie',
  Notes TEXT COMMENT 'Notities van de sessie / intake',
  Cost DECIMAL(10,2) DEFAULT 0 COMMENT 'Kosten gekoppeld aan deze sessie',
  CreatedAt DATETIME(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) COMMENT 'Datum en tijd van aanmaken',
  CONSTRAINT FK_RevalSession_Patient FOREIGN KEY (PatientId) REFERENCES Patients(Id) ON DELETE RESTRICT
) ENGINE=InnoDB COMMENT='Tabel met revalidatie-sessies (intake, controle)';

-- Tabel: ExerciseLogs (uitvoeringen van oefeningen)
CREATE TABLE ExerciseLogs (
  Id INT AUTO_INCREMENT PRIMARY KEY COMMENT 'Primaire sleutel voor oefening-registratie',
  PatientId INT NOT NULL COMMENT 'FK naar patiënt',
  PerformedAt DATETIME(6) NOT NULL COMMENT 'Datum en tijd waarop oefening is uitgevoerd',
  Exercise VARCHAR(255) NOT NULL COMMENT 'Naam/omschrijving van de oefening',
  Repetitions INT DEFAULT 0 COMMENT 'Aantal herhalingen',
  Notes TEXT COMMENT 'Extra opmerkingen over uitvoering',
  CONSTRAINT FK_Exercise_Patient FOREIGN KEY (PatientId) REFERENCES Patients(Id) ON DELETE RESTRICT
) ENGINE=InnoDB COMMENT='Log van uitgevoerde oefeningen door patiënten';

-- Tabel: PainLogs (pijnregistratie 0-10)
CREATE TABLE PainLogs (
  Id INT AUTO_INCREMENT PRIMARY KEY COMMENT 'Primaire sleutel voor pijnregistratie',
  PatientId INT NOT NULL COMMENT 'FK naar patiënt',
  RecordedAt DATETIME(6) NOT NULL COMMENT 'Datum en tijd van registratie',
  PainScore TINYINT NOT NULL COMMENT 'Pijnscore (0-10)',
  Notes TEXT COMMENT 'Extra toelichting bij pijnscore',
  CONSTRAINT CK_PainScore_Range CHECK (PainScore BETWEEN 0 AND 10),
  CONSTRAINT FK_Pain_Patient FOREIGN KEY (PatientId) REFERENCES Patients(Id) ON DELETE RESTRICT
) ENGINE=InnoDB COMMENT='Tabel met pijnscores en toelichting';

-- Tabel: Medication (medicatie)
CREATE TABLE Medication (
  Id INT AUTO_INCREMENT PRIMARY KEY COMMENT 'Primaire sleutel voor medicatie-item',
  PatientId INT NOT NULL COMMENT 'FK naar patiënt',
  Name VARCHAR(255) NOT NULL COMMENT 'Naam van medicatie',
  Dosage VARCHAR(100) NOT NULL COMMENT 'Dosering (bv. 2x per dag)',
  StartDate DATE NOT NULL COMMENT 'Startdatum gebruik',
  EndDate DATE COMMENT 'Einddatum gebruik (optioneel)',
  CONSTRAINT FK_Med_Patient FOREIGN KEY (PatientId) REFERENCES Patients(Id) ON DELETE RESTRICT
) ENGINE=InnoDB COMMENT='Tabel met voorgeschreven medicatie';

-- Tabel: Declarations (declaraties / claims)
CREATE TABLE Declarations (
  Id INT AUTO_INCREMENT PRIMARY KEY COMMENT 'Primaire sleutel voor declaratie',
  PatientId INT NOT NULL COMMENT 'FK naar patiënt',
  Date DATETIME(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) COMMENT 'Datum en tijd van declaratie',
  Amount DECIMAL(10,2) NOT NULL COMMENT 'Bedrag van de declaratie',
  Status ENUM('Concept','Ingediend','Betaald','Afgewezen') NOT NULL DEFAULT 'Concept' COMMENT 'Status van de declaratie',
  CONSTRAINT FK_Decl_Patient FOREIGN KEY (PatientId) REFERENCES Patients(Id) ON DELETE RESTRICT
) ENGINE=InnoDB COMMENT='Tabel voor declaraties richting zorgverzekeraar';

-- Tabel: AuditEntries (audit / historiek)
CREATE TABLE AuditEntries (
  Id INT AUTO_INCREMENT PRIMARY KEY COMMENT 'Primaire sleutel voor auditrecord',
  TableName VARCHAR(200) NOT NULL COMMENT 'Tabel waarop actie is uitgevoerd',
  KeyValues JSON NOT NULL COMMENT 'Sleutelwaarden van het gewijzigde record (in JSON)',
  Action VARCHAR(20) NOT NULL COMMENT 'Actie: INSERT, UPDATE, DELETE',
  OldValues JSON COMMENT 'Oude waarden (voor UPDATE/DELETE) in JSON',
  NewValues JSON COMMENT 'Nieuwe waarden (voor INSERT/UPDATE) in JSON',
  UserId VARCHAR(100) COMMENT 'Gebruiker die de wijziging veroorzaakte (optioneel)',
  Timestamp DATETIME(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) COMMENT 'Tijdstip van de audit-actie'
) ENGINE=InnoDB COMMENT='Audittrail voor wijzigingen in de database';

-- Indexes (verbeter zoek- & filterprestaties op veelgebruikte kolommen)
CREATE INDEX IX_RevalSession_PatientId ON RevalidationSessions (PatientId);
CREATE INDEX IX_Exercise_PatientId ON ExerciseLogs (PatientId);
CREATE INDEX IX_Pain_PatientId_RecordedAt ON PainLogs (PatientId, RecordedAt);
CREATE INDEX IX_Med_PatientId ON Medication (PatientId);
CREATE INDEX IX_Decl_PatientId ON Declarations (PatientId);
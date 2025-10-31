USE revali_db;

-- Voorbeeldgebruikers (testaccounts)
INSERT INTO Users (Username, PasswordHash, Role, DisplayName)
VALUES
  ('revalarts', '$2a$12$tYWKM3qVgNn5xk6exkJz4.GS3Om4rALLQGeGkEY63Tbft4zV.bJBm', 'revalidatiearts', 'Dr. Revali'),
  ('huisarts',  '$2a$12$NVksrIX69QKX0PFSqz.MrOql8sqQ3mnnVmkzvXB5Hmehw5zI9f4..', 'huisarts', 'Huisarts Jansen');

-- Voorbeeldpatiënt
INSERT INTO Patients (FirstName, LastName, DateOfBirth, Notes)
VALUES ('Freddy', 'Voetbal', '1995-01-01', 'Patiënt met gescheurde kruisbanden en open wond');

-- Voorbeeld revalidatie-sessie
INSERT INTO RevalidationSessions (PatientId, SessionDate, Notes, Cost)
VALUES (1, NOW(), 'Eerste intake en opstellen oefenprogramma', 75.00);

-- Voorbeeld oefenlog
INSERT INTO ExerciseLogs (PatientId, PerformedAt, Exercise, Repetitions, Notes)
VALUES (1, NOW(), 'Kniebuigingen met weerstand', 15, 'Let op pijn in 2e set');

-- Voorbeeld pijnlog
INSERT INTO PainLogs (PatientId, RecordedAt, PainScore, Notes)
VALUES (1, NOW(), 6, 'Pijn direct na oefening');

-- Voorbeeld medicatie
INSERT INTO Medication (PatientId, Name, Dosage, StartDate, EndDate)
VALUES (1, 'Amoxicilline', '500 mg, 3x/dag', '2025-10-01', NULL);

-- Voorbeeld declaratie
INSERT INTO Declarations (PatientId, Date, Amount, Status)
VALUES (1, NOW(), 75.00, 'Concept');
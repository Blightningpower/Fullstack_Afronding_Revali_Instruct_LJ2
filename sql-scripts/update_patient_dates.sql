-- Update alle startdatums met random datums in 2024/2025
UPDATE Patients 
SET StartDate = DATEADD(DAY, ABS(CHECKSUM(NEWID()) % 730), '2024-01-01')
WHERE StartDate IS NULL OR StartDate < '2020-01-01';

-- Optioneel: Update specifieke patiënten met vaste datums
UPDATE Patients SET StartDate = '2024-01-15' WHERE Id = 1;
UPDATE Patients SET StartDate = '2024-03-22' WHERE Id = 2;

-- Voeg geboortedatums toe (tussen 1950 en 2005)
UPDATE Patients 
SET DateOfBirth = DATEADD(DAY, ABS(CHECKSUM(NEWID()) % 20000), '1950-01-01')
WHERE DateOfBirth IS NULL;

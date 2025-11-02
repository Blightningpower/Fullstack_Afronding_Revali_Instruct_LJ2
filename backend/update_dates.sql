-- Update startdatums met random datums in 2024/2025
UPDATE Patients 
SET StartDate = DATEADD(DAY, ABS(CHECKSUM(NEWID()) % 365), '2024-01-01')
WHERE StartDate IS NULL OR StartDate = '0001-01-01';

-- Optioneel: voeg ook geboortedatums toe (tussen 1950 en 2000)
UPDATE Patients 
SET BirthDate = DATEADD(DAY, ABS(CHECKSUM(NEWID()) % 18250), '1950-01-01')
WHERE BirthDate IS NULL;

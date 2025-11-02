-- Fix specifieke datums voor Jan Jansen en Maria Bakker
UPDATE Patients SET StartDate = '2024-01-15' WHERE Id = 1 AND FirstName = 'Jan';
UPDATE Patients SET StartDate = '2024-03-22' WHERE Id = 2 AND FirstName = 'Maria';

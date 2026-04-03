USE HospitalManagementDb;
GO

INSERT INTO Patient (FirstName, LastName, CNP, DateOfBirth, Sex, Phone, EmergencyContact, Archived, IsDonor)
VALUES ('Andrei', 'Suspectu', '1990010199999', '1990-01-01', 'M', '+40722000111', 'Elena Suspectu', 0, 0);

DECLARE @PatientID INT = SCOPE_IDENTITY();


INSERT INTO MedicalHistory (PatientID, BloodType, RH, ChronicConditions)
VALUES (@PatientID, 'AB', 'Positive', 'No documented chronic pain');

DECLARE @HistoryID INT = SCOPE_IDENTITY();


INSERT INTO MedicalRecord (HistoryID, SourceType, SourceID, StaffID, Symptoms, Diagnosis, BasePrice, FinalPrice, PoliceNotified)
VALUES (@HistoryID, 'Appointment', 101, 101, 'Severe back pain', 'Acute pain', 100, 100, 0);
DECLARE @R1 INT = SCOPE_IDENTITY();

INSERT INTO MedicalRecord (HistoryID, SourceType, SourceID, StaffID, Symptoms, Diagnosis, BasePrice, FinalPrice, PoliceNotified)
VALUES (@HistoryID, 'Appointment', 102, 102, 'Unbearable back pain', 'Muscle spasm', 100, 100, 0);
DECLARE @R2 INT = SCOPE_IDENTITY();

INSERT INTO MedicalRecord (HistoryID, SourceType, SourceID, StaffID, Symptoms, Diagnosis, BasePrice, FinalPrice, PoliceNotified)
VALUES (@HistoryID, 'ER Visit', 103, 103, 'Needs pain meds', 'Lumbar pain', 200, 200, 0);
DECLARE @R3 INT = SCOPE_IDENTITY();

INSERT INTO MedicalRecord (HistoryID, SourceType, SourceID, StaffID, Symptoms, Diagnosis, BasePrice, FinalPrice, PoliceNotified)
VALUES (@HistoryID, 'Appointment', 104, 104, 'Back injury', 'Pain management', 100, 100, 0);
DECLARE @R4 INT = SCOPE_IDENTITY();

INSERT INTO Prescription (RecordID, DoctorNotes) VALUES (@R1, 'Urgent refill'), (@R2, 'Patient requested X'), (@R3, 'Second opinion pain'), (@R4, 'Emergency refill');

DECLARE @P1 INT = (SELECT PrescriptionID FROM Prescription WHERE RecordID = @R1);
DECLARE @P2 INT = (SELECT PrescriptionID FROM Prescription WHERE RecordID = @R2);
DECLARE @P3 INT = (SELECT PrescriptionID FROM Prescription WHERE RecordID = @R3);
DECLARE @P4 INT = (SELECT PrescriptionID FROM Prescription WHERE RecordID = @R4);

INSERT INTO PrescriptionItems (PrescriptionID, MedName, Quantity)
VALUES 
(@P1, 'Oxycodone', '20mg'),
(@P2, 'Oxycodone', '20mg'),
(@P3, 'Oxycodone', '20mg'),
(@P4, 'Oxycodone', '20mg');

GO


SELECT 
    p.FirstName, p.LastName, 
    pi.MedName, 
    COUNT(DISTINCT mr.StaffID) as DifferentDoctors,
    COUNT(pr.PrescriptionID) as TotalPrescriptions,
    mh.ChronicConditions
FROM Patient p
JOIN MedicalHistory mh ON p.PatientID = mh.PatientID
JOIN MedicalRecord mr ON mh.HistoryID = mr.HistoryID
JOIN Prescription pr ON mr.RecordID = pr.RecordID
JOIN PrescriptionItems pi ON pr.PrescriptionID = pi.PrescriptionID
GROUP BY p.FirstName, p.LastName, pi.MedName, mh.ChronicConditions
HAVING COUNT(pr.PrescriptionID) > 3 AND COUNT(DISTINCT mr.StaffID) > 3;


UPDATE mh
SET mh.ChronicConditions = 'No chronic pain conditions. History of anxiety. Patient frequently requests specific analgesics.'
FROM MedicalHistory mh
JOIN Patient p ON mh.PatientID = p.PatientID
WHERE p.CNP = '1990010199999';

SELECT p.FirstName, p.LastName, mh.ChronicConditions
FROM Patient p
JOIN MedicalHistory mh ON p.PatientID = mh.PatientID
WHERE p.CNP = '1990010199999';
GO


INSERT INTO Patient (FirstName, LastName, CNP, DateOfBirth, Sex, Phone, EmergencyContact, Archived, IsDonor)
VALUES ('Maria', 'Ionescu', '2850505223344', '1985-05-05', 'F', '+40733111222', 'George Ionescu', 0, 0);

DECLARE @PID INT = SCOPE_IDENTITY();

INSERT INTO MedicalHistory (PatientID, BloodType, RH, ChronicConditions)
VALUES (@PID, 'O', 'Positive', 'Chronic Lower Back Pain (L4-L5 Herniated Disc), History of Spinal Surgery 2022.');

DECLARE @HID INT = SCOPE_IDENTITY();


INSERT INTO MedicalRecord (HistoryID, SourceType, SourceID, StaffID, Symptoms, Diagnosis, BasePrice, FinalPrice, PoliceNotified)
VALUES (@HID, 'Appointment', 201, 101, 'Sharp back pain', 'Chronic pain flare-up', 150, 150, 0);
DECLARE @Rec1 INT = SCOPE_IDENTITY();

INSERT INTO MedicalRecord (HistoryID, SourceType, SourceID, StaffID, Symptoms, Diagnosis, BasePrice, FinalPrice, PoliceNotified)
VALUES (@HID, 'ER Visit', 202, 102, 'Cannot walk due to pain', 'Radiculopathy', 300, 300, 0);
DECLARE @Rec2 INT = SCOPE_IDENTITY();

INSERT INTO MedicalRecord (HistoryID, SourceType, SourceID, StaffID, Symptoms, Diagnosis, BasePrice, FinalPrice, PoliceNotified)
VALUES (@HID, 'Appointment', 203, 105, 'Pain medication refill', 'Chronic Pain Management', 150, 150, 0);
DECLARE @Rec3 INT = SCOPE_IDENTITY();

INSERT INTO MedicalRecord (HistoryID, SourceType, SourceID, StaffID, Symptoms, Diagnosis, BasePrice, FinalPrice, PoliceNotified)
VALUES (@HID, 'Appointment', 204, 106, 'Severe discomfort', 'Post-op complications', 150, 150, 0);
DECLARE @Rec4 INT = SCOPE_IDENTITY();

INSERT INTO Prescription (RecordID, DoctorNotes) VALUES (@Rec1, 'Refill'), (@Rec2, 'Emergency dose'), (@Rec3, 'Patient claims lost meds'), (@Rec4, 'Follow up');

INSERT INTO PrescriptionItems (PrescriptionID, MedName, Quantity)
VALUES 
(SCOPE_IDENTITY()-3, 'Tramadol', '50mg x 20 caps'),
(SCOPE_IDENTITY()-2, 'Tramadol', '50mg x 20 caps'),
(SCOPE_IDENTITY()-1, 'Tramadol', '50mg x 20 caps'),
(SCOPE_IDENTITY(), 'Tramadol', '50mg x 20 caps');
GO

select * from MedicalRecord
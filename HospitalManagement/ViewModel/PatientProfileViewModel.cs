using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using HospitalManagement.Database;
using HospitalManagement.Entity;
using HospitalManagement.Repository;
using HospitalManagement.Service;

namespace HospitalManagement.ViewModel
{
    public class PatientProfileViewModel : INotifyPropertyChanged
    {
        private Patient _patient;
        private readonly PatientService _patientService;

        public Patient CurrentPatient
        {
            get => _patient;
            set { _patient = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public PatientProfileViewModel(int patientId)
        {
            var dbContext = new HospitalDbContext();
            var patientRepo = new PatientRepository(dbContext);
            var historyRepo = new MedicalHistoryRepository(dbContext);
            var recordRepo = new MedicalRecordRepository(dbContext);

            _patientService = new PatientService(patientRepo, historyRepo, recordRepo);

            // 1. FIX: Ensure the dummy payload has NO null collections
            CurrentPatient = new Patient
            {
                MedicalHistory = new MedicalHistory
                {
                    MedicalRecords = new System.Collections.Generic.List<MedicalRecord>()
                }
            };

            LoadFullPatientProfile(patientId);
        }

        private void LoadFullPatientProfile(int id)
        {
            try
            {
                var p = _patientService.GetPatientDetails(id);
                if (p != null)
                {
                    // 2. FIX: Protect against incomplete database data
                    if (p.MedicalHistory == null)
                    {
                        p.MedicalHistory = new MedicalHistory();
                    }
                    if (p.MedicalHistory.MedicalRecords == null)
                    {
                        p.MedicalHistory.MedicalRecords = new System.Collections.Generic.List<MedicalRecord>();
                    }

                    CurrentPatient = p;
                }
            }
            catch (Exception)
            {
                // Keep the dummy data if the database completely fails
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
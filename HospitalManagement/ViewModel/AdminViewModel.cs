using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input; // Required for ICommand
using HospitalManagement.Entity;
using HospitalManagement.Integration;
using HospitalManagement.Service;

namespace HospitalManagement.ViewModel
{
    public class AdminViewModel : INotifyPropertyChanged
    {
        private readonly PatientService _patientService;

        // --- Properties bound to the View ---
        public ObservableCollection<Patient> Patients { get; set; }

        public ObservableCollection<Patient> ArchivedPatients { get; set; }

        // --- Commands bound to the View Buttons ---
        public ICommand LoadAllPatientsCommand { get; }

        public ICommand LoadArchivedPatientsCommand { get; }

        // --- Constructor ---
        public AdminViewModel(PatientService patientService)
        {
            _patientService = patientService;

            Patients = new ObservableCollection<Patient>();
            LoadAllPatientsCommand = new RelayCommand(LoadAllPatients);

            ArchivedPatients = new ObservableCollection<Patient>();
            LoadArchivedPatientsCommand = new RelayCommand(LoadArchivedPatients);


        }

        // --- VM6: Load All Patients (The Method) ---
        public void LoadAllPatients()
        {
            var emptyFilter = new PatientFilter();
            var allPatients = _patientService.SearchPatients(emptyFilter);

            Patients.Clear();

            var activePatients = allPatients.Where(p => p.IsArchived == false);

            foreach (var patient in activePatients)
            {
                patient.PhoneNo = FormatPhoneNumber(patient.PhoneNo);
                patient.EmergencyContact = FormatPhoneNumber(patient.EmergencyContact);
                Patients.Add(patient);
            }
        }

        // --- Helper: Phone Number Formatter ---
        private string FormatPhoneNumber(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone)) return phone;
            phone = phone.Replace(" ", "").Replace("-", "");
            if (phone.StartsWith("0") && phone.Length == 10)
            {
                return $"+40 {phone.Substring(1, 3)} {phone.Substring(4, 3)} {phone.Substring(7, 3)}";
            }
            return phone;
        }

        // --- VM7: Load Archived Patients ---
        public void LoadArchivedPatients()
        {
            // 1. Fetch ALL patients using an empty filter
            var emptyFilter = new PatientFilter();
            var allPatients = _patientService.SearchPatients(emptyFilter);

            // 2. Clear the UI list to prevent duplicates
            ArchivedPatients.Clear();

            // 3. Filter archived patients in-memory using LINQ
            var archivedList = allPatients.Where(p => p.IsArchived == true);

            // 4. Format phone numbers and add to the ObservableCollection
            foreach (var patient in archivedList)
            {
                patient.PhoneNo = FormatPhoneNumber(patient.PhoneNo);
                patient.EmergencyContact = FormatPhoneNumber(patient.EmergencyContact);

                ArchivedPatients.Add(patient);
            }
        }

        // --- INotifyPropertyChanged Implementation ---
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
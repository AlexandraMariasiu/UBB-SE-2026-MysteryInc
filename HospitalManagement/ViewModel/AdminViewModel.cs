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

        public Patient NewPatient { get; set; }

        // --- Validation Errors (For the red labels) ---
        public ObservableCollection<string> ValidationErrors { get; set; }

        private string _cnpError;
        public string CnpError { get => _cnpError; set { _cnpError = value; OnPropertyChanged(); } }

        private string _phoneError;
        public string PhoneError { get => _phoneError; set { _phoneError = value; OnPropertyChanged(); } }

        private string _dobError;
        public string DobError { get => _dobError; set { _dobError = value; OnPropertyChanged(); } }

        // --- The Close Window Notification ---
        public Action CloseAddPatientWindow { get; set; }

        // --- Commands bound to the View Buttons ---
        public ICommand LoadAllPatientsCommand { get; }

        public ICommand LoadArchivedPatientsCommand { get; }

        public ICommand AddPatientCommand { get; }

        // --- Constructor ---
        public AdminViewModel(PatientService patientService)
        {
            _patientService = patientService;

            Patients = new ObservableCollection<Patient>();
            LoadAllPatientsCommand = new RelayCommand(LoadAllPatients);

            ArchivedPatients = new ObservableCollection<Patient>();
            LoadArchivedPatientsCommand = new RelayCommand(LoadArchivedPatients);

            NewPatient = new Patient { Dob = DateTime.Today };
            ValidationErrors = new ObservableCollection<string>();
            AddPatientCommand = new RelayCommand(AddPatient);


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

        // --- VM8: Add Patient Logic ---
        private void AddPatient()
        {
            // 1. Reset previous errors
            ValidationErrors.Clear();
            CnpError = string.Empty;
            PhoneError = string.Empty;
            DobError = string.Empty;
            bool isValid = true;

            // 2. Validate CNP (13 digits + Secondary Service Check)
            if (string.IsNullOrWhiteSpace(NewPatient.Cnp) || NewPatient.Cnp.Length != 13 || !NewPatient.Cnp.All(char.IsDigit))
            {
                CnpError = "CNP must be exactly 13 digits.";
                ValidationErrors.Add(CnpError);
                isValid = false;
            }
            else if (!_patientService.ValidateCNP(NewPatient.Cnp, NewPatient.Sex, NewPatient.Dob)) // Secondary check
            {
                CnpError = "CNP does not match the selected sex or date of birth.";
                ValidationErrors.Add(CnpError);
                isValid = false;
            }

            // 3. Validate Phone (10 digits)
            if (string.IsNullOrWhiteSpace(NewPatient.PhoneNo) || NewPatient.PhoneNo.Length != 10 || !NewPatient.PhoneNo.All(char.IsDigit))
            {
                PhoneError = "Phone must be exactly 10 digits.";
                ValidationErrors.Add(PhoneError);
                isValid = false;
            }

            // 4. Validate Date of Birth (Must be in the past)
            if (NewPatient.Dob >= DateTime.Today)
            {
                DobError = "Date of Birth must be in the past.";
                ValidationErrors.Add(DobError);
                isValid = false;
            }

            // 5. Check if we should stop
            if (!isValid)
            {
                // We update this property to force the UI to refresh the error list
                OnPropertyChanged(nameof(ValidationErrors));
                return;
            }

            // --- SUCCESS PATH ---

            // 6. Map and Create (Assumes your teammate named the method CreatePatient)
            _patientService.CreatePatient(NewPatient);

            // 7. Refresh the main patient list so the new patient appears instantly
            LoadAllPatients();

            // 8. Reset the form for the next time it opens
            NewPatient = new Patient { Dob = DateTime.Today };
            OnPropertyChanged(nameof(NewPatient));

            // 9. Trigger the CloseWindow notification
            CloseAddPatientWindow?.Invoke();
        }

        // --- INotifyPropertyChanged Implementation ---
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
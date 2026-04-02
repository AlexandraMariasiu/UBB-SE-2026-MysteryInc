using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using HospitalManagement.Entity;
using HospitalManagement.Integration;
using HospitalManagement.Service;
using HospitalManagement.Database;
using HospitalManagement.Repository;

namespace HospitalManagement.ViewModel
{
    // Inherit from INotifyPropertyChanged so the UI knows when to update
    public class MedicalStaffViewModel : INotifyPropertyChanged
    {
        private string _searchQuery = string.Empty;
        private string _errorMessage = string.Empty;
        private ObservableCollection<Patient> _searchResults = new ObservableCollection<Patient>();
        private readonly PatientService _patientService;
        private Patient _selectedPatient;

        public Patient SelectedPatient
        {
            get => _selectedPatient;
            set
            {
                _selectedPatient = value;
                OnPropertyChanged();
            }
        }
        

        public string SearchQuery
        {
            get => _searchQuery;
            set { _searchQuery = value; OnPropertyChanged(); }
        }

        // Binds to the error message text
        public string ErrorMessage
        {
            get => _errorMessage;
            set { _errorMessage = value; OnPropertyChanged(); }
        }

        // Binds to the results table
        public ObservableCollection<Patient> SearchResults
        {
            get => _searchResults;
            set { _searchResults = value; OnPropertyChanged(); }
        }

        public ICommand SearchCommand { get; set; }
        public ICommand BackToMainCommand { get; set; }
        public ICommand GhostSightingCommand { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public MedicalStaffViewModel()
        {
            // 1. Initialize the Database connection and Services
            var dbContext = new HospitalDbContext();
            var patientRepo = new PatientRepository(dbContext);
            var historyRepo = new MedicalHistoryRepository(dbContext);
            var recordRepo = new MedicalRecordRepository(dbContext);

            _patientService = new PatientService(patientRepo, historyRepo, recordRepo);

            // 2. Setup Commands
            SearchCommand = new RelayCommand(ExecuteSearch);
        }

        private void ExecuteSearch()
        {
            // Reset the UI before searching
            ErrorMessage = string.Empty;
            SearchResults.Clear();

            if (string.IsNullOrWhiteSpace(SearchQuery)) return;

            var filter = new PatientFilter();

            // Logic: If the query is exactly 13 digits, it's a CNP. Otherwise, search by Name.
            if (SearchQuery.Length == 13 && SearchQuery.All(char.IsDigit))
            {
                filter.CNP = SearchQuery;
            }
            else
            {
                filter.namePart = SearchQuery;
            }

            try
            {
                var results = _patientService.SearchPatients(filter);

                if (results == null || results.Count == 0)
                {
                    // Requirement: Exact exception message
                    ErrorMessage = "There are no patients with this name or CNP.";
                }
                else
                {
                    // Add matches to the table
                    foreach (var p in results)
                    {
                        SearchResults.Add(p);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "Database connection error: " + ex.Message;
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
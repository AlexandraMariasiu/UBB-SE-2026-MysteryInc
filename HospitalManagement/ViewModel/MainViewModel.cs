using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace HospitalManagement.ViewModel
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private object _currentView;
        private int _ghostSightingCount = 0;
        private DateTime _lastResetTime = DateTime.Now;

        public event PropertyChangedEventHandler PropertyChanged;

        // View-ul activ (Admin, Staff sau Pharmacist)
        public object CurrentView
        {
            get => _currentView;
            set
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }

        // Comenzi pentru interfață
        public ICommand RedirectToAdminRoleCommand { get; }
        public ICommand RedirectToStaffRoleCommand { get; }
        public ICommand RedirectToPharmacistRoleCommand { get; }
        public ICommand GhostSightingCommand { get; }

        public MainWindowViewModel()
        {
            RedirectToAdminRoleCommand = new RelayCommand(RedirectToAdminRole);
            RedirectToStaffRoleCommand = new RelayCommand(RedirectToStaffRole);
            RedirectToPharmacistRoleCommand = new RelayCommand(RedirectToPharmacistRole);
            GhostSightingCommand = new RelayCommand(RecordGhostSighting);
        }

        private void RedirectToAdminRole() => CurrentView = "AdminDashboard";
        private void RedirectToStaffRole() => CurrentView = "StaffDashboard";
        private void RedirectToPharmacistRole() => CurrentView = "PharmacistDashboard";

        // SV23: Implementare monitorizare fantome
        private void RecordGhostSighting()
        {
            if ((DateTime.Now - _lastResetTime).TotalHours > 24)
            {
                _ghostSightingCount = 0;
                _lastResetTime = DateTime.Now;
            }

            _ghostSightingCount++;

            if (_ghostSightingCount > 3)
            {
                TriggerExorcismAlert();
            }
        }

        private void TriggerExorcismAlert()
        {
            // Aici va veni logica de afișare a alertei
            string message = "CRITICAL PARANORMAL ACTIVITY DETECTED: Multiple sightings confirmed. Please CALL THE PRIEST immediately.";
            Console.WriteLine(message); // Exemplu temporar
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    // Această clasă transformă o simplă metodă într-o "Comandă" pe care o pot folosi butoanele din XAML
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;

        public RelayCommand(Action execute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter)
        {
            _execute();
        }
    }
}
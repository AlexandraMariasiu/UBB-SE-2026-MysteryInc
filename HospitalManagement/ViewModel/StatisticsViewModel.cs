using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HospitalManagement.Service;
using Windows.Services.Maps;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using System.Runtime.CompilerServices;

namespace HospitalManagement.ViewModel
{
    public class StatisticsViewModel : INotifyPropertyChanged
    {
        private readonly StatisticsService _statisticsService;
        public event PropertyChangedEventHandler? PropertyChanged;

        public ObservableCollection<ISeries> GenderSeries { get; set; } = new();
        public ObservableCollection<ISeries> AgeSeries { get; set; } = new();
        public Axis[] AgeXAxes { get; set; }

        public ObservableCollection<string> MenuOptions { get; } = new() { 
            "Patient Distributiton",
            "Consultation Source",
            "Top Diagnoses",
            "Top Medications",
            "Demographics"
        };

        private string _selectedStatistic;

        public string SelectedStatistic
        {
            get => _selectedStatistic;
            set
            {
                _selectedStatistic = value;
                OnPropertyChanged();
                LoadDataForSelection(value);
            }
        }

        private ISeries[] _currentSeries;
        public ISeries[] CurrentSeries
        {
            get => _currentSeries;
            set { _currentSeries = value; OnPropertyChanged(); }
        }

        private Axis[] _xAxes;
        public Axis[] XAxes
        {
            get => _xAxes;
            set { _xAxes = value; OnPropertyChanged(); }
        }

        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set { _errorMessage = value; OnPropertyChanged(); }
        }

        public ObservableCollection<ISeries> PatientDistributionSeries { get; set; } =new ObservableCollection<ISeries>();
        public ObservableCollection<ISeries> ConsultationSourceSeries { get; set; } = new ObservableCollection<ISeries>();
       
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public StatisticsViewModel(StatisticsService statisticsService)
        {
            this._statisticsService = statisticsService;
            SelectedStatistic = MenuOptions[0];
        }

        private void LoadDataForSelection(string selection)
        {
            ErrorMessage = string.Empty;
            try
            {
                switch (selection)
                {
                    case "Patient Distribution":
                        LoadPatientDistribution();
                        break;
                    case "Consultation Source": 
                        LoadConsultationSources(); 
                        break;
                    case "Top Diagnoses": 
                        LoadTopDiagnoses(); 
                        break;
                    case "Top Medications":
                        LoadTopMedications();
                        break;
                    case "Demographics":
                        LoadDemographics();
                        break;
                }
            }
            catch (Exception)
            {
                ErrorMessage = "Error loading statistics from service";
            }
        }

        public void LoadPatientDistribution()
        {
            var data = _statisticsService.GetActiveVsArchivedRatio();
            CurrentSeries = data.Select(kvp => new PieSeries<int>
            {
                Name = kvp.Key,
                Values = new[] { kvp.Value }
            }).Cast<ISeries>().ToArray();

        }
        private void LoadConsultationSources()
        {
            var data = _statisticsService.GetConsultationDistribution();
            CurrentSeries = data.Select(kvp => new PieSeries<int>
            {
                Name = kvp.Key == "ER" ? "Emergency Department" : "Scheduled Appointments",
                Values = new[] { kvp.Value }
            }).Cast<ISeries>().ToArray();
        }

        private void LoadTopDiagnoses()
        {
            var data = _statisticsService.GetTopDiagnoses(); // Assuming returns Map<string, int>
            XAxes = new[] { new Axis { Labels = data.Keys.ToArray(), LabelsRotation = 15 } };
            CurrentSeries = new ISeries[] { new ColumnSeries<int> { Values = data.Values.ToArray(), Name = "Diagnoses" } };
        }

        private void LoadTopMedications()
        {
            var data = _statisticsService.GetMostPrescribedMeds();
            XAxes = new[] { new Axis { Labels = data.Keys.ToArray() } };
            CurrentSeries = new ISeries[] { new ColumnSeries<int> { Values = data.Values.ToArray(), Name = "Prescriptions" } };
        }
        private void LoadDemographics()
        {
            // 1. Fetch Gender Data (Pie Chart)
            var genderData = _statisticsService.GetPatientGenderDistribution(); // Map<SexEnum, int>
            GenderSeries.Clear();
            foreach (var entry in genderData)
            {
                GenderSeries.Add(new PieSeries<int>
                {
                    Name = entry.Key.ToString(), // Maps "Male", "Female", etc.
                    Values = new[] { entry.Value }
                });
            }

            // 2. Fetch Age Data (Bar Chart)
            var ageData = _statisticsService.GetAgeDistribution(); // Map<string, int>
            AgeSeries.Clear();
            AgeSeries.Add(new ColumnSeries<int>
            {
                Name = "Patients",
                Values = ageData.Values.ToArray()
            });

            AgeXAxes = new[] {
                new Axis { Labels = ageData.Keys.ToArray() } // 0-17, 18-64, 65+
    };

            // Trigger UI update for the Axes
            OnPropertyChanged(nameof(AgeXAxes));
        }


    }
}

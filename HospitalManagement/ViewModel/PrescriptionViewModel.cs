using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq; 
using HospitalManagement.Entity;
using HospitalManagement.Integration;
using HospitalManagement.Service;

namespace HospitalManagement.ViewModel
{
    public class PrescriptionViewModel
    {
      
        private readonly PrescriptionService _prescriptionService;
        private readonly AddictDetectionService _addictDetectionService;

        
        public ObservableCollection<Prescription> Prescriptions { get; set; }

        public ObservableCollection<Patient> AddictCandidates { get; set; }

        public PrescriptionFilter ActiveFilter { get; set; }
        public int CurrentPage { get; set; }
        public string InfoMessage { get; set; } = string.Empty;

        private const int PageSize = 9;

        public PrescriptionViewModel(
            PrescriptionService prescriptionService, 
            AddictDetectionService addictDetectionService)
        {
            _prescriptionService = prescriptionService ?? throw new ArgumentNullException(nameof(prescriptionService));
            _addictDetectionService = addictDetectionService ?? throw new ArgumentNullException(nameof(addictDetectionService));

            Prescriptions = new ObservableCollection<Prescription>();
            AddictCandidates = new ObservableCollection<Patient>();
            
            ActiveFilter = new PrescriptionFilter();
            CurrentPage = 1;
        }

        public void LoadPrescriptions()
        {
            Prescriptions.Clear();
            InfoMessage = string.Empty; 

            var latestOrders = _prescriptionService.GetLatestPrescriptions(PageSize, CurrentPage);

            foreach (var order in latestOrders)
            {
                Prescriptions.Add(order);
            }
        }

        public void ApplyFilterCommand(int? searchId, string? medName, DateTime? dateFrom, DateTime? dateTo, string? patientName, string? doctorName)
        {
            InfoMessage = string.Empty;
            CurrentPage = 1;
            Prescriptions.Clear();

            ActiveFilter.PrescriptionId = searchId;
            ActiveFilter.MedName = medName;
            ActiveFilter.DateFrom = dateFrom;
            ActiveFilter.DateTo = dateTo;
            ActiveFilter.PatientName = patientName;
            ActiveFilter.DoctorName = doctorName;

            try
            {
                var results = _prescriptionService.ApplyFilter(ActiveFilter);

                if (results == null || results.Count == 0)
                {
                    InfoMessage = "No prescriptions found matching those criteria.";
                }
                else
                {
                    foreach (var res in results)
                    {
                        Prescriptions.Add(res);
                    }
                }
            }
            catch (Exception ex)
            {
                InfoMessage = ex.Message; 
            }
        }

        public void NextPage()
        {
            if (Prescriptions.Count == PageSize)
            {
                CurrentPage++;
                LoadPrescriptions();
            }
        }

        public void PrevPage()
        {
            if (CurrentPage > 1)
            {
                CurrentPage--;
                LoadPrescriptions();
            }
        }
        
     
        public Prescription ShowMedsList(int id)
        {
            return _prescriptionService.GetPrescriptionDetails(id);
        }
  
        public void SeeAllAddicts()
        {
            AddictCandidates.Clear();
            
            var candidates = _addictDetectionService.GetAddictCandidates();
            
            foreach(var candidate in candidates)
            {
                AddictCandidates.Add(candidate);
            }
        }

  
        public string NotifyPolice(int patientId)
        {
            var flaggedPatient = AddictCandidates.FirstOrDefault(p => p.Id == patientId);

            if (flaggedPatient == null)
            {
                return "Error: Patient data not completely synced or patient ID is invalid.";
            }

            return _addictDetectionService.BuildPoliceReport(flaggedPatient);
        }
    }
}

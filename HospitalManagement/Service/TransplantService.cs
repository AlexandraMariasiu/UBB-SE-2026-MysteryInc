using System;
using System.Collections.Generic;
using System.Linq;
using HospitalManagement.Entity;
using HospitalManagement.Entity.Enums;
using HospitalManagement.Repository;

namespace HospitalManagement.Service
{
    public class TransplantService
    {
        private readonly TransplantRepository _transplantRepo;
        private readonly PatientRepository _patientRepo;
        private readonly BloodCompatibilityService _compatibilityService;

        public TransplantService(
            TransplantRepository transplantRepo,
            PatientRepository patientRepo,
            BloodCompatibilityService compatibilityService)
        {
            _transplantRepo = transplantRepo;
            _patientRepo = patientRepo;
            _compatibilityService = compatibilityService;
        }


        // VM40: CREATE REQUEST
        public void CreateTransplantRequest(int receiverId, string organType)
        {
            var receiver = _patientRepo.GetById(receiverId);

            if (receiver == null)
                throw new ArgumentException("Receiver patient not found.");

            //Prevent duplicate pending requests
            var existing = _transplantRepo
                .GetByReceiverId(receiverId)
                .Any(t => t.OrganType == organType &&
                          t.Status == TransplantStatus.Pending);

            if (existing)
                throw new InvalidOperationException("Patient already has a pending request for this organ.");

            var request = new Transplant
            {
                ReceiverId = receiverId,
                DonorId = null,
                OrganType = organType,
                RequestDate = DateTime.Now,
                Status = TransplantStatus.Pending,
                CompatibilityScore = 0
            };

            _transplantRepo.Add(request);
        }


        // VM38: FIND MATCHES
        public List<(Transplant Request, float Score)> GetPotentialMatchesForDonor(int donorId, string organType)
        {
            var donor = _patientRepo.GetById(donorId);

            if (donor == null || !donor.IsDeceased || !donor.IsDonor)
                throw new InvalidOperationException("Matching is only allowed for deceased, registered donors.");

            var waitlist = _transplantRepo.GetWaitingByOrgan(organType);

            var results = new List<(Transplant, float)>();

            foreach (var request in waitlist)
            {
                var receiver = _patientRepo.GetById(request.ReceiverId);
                if (receiver == null) continue;

               //reuse scoring sistem
                float score = _compatibilityService.CalculateScore(donor, receiver);

                results.Add((request, score));
            }

            return results
                .OrderByDescending(r => r.Item2) // Score
                .ThenBy(r => r.Item1.RequestDate) // fairness (older first)
                .Take(5)
                .ToList();
        }


        // VM38: CONFIRM MATCH
        public void ConfirmMatch(int transplantId, int donorId, float finalScore)
        {
            var transplant = _transplantRepo.GetById(transplantId);
            if (transplant == null)
                throw new ArgumentException("Transplant request not found.");

            if (transplant.Status != TransplantStatus.Pending)
                throw new InvalidOperationException("Only pending requests can be confirmed.");

            var donor = _patientRepo.GetById(donorId);
            if (donor == null || !donor.IsDeceased || !donor.IsDonor)
                throw new InvalidOperationException("Invalid donor.");

            //update state
            transplant.DonorId = donorId;
            transplant.Status = TransplantStatus.Scheduled;
            transplant.CompatibilityScore = finalScore;
            transplant.TransplantDate = DateTime.Now;

            _transplantRepo.Update(transplant.TransplantId, transplant.DonorId.Value, transplant.CompatibilityScore);
        }

        // HISTORY
        public List<Transplant> GetPatientTransplantHistory(int patientId)
        {
            var asReceiver = _transplantRepo.GetByReceiverId(patientId);
            var asDonor = _transplantRepo.GetByDonorId(patientId);

            return asReceiver
                .Concat(asDonor)
                .OrderByDescending(t => t.RequestDate)
                .ToList();
        }
    }
}
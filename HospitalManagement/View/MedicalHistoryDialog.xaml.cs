using Microsoft.UI.Xaml.Controls;
using HospitalManagement.Entity;
using HospitalManagement.Entity.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HospitalManagement.View
{
    public sealed partial class MedicalHistoryDialog : ContentDialog
    {
        public MedicalHistory MedicalHistory { get; private set; }
        public bool WasSkipped { get; private set; }

        public MedicalHistoryDialog()
        {
            this.InitializeComponent();
            this.Closing += MedicalHistoryDialog_Closing;
        }

        private void MedicalHistoryDialog_Closing(ContentDialog sender, ContentDialogClosingEventArgs args)
        {
            // If user clicked "Skip" or closed dialog (not Primary button), mark as skipped
            if (args.Result != ContentDialogResult.Primary)
            {
                WasSkipped = true;
            }
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            try
            {
                // Parse Blood Type
                string bloodTypeStr = (BloodTypeEntry.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "A";
                BloodType bloodType = Enum.Parse<BloodType>(bloodTypeStr);

                // Parse RH Factor
                string rhStr = (RhFactorEntry.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Positive";
                RhEnum rhFactor = rhStr == "Positive" ? RhEnum.Positive : RhEnum.Negative;

                // Parse Chronic Conditions (comma-separated)
                List<string> chronicConditions = new();
                if (!string.IsNullOrWhiteSpace(ChronicConditionsEntry.Text))
                {
                    chronicConditions = ChronicConditionsEntry.Text
                        .Split(',')
                        .Select(c => c.Trim())
                        .Where(c => !string.IsNullOrWhiteSpace(c))
                        .ToList();
                }

                // Create MedicalHistory object
                MedicalHistory = new MedicalHistory
                {
                    BloodType = bloodType,
                    Rh = rhFactor,
                    ChronicConditions = chronicConditions
                };

                WasSkipped = false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error parsing medical history: {ex.Message}");
                args.Cancel = true;
            }
        }
    }
}

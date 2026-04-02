using Microsoft.UI.Xaml.Controls;
using HospitalManagement.ViewModel;
using HospitalManagement.Entity;
using System.Threading.Tasks;
using System;

namespace HospitalManagement.View
{
    public sealed partial class OrganDonorDialog : ContentDialog
    {
        public OrganDonorViewModel ViewModel { get; private set; }

        public OrganDonorDialog()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Initialize the dialog with a deceased donor and handle confirmation
        /// </summary>
        public void Initialize(OrganDonorViewModel viewModel, System.Action<int, int, float> onAssigned)
        {
            ViewModel = viewModel;
            this.DataContext = viewModel;

            // Hook up confirmation callback
            ViewModel.OnAssignmentConfirmed = onAssigned;

            // Wire up primary button
            this.PrimaryButtonClick += async (s, e) =>
            {
                try
                {
                    ViewModel.ConfirmAssignment();
                }
                catch (System.Exception ex)
                {
                    ContentDialog errorDialog = new ContentDialog
                    {
                        Title = "Confirmation Error",
                        Content = ex.Message,
                        CloseButtonText = "OK",
                        XamlRoot = this.XamlRoot
                    };
                    await errorDialog.ShowAsync().AsTask();
                    e.Cancel = true; // Keep dialog open
                }
            };
        }
    }
}

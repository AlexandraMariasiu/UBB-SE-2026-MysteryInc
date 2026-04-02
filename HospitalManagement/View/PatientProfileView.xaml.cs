using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls; // Add this for Page
using HospitalManagement.ViewModel;

namespace HospitalManagement.View
{
    public sealed partial class PatientProfileView : Page
    {
        public PatientProfileViewModel ViewModel { get; }

        public PatientProfileView(int patientId)
        {
            // 1. CREATE THE VIEWMODEL FIRST!
            ViewModel = new PatientProfileViewModel(patientId);

            // 2. INITIALIZE THE UI SECOND!
            this.InitializeComponent();

            // 3. Set the DataContext
            if (this.Content is FrameworkElement rootElement)
            {
                rootElement.DataContext = ViewModel;
            }
        }
    }
}
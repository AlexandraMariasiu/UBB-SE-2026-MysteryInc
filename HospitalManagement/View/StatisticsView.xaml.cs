using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using HospitalManagement.ViewModel;
using HospitalManagement.Service;
using HospitalManagement.Repository;
using HospitalManagement.Database;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace HospitalManagement.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Statistics : Page
    {
        public StatisticsViewModel _statisticsViewModel { get;  }
        public Statistics()
        {
            this.InitializeComponent();

            var dbContext = new HospitalDbContext();
            var pRepo = new PatientRepository(dbContext);
            var prRepo = new PrescriptionRepository(dbContext);
            var rRepo = new MedicalRecordRepository(dbContext);
            var service = new StatisticsService(pRepo, rRepo, prRepo);
            this._statisticsViewModel = new StatisticsViewModel(service);
        }
    }
}

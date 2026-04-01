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
using Microsoft.UI.Windowing;
using HospitalManagement.Database;
using HospitalManagement.Repository;
using HospitalManagement.Service;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace HospitalManagement.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AdminView : Window
    {
        private AdminViewModel _viewModel;
        public AdminView()
        {
            this.InitializeComponent();

            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hwnd);
            var appWindow = AppWindow.GetFromWindowId(windowId);
            if (appWindow.Presenter is OverlappedPresenter presenter)
            {
                presenter.Maximize();
            }

            var dbContext = new HospitalDbContext();
            var pRepo = new PatientRepository(dbContext);
            var hRepo = new MedicalHistoryRepository(dbContext);
            var rRepo = new MedicalRecordRepository(dbContext);

            var service = new PatientService(pRepo, hRepo, rRepo);

            this._viewModel = new AdminViewModel(service);
            _viewModel.PropertyChanged += ViewModel_PropertyChanged;

            UpdateView(_viewModel.CurrentView);
        }

        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(AdminViewModel.CurrentView))
            {
                UpdateView(_viewModel.CurrentView);
            }
        }

        private void UpdateView(string viewName)
        {
            switch (viewName)
            {
                case "AdminDashboard":
                    //var adminView = new AdminView();

                    // Replace 'Show()' with 'Activate()' as 'Show()' is not defined for AdminView.
                    //MainContentArea.Content = adminView;
                    break;

                case "Statistics":
                    Console.WriteLine("hello");
                    var statisticsView = new Statistics();

                    // Replace 'Show()' with 'Activate()' as 'Show()' is not defined for Statistics.
                    MainContentArea.Navigate(typeof( Statistics));
                    break;
            }
        }

        public Page GetCurrentPage()
        {
            return _viewModel.CurrentView switch
            {
                "Statistics" => new Statistics(),
                
            };
        }

        private void OpenPage_Click(object sender, RoutedEventArgs e)
        {
            // Check if the page is already shown to avoid redundant loading
            if (MainContentArea.CurrentSourcePageType != typeof(Statistics))
            {
                MainContentArea.Navigate(typeof(Statistics));
            }
        }


    }
}

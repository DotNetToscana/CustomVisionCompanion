using CustomVisionCompanion.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace CustomVisionCompanion
{
    public partial class App : Application
    {
        public static bool IsPausing { get; set; }

        public App()
        {
            InitializeComponent();

            var start = new MainPage();
            MainPage = new NavigationPage(start);
        }

        protected override void OnStart()
        {
            // Handle when your app starts
            //await MainPage.Navigation.RestoreAsync(TimeSpan.FromHours(1));
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
            //MainPage.Navigation.Store();
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}

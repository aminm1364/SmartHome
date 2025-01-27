using SmartHome.Pages;

namespace SmartHome
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Register routes for additional pages if needed
            Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
            Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
            Routing.RegisterRoute(nameof(ConfigurationPage), typeof(ConfigurationPage));
            Routing.RegisterRoute(nameof(LogsPage), typeof(LogsPage));
            //Routing.RegisterRoute(nameof(ProfilePage), typeof(ProfilePage));
        }

        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            SecureStorage.Default.Remove("AuthToken");
            Application.Current.MainPage = new NavigationPage(new LoginPage());
        }
    }
}

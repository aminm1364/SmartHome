namespace SmartHome.Pages;

public partial class LoginPage : ContentPage
{
	public LoginPage()
	{
		InitializeComponent();
	}
    private async void OnLoginClicked(object sender, EventArgs e)
    {
        bool isValidUser = ValidateCredentials(PasswordEntry.Text);

        if (isValidUser)
        {
            // Save token to SecureStorage
            await SecureStorage.Default.SetAsync("AuthToken", "YourToken");

            // Navigate to the main page
            Application.Current.MainPage = new AppShell();
        }
        else
        {
            await DisplayAlert("Error", "Invalid login credentials.", "OK");
        }
    }

    private bool ValidateCredentials(string password)
    {
        // Replace with real authentication logic
        string code = DateTime.Now.ToString("ddHHmm");
        return password == code;
    }
}
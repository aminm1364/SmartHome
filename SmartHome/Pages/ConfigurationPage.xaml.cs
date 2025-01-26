using SmartHome.Interfaces;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
//using CommunityToolkit.Maui.Views;

namespace SmartHome.Pages;

public partial class ConfigurationPage : ContentPage
{
    public IServerConfiguration _Configuration { get; set; }
    bool changeDetected = false;
    bool changeDetectedForPasswordField = false;
    public ConfigurationPage(IServerConfiguration serverConfiguration)
    {
        _Configuration = serverConfiguration;
        InitializeComponent();
        ComponentConfigurations();
    }

    private void ComponentConfigurations()
    {
        section1_label.Text = "Server Configuration";
        section2_label.Text = "Relay options";

        if (string.IsNullOrEmpty(_Configuration.Host))
        {
            expander1.IsExpanded = true;
        }
    }

    private void FillData()
    {
        bool isConfigurationExist = !String.IsNullOrEmpty(_Configuration.Host) && !String.IsNullOrEmpty(_Configuration.Username) && !String.IsNullOrEmpty(_Configuration.Password);
        if (!isConfigurationExist)
        {
            saveButton.IsEnabled = true;
            ProtocolPicker.SelectedItem = string.Empty;
            HostEntry.Text = string.Empty;
            PortEntry.Text = string.Empty;
            VirtualHostEntry.Text = string.Empty;
            UsernameEntry.Text = string.Empty;
            PasswordEntry.Placeholder = string.Empty;
            SslSwitch.IsToggled = false;
            SslHostNameEntry.Text = string.Empty;
            return;
        }

        // Retrieve user input
        ProtocolPicker.SelectedItem = _Configuration.Protocol;
        HostEntry.Text = _Configuration.Host;
        PortEntry.Text = _Configuration.HostPort.ToString();
        VirtualHostEntry.Text = _Configuration.VirtualHost;
        UsernameEntry.Text = _Configuration.Username;
        PasswordEntry.Placeholder = "*********";
        SslSwitch.IsToggled = _Configuration.IsSSL;
        SslHostNameEntry.Text = _Configuration.SslHostName;
        Relay1Checkbox.IsChecked = _Configuration?.Relay1Checked ?? false;
        Relay2Checkbox.IsChecked = _Configuration?.Relay2Checked ?? false;
        Relay3Checkbox.IsChecked = _Configuration?.Relay3Checked ?? false;
        Relay4Checkbox.IsChecked = _Configuration?.Relay4Checked ?? false;


        saveButton.IsEnabled = false;
    }

    private void ChangeDetected(object sender, EventArgs e)
    {
        if (sender is Entry entry)
        {
            string fieldName = entry.StyleId;
            if (fieldName == nameof(PasswordEntry))
            {
                ChangePasswordDetected();
            }
        }
        changeDetected = true;
        saveButton.IsEnabled = true;
    }

    private void ChangePasswordDetected()
    {
        changeDetectedForPasswordField = true;
    }

    // Handle SSL Toggle
    private void OnSslToggled(object sender, ToggledEventArgs e)
    {
        ChangeDetected(sender, e);
        // Show or hide the SSL Host Name input based on toggle state
        SslHostNameContainer.IsVisible = e.Value;
        ProtocolPicker.SelectedIndex = e.Value ? 1 : 0;
    }

    // Cancel button click handler
    private async void OnCancelClicked(object sender, EventArgs e)
    {
        // Navigate back to the MainPage
        await Shell.Current.GoToAsync("//MainPage");
    }

    // Save button click handler
    private async void OnSaveClicked(object sender, EventArgs e)
    {
        // Retrieve user input
        string protocol = ProtocolPicker.SelectedItem?.ToString();
        string host = HostEntry.Text;
        bool isPortEntryValid = Int32.TryParse(PortEntry.Text, out int port);
        string virtualHost = VirtualHostEntry.Text;
        string username = UsernameEntry.Text;
        string password = PasswordEntry.Text;
        bool isSSL = SslSwitch.IsToggled;
        string sslHostName = SslHostNameEntry.Text;

        // Retrieve user input
        bool relay1_isChecked = Relay1Checkbox.IsChecked;
        bool relay2_isChecked = Relay2Checkbox.IsChecked;
        bool relay3_isChecked = Relay3Checkbox.IsChecked;
        bool relay4_isChecked = Relay4Checkbox.IsChecked;

        string relay1LabelHighEntry = Relay1LabelHighEntry.Text;
        string relay1LabelLowEntry = Relay1LabelLowEntry.Text;
        string relay2LabelHighEntry = Relay2LabelHighEntry.Text;
        string relay2LabelLowEntry = Relay2LabelLowEntry.Text;
        string relay3LabelHighEntry = Relay3LabelHighEntry.Text;
        string relay3LabelLowEntry = Relay3LabelLowEntry.Text;
        string relay4LabelHighEntry = Relay4LabelHighEntry.Text;
        string relay4LabelLowEntry = Relay4LabelLowEntry.Text;

        // Validate inputs
        if (
            string.IsNullOrWhiteSpace(protocol) ||
            string.IsNullOrWhiteSpace(host) ||
            !isPortEntryValid ||
            string.IsNullOrWhiteSpace(virtualHost) ||
            string.IsNullOrWhiteSpace(username) ||
            (changeDetectedForPasswordField && string.IsNullOrWhiteSpace(password)) ||
            (isSSL && string.IsNullOrWhiteSpace(sslHostName))
            )
        {
            await DisplayAlert("Error", "Please fill in all required fields.", "OK");
            return;
        }

        // Save data securely
        await SecureStorage.Default.SetAsync("Protocol", protocol);
        await SecureStorage.Default.SetAsync("Host", host);
        await SecureStorage.Default.SetAsync("Port", port.ToString());
        await SecureStorage.Default.SetAsync("VirtualHost", virtualHost);
        await SecureStorage.Default.SetAsync("Username", username);
        if (changeDetectedForPasswordField)
            await SecureStorage.Default.SetAsync("Password", password);

        await SecureStorage.Default.SetAsync("IsSSL", isSSL.ToString());

        if (isSSL) await SecureStorage.Default.SetAsync("SslHostName", sslHostName);
        else SecureStorage.Default.Remove("SslHostName");



        // Saving Relay configurations
        Preferences.Set("Relay1_IsChecked", relay1_isChecked);
        Preferences.Set("Relay2_IsChecked", relay2_isChecked);
        Preferences.Set("Relay3_IsChecked", relay3_isChecked);
        Preferences.Set("Relay4_IsChecked", relay4_isChecked);

        if (!string.IsNullOrEmpty(relay1LabelHighEntry))
            Preferences.Set(nameof(_Configuration.Relay1HighLabel), relay1LabelHighEntry);
        if (!string.IsNullOrEmpty(relay1LabelLowEntry))
            Preferences.Set(nameof(_Configuration.Relay1LowLabel), relay1LabelLowEntry);

        if (!string.IsNullOrEmpty(relay2LabelHighEntry))
            Preferences.Set(nameof(_Configuration.Relay2HighLabel), relay2LabelHighEntry);
        if (!string.IsNullOrEmpty(relay2LabelLowEntry))
            Preferences.Set(nameof(_Configuration.Relay2LowLabel), relay2LabelLowEntry);

        if (!string.IsNullOrEmpty(relay3LabelHighEntry))
            Preferences.Set(nameof(_Configuration.Relay3HighLabel), relay3LabelHighEntry);
        if (!string.IsNullOrEmpty(relay3LabelLowEntry))
            Preferences.Set(nameof(_Configuration.Relay3LowLabel), relay3LabelLowEntry);

        if (!string.IsNullOrEmpty(relay4LabelHighEntry))
            Preferences.Set(nameof(_Configuration.Relay4HighLabel), relay4LabelHighEntry);
        if (!string.IsNullOrEmpty(relay4LabelLowEntry))
            Preferences.Set(nameof(_Configuration.Relay4LowLabel), relay4LabelLowEntry);

        // ---------------------------


        await DisplayAlert("Success", "Configuration saved!", "OK");

        // Navigate back to the MainPage
        await Shell.Current.GoToAsync("//MainPage");
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        FillData();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _Configuration.GetConfigurations(null);
        expander1.IsExpanded = false;
        expander2.IsExpanded = false;
    }

    protected override bool OnBackButtonPressed()
    {
        _Configuration.GetConfigurations(null);
        return base.OnBackButtonPressed();
    }

    private void Expander1_ExpandedChanged(object sender, CommunityToolkit.Maui.Core.ExpandedChangedEventArgs e)
    {
        border_expander1.StrokeShape = expander1.IsExpanded ? new RoundRectangle { CornerRadius = new CornerRadius(10, 10, 0, 0) } : new RoundRectangle { CornerRadius = new CornerRadius(10) };
    }

    private void Expander2_ExpandedChanged(object sender, CommunityToolkit.Maui.Core.ExpandedChangedEventArgs e)
    {
        border_expander2.StrokeShape = expander2.IsExpanded ? new RoundRectangle { CornerRadius = new CornerRadius(10, 10, 0, 0) } : new RoundRectangle { CornerRadius = new CornerRadius(10) };
    }

    private void ProtocolPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        ChangeDetected(sender, e);
        SslSwitch.IsToggled = ProtocolPicker.SelectedIndex == 1;
    }
    private void onCheckBoxRelay1CheckedChanged(object sender, CheckedChangedEventArgs e) => ChangeDetected(sender, e);
    private void onCheckBoxRelay2CheckedChanged(object sender, CheckedChangedEventArgs e) => ChangeDetected(sender, e);
    private void onCheckBoxRelay3CheckedChanged(object sender, CheckedChangedEventArgs e) => ChangeDetected(sender, e);
    private void onCheckBoxRelay4CheckedChanged(object sender, CheckedChangedEventArgs e) => ChangeDetected(sender, e);
    private void Relay1LabelHighEntry_TextChanged(object sender, TextChangedEventArgs e) => ChangeDetected(sender, e);

    private void Relay1LabelLowEntry_TextChanged(object sender, TextChangedEventArgs e) => ChangeDetected(sender, e);

    private void Relay2LabelHighEntry_TextChanged(object sender, TextChangedEventArgs e) => ChangeDetected(sender, e);

    private void Relay2LabelLowEntry_TextChanged(object sender, TextChangedEventArgs e) => ChangeDetected(sender, e);

    private void Relay3LabelHighEntry_TextChanged(object sender, TextChangedEventArgs e) => ChangeDetected(sender, e);

    private void Relay3LabelLowEntry_TextChanged(object sender, TextChangedEventArgs e) => ChangeDetected(sender, e);

    private void Relay4LabelHighEntry_TextChanged(object sender, TextChangedEventArgs e) => ChangeDetected(sender, e);

    private void Relay4LabelLowEntry_TextChanged(object sender, TextChangedEventArgs e) => ChangeDetected(sender, e);
}
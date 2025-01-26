
using SmartHome.Interfaces;

namespace SmartHome.Models
{
    public class ServerConfiguration : IServerConfiguration
    {
        private readonly string DEFAULT_HIGH_LABEL = "HIGH";
        private readonly string DEFAULT_LOW_LABEL = "LOW";
        public string Protocol { get; private set; }
        public string Host { get; private set; }
        public int HostPort { get; private set; }
        public string VirtualHost { get; private set; }
        public string Username { get; private set; }
        public string Password { get; private set; }
        public bool IsSSL { get; private set; }
        public string SslHostName { get; private set; }
        public bool Relay1Checked { get; private set; }
        public bool Relay2Checked { get; private set; }
        public bool Relay3Checked { get; private set; }
        public bool Relay4Checked { get; private set; }

        public string Relay1HighLabel { get; private set; }
        public string Relay1LowLabel { get; private set; }
        public string Relay2HighLabel { get; private set; }
        public string Relay2LowLabel { get; private set; }
        public string Relay3HighLabel { get; private set; }
        public string Relay3LowLabel { get; private set; }
        public string Relay4HighLabel { get; private set; }
        public string Relay4LowLabel { get; private set; }


        public async void GetConfigurations(Action callback)
        {
            this.Protocol = await SecureStorage.GetAsync("Protocol") ?? string.Empty;
            this.Host = await SecureStorage.GetAsync("Host") ?? string.Empty;
            this.HostPort = int.TryParse(await SecureStorage.GetAsync("Port"), out int _hostPort) ? _hostPort : 0;
            this.VirtualHost = await SecureStorage.GetAsync("VirtualHost") ?? string.Empty;
            this.Username = await SecureStorage.GetAsync("Username") ?? string.Empty;
            this.Password = await SecureStorage.GetAsync("Password") ?? string.Empty;
            this.IsSSL = bool.TryParse(await SecureStorage.GetAsync("IsSSL"), out bool value) ? value : false;
            this.SslHostName = await SecureStorage.GetAsync("SslHostName") ?? string.Empty;
            this.Relay1Checked = Preferences.Get("Relay1_IsChecked", false);
            this.Relay2Checked = Preferences.Get("Relay2_IsChecked", false);
            this.Relay3Checked = Preferences.Get("Relay3_IsChecked", false);
            this.Relay4Checked = Preferences.Get("Relay4_IsChecked", false);

            this.Relay1HighLabel = Preferences.Get(nameof(this.Relay1HighLabel), DEFAULT_HIGH_LABEL);
            this.Relay1LowLabel = Preferences.Get(nameof(this.Relay1LowLabel), DEFAULT_LOW_LABEL);
            this.Relay2HighLabel = Preferences.Get(nameof(this.Relay2HighLabel), DEFAULT_HIGH_LABEL);
            this.Relay2LowLabel = Preferences.Get(nameof(this.Relay2LowLabel), DEFAULT_LOW_LABEL);
            this.Relay3HighLabel = Preferences.Get(nameof(this.Relay3HighLabel), DEFAULT_HIGH_LABEL);
            this.Relay3LowLabel = Preferences.Get(nameof(this.Relay3LowLabel), DEFAULT_LOW_LABEL);
            this.Relay4HighLabel = Preferences.Get(nameof(this.Relay4HighLabel), DEFAULT_HIGH_LABEL);
            this.Relay4LowLabel = Preferences.Get(nameof(this.Relay4LowLabel), DEFAULT_LOW_LABEL);

            callback?.Invoke();
        }

    }

}

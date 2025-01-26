namespace SmartHome.Interfaces
{
    public interface IServerConfiguration
    {
        string Host { get; }
        int HostPort { get; }
        bool IsSSL { get; }
        string Password { get; }
        string Protocol { get; }
        string SslHostName { get; }
        string Username { get; }
        string VirtualHost { get; }
        public bool Relay1Checked { get; }
        public bool Relay2Checked { get; }
        public bool Relay3Checked { get; }
        public bool Relay4Checked { get; }

        public string Relay1HighLabel { get; }
        public string Relay1LowLabel { get; }
        public string Relay2HighLabel { get; }
        public string Relay2LowLabel { get; }
        public string Relay3HighLabel { get; }
        public string Relay3LowLabel { get; }
        public string Relay4HighLabel { get; }
        public string Relay4LowLabel { get; }

        void GetConfigurations(Action callback);
    }
}
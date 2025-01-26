using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SmartHome.Enums;
using SmartHome.Interfaces;
using SmartHome.Models;
using SmartHome.Pages;
using System.Runtime.CompilerServices;
using System.Text;

namespace SmartHome
{
    public partial class MainPage : ContentPage
    {
        private IMQTTConnection _mQTT { get; set; }
        private AsyncEventingBasicConsumer Consumer { get; set; }
        
        private IServerConfiguration _Configuration;
        private static string DOOR_OPENER_COMMAND(int relayNumber) => $"Relay_{relayNumber}_LHT_1";
        private static string DOOR_BUTTON_DEFAULT_TEXT(int relayNumber) => $"Open the door {relayNumber}";
        private static string DOOR_BUTTON_IS_OPEN_TEXT(int relayNumber) => $"Door {relayNumber} is open...";
        public MainPage(IMQTTConnection mQTTConnection)
        {
            InitializeComponent();
            _mQTT = mQTTConnection;
            _mQTT.RetrieveConfigurations(() =>
            {
                AssignRelayConfigurationToParameters();
                SetTheMainPage();

                _mQTT.InitConnection((consumer) =>
                {
                    Consumer = consumer;
                    RunConsumer();
                    CheckCurrentStatus();
                });
            });
        }

        private void SetTheMainPage()
        {
            Task.Run(async () =>
            {
                var _authToken = await SecureStorage.Default.GetAsync("AuthToken");
                if (_authToken == null)
                {
                    // Redirect to LoginPage if not authenticated
                    Application.Current.MainPage = new NavigationPage(new LoginPage());

                    return;
                }
                if (string.IsNullOrEmpty(_mQTT.Configuration.Host))
                {
                    await Navigation.PushAsync(new ConfigurationPage(_mQTT.Configuration));
                    return;
                }
            });
        }

        void AssignRelayConfigurationToParameters()
        {
            _Configuration = _mQTT.Configuration;
            relay1OnButton.IsVisible = _Configuration.Relay1Checked;
            relay1StatusButton.IsVisible = _Configuration.Relay1Checked;

            relay2OnButton.IsVisible = _Configuration.Relay2Checked;
            relay2StatusButton.IsVisible = _Configuration.Relay2Checked;

            relay3OnButton.IsVisible = _Configuration.Relay3Checked;
            relay3StatusButton.IsVisible = _Configuration.Relay3Checked;

            relay4OnButton.IsVisible = _Configuration.Relay4Checked;
            relay4StatusButton.IsVisible = _Configuration.Relay4Checked;
        }


        private async void CheckCurrentStatus(int? relayNumber = null)
        {
            if (relayNumber.HasValue)
            {
                await _mQTT.SendMessageAsync($"Relay_{relayNumber}_STATUS", isPrivate: true);
                return;
            }

            for (int i = 1; i <= 4; i++)
            {
                await _mQTT.SendMessageAsync($"Relay_{i}_STATUS", isPrivate: true);
            }
        }

        private async void OndeleteTockenButtonClicked(object sender, EventArgs e)
        {
            SecureStorage.Default.Remove("AuthToken");
            SemanticScreenReader.Announce(deleteTocken.Text);
        }

        private async void RunConsumer()
        {
            if (_mQTT == null) throw new Exception("MQTT properties cannot be null here!");
            if (Consumer == null) throw new Exception("Consumer cannot be null here!");
            if (_mQTT.Connection == null) throw new Exception("Connection cannot be null here!");
            if (_mQTT.Channel == null) throw new Exception("Channel cannot be null here!");

            Consumer.ReceivedAsync += async (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var jsonMessage = Encoding.UTF8.GetString(body);
                    var message = Message.Deserialize(jsonMessage);

                    if (message == null) throw new Exception("Error: message couldn't be deserialized!");

                    // Ensure UI updates happen on the main thread
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        //rawMessage.Text = ea.RoutingKey;

                        if (message.IsPrivate)
                        {
                            if (message.UserId == _mQTT.ReceiverQueueName)
                            {
                                if (ea.RoutingKey == _mQTT.RoutingKeyStatus)
                                {
                                    if (message.Text.StartsWith("Relay_") && message.Text.EndsWith("_Status"))
                                    {
                                        int relayNumber = Convert.ToInt16(message.Text.Replace("Relay_", "").Replace("_Status", ""));
                                        if (message.Status == "HIGH" || message.Status == "LOW")
                                        {
                                            switch (relayNumber)
                                            {
                                                case 1:
                                                    relay1OnButton.Text = message.Status == "HIGH" ? _Configuration.Relay1HighLabel : _Configuration.Relay1LowLabel; // HIGH -> OFF. LOW -> ON.
                                                    break;
                                                case 2:
                                                    relay2OnButton.Text = message.Status == "HIGH" ? _Configuration.Relay2HighLabel : _Configuration.Relay2LowLabel;
                                                    break;
                                                case 3:
                                                    relay3OnButton.Text = message.Status == "HIGH" ? _Configuration.Relay3HighLabel : _Configuration.Relay3LowLabel;
                                                    break;
                                                case 4:
                                                    relay4OnButton.Text = message.Status == "HIGH" ? _Configuration.Relay4HighLabel : _Configuration.Relay4LowLabel;
                                                    break;
                                                default:
                                                    break;
                                            }

                                            if (NumberStepper.Value == relayNumber)
                                            {
                                                relayLHT2Button.IsEnabled = message.Status == "HIGH";
                                                relayLHT2Button.Text = DOOR_BUTTON_DEFAULT_TEXT(relayNumber);
                                            }

                                        }

                                    }

                                }
                            }
                        }
                        else
                        {
                            if (message.DirectionType == MessageDirectionType.Callback)
                            {

                                if (message.Token == "MCU2207|" && message.Status == "RING")
                                {
                                    // The button / ring on the MCU has been pressed / triggered.
                                    ringingLabel.Text = $"[{DateTime.Now.ToString("yy.MM.dd-HH:mm:ss")}]-{message.Text}";

                                    // TODO: The rest of the code goes here
                                    return;
                                }
                                if (ea.RoutingKey == _mQTT.RoutingKeyStatus)
                                {
                                    if (message.Text.StartsWith("Relay_") && message.Text.EndsWith("_Status"))
                                    {
                                        int relayNumber = Convert.ToInt16(message.Text.Replace("Relay_", "").Replace("_Status", ""));
                                        if (message.Status == "HIGH" || message.Status == "LOW")
                                        {
                                            switch (relayNumber)
                                            {
                                                case 1:
                                                    relay1OnButton.Text = message.Status == "HIGH" ? _Configuration.Relay1HighLabel : _Configuration.Relay1LowLabel;
                                                    break;
                                                case 2:
                                                    relay2OnButton.Text = message.Status == "HIGH" ? _Configuration.Relay2HighLabel : _Configuration.Relay2LowLabel;
                                                    break;
                                                case 3:
                                                    relay3OnButton.Text = message.Status == "HIGH" ? _Configuration.Relay3HighLabel : _Configuration.Relay3LowLabel;
                                                    break;
                                                case 4:
                                                    relay4OnButton.Text = message.Status == "HIGH" ? _Configuration.Relay4HighLabel : _Configuration.Relay4LowLabel;
                                                    break;
                                                default:
                                                    break;
                                            }

                                            if (NumberStepper.Value == relayNumber)
                                            {
                                                relayLHT2Button.IsEnabled = message.Status == "HIGH";
                                                relayLHT2Button.Text = DOOR_BUTTON_DEFAULT_TEXT(relayNumber);
                                            }

                                        }

                                    }
                                    else if (message.Text.StartsWith("Relay_") && message.Text.EndsWith("_Switched"))
                                    {
                                        var numberString = message.Text.Replace("Relay_", "").Replace("_Switched", "");
                                        Int32.TryParse(numberString, out int relayNumber);
                                        if (message.Status == "LHT")
                                        {
                                            relayLHT2Button.IsEnabled = true;
                                            relayLHT2Button.Text = DOOR_BUTTON_DEFAULT_TEXT(relayNumber);
                                        }
                                    }

                                }
                                else if (ea.RoutingKey == _mQTT.RoutingKeyControl)
                                {
                                    resultLabel.Text = message.Text;
                                }
                            }
                        }

                    });
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            };

            await _mQTT.Channel.BasicConsumeAsync(queue: _mQTT.ReceiverQueueName, autoAck: true, consumer: Consumer);
        }

        private async void OnRelay1OnButtonClicked(object sender, EventArgs e)
        {
            var result = await _mQTT.SendMessageAsync(relay1OnButton.Text == _Configuration.Relay1HighLabel ? "Relay_1_L!" : "Relay_1_H!", isPrivate: false);
            relay1OnButton.Text = relay1OnButton.Text == _Configuration.Relay1HighLabel ? _Configuration.Relay1HighLabel : _Configuration.Relay1LowLabel;

            SemanticScreenReader.Announce(relay1OnButton.Text);
        }

        private async void OnRelay2OnButtonClicked(object sender, EventArgs e)
        {
            var result = await _mQTT.SendMessageAsync(relay2OnButton.Text == _Configuration.Relay2HighLabel ? "Relay_2_L!" : "Relay_2_H!", isPrivate: false);
            relay2OnButton.Text = relay2OnButton.Text == _Configuration.Relay2HighLabel ? _Configuration.Relay2HighLabel : _Configuration.Relay2LowLabel;

            SemanticScreenReader.Announce(relay2OnButton.Text);
        }

        private async void OnRelay3OnButtonClicked(object sender, EventArgs e)
        {
            var result = await _mQTT.SendMessageAsync(relay3OnButton.Text == _Configuration.Relay3HighLabel ? "Relay_3_L!" : "Relay_3_H!", isPrivate: false);
            relay3OnButton.Text = relay3OnButton.Text == _Configuration.Relay3HighLabel ? _Configuration.Relay3HighLabel : _Configuration.Relay3LowLabel;

            SemanticScreenReader.Announce(relay3OnButton.Text);
        }

        private async void OnRelay4OnButtonClicked(object sender, EventArgs e)
        {
            var result = await _mQTT.SendMessageAsync(relay4OnButton.Text == _Configuration.Relay4HighLabel ? "Relay_4_L!" : "Relay_4_H!", isPrivate: false);
            relay4OnButton.Text = relay4OnButton.Text == _Configuration.Relay4HighLabel ? _Configuration.Relay4HighLabel : _Configuration.Relay4LowLabel;

            SemanticScreenReader.Announce(relay4OnButton.Text);
        }


        private async void OnRelay1StatusButtonClicked(object sender, EventArgs e)
        {
            CheckCurrentStatus(1);
            SemanticScreenReader.Announce(relay4OnButton.Text);
        }
        private async void OnRelay2StatusButtonClicked(object sender, EventArgs e)
        {
            CheckCurrentStatus(2);
            SemanticScreenReader.Announce(relay4OnButton.Text);
        }
        private async void OnRelay3StatusButtonClicked(object sender, EventArgs e)
        {
            CheckCurrentStatus(3);
            SemanticScreenReader.Announce(relay4OnButton.Text);
        }
        private async void OnRelay4StatusButtonClicked(object sender, EventArgs e)
        {
            CheckCurrentStatus(4);
            SemanticScreenReader.Announce(relay4OnButton.Text);
        }



        private async void OnRelayLHT2ButtonClicked(object sender, EventArgs e)
        {
            int relayNumber = Convert.ToInt32(NumberStepper.Value);
            if (!relayLHT2Button.IsEnabled && relayLHT2Button.Text != DOOR_BUTTON_IS_OPEN_TEXT(relayNumber))
            {
                CheckCurrentStatus();
                return;
            }
            else if (!relayLHT2Button.IsEnabled && relayLHT2Button.Text == DOOR_BUTTON_IS_OPEN_TEXT(relayNumber))
            {
                return;
            }
            else if (relayLHT2Button.IsEnabled && relayLHT2Button.Text == DOOR_BUTTON_DEFAULT_TEXT(relayNumber))
            {
                var result = await _mQTT.SendMessageAsync(DOOR_OPENER_COMMAND(relayNumber), isPrivate: false);
                relayLHT2Button.IsEnabled = false;
                relayLHT2Button.Text = DOOR_BUTTON_IS_OPEN_TEXT(relayNumber);
                return;
            }

            SemanticScreenReader.Announce(relayLHT2Button.Text);
        }

        private async void OnStepperValueChanged(object sender, ValueChangedEventArgs e)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                ValueLabel.Text = $"Relay number: {e.NewValue}";
            });
            CheckCurrentStatus((int)NumberStepper.Value);
            await Task.CompletedTask;
        }

        protected override void OnAppearing() => _mQTT.Configuration.GetConfigurations(OnGetConfigurationCallback);

        protected override void OnDisappearing() => base.OnDisappearing();

        void OnGetConfigurationCallback()
        {
            AssignRelayConfigurationToParameters();
        }

    }

}

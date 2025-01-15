using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SmartHome.Enums;
using SmartHome.Models;
using System.Text;

namespace SmartHome
{
    public partial class MainPage : ContentPage
    {
        private Services.MQTTConnection Mqtt { get; set; }
        private AsyncEventingBasicConsumer Consumer { get; set; }

        private string ReceiverQueueName = "relay_client_receiver_queue";
        private readonly string RoutingKeyControl = "relay.control";
        private readonly string RoutingKeyStatus = "relay.status";

        public MainPage()
        {
            InitializeComponent();
            ReceiverQueueName = $"{ReceiverQueueName}_{DeviceInfo.Current.Idiom}_{DeviceInfo.Current.Name}";

            RunTasksAsync();
        }
        public async void RunTasksAsync()
        {
            Mqtt = new Services.MQTTConnection(ReceiverQueueName);
            await Mqtt.Run();
            Consumer = await Mqtt.RunReceiver();
            await InitConsumer();
            CheckCurrentStatus();
        }

        private async void CheckCurrentStatus()
        {
            await Mqtt.SendMessageAsync("STATUS");            
        }

        private async Task InitConsumer()
        {
            Consumer.ReceivedAsync += async (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var jsonMessage = Encoding.UTF8.GetString(body);
                    var message = Message.Deserialize(jsonMessage);

                    if (message.UserId == ReceiverQueueName || true) // To make sure, only the sender, gets the respond.
                    {
                        // Ensure UI updates happen on the main thread
                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            //rawMessage.Text = ea.RoutingKey;

                            if (message.DirectionType == MessageDirectionType.Callback)
                            {
                                resultLabel.Text = message.Text;
                                if (ea.RoutingKey == RoutingKeyStatus)
                                {
                                    if (message.Status == "HIGH")
                                        relayOnButton.Text = "LOW";
                                    
                                    if (message.Status == "LOW")
                                        relayOnButton.Text = "HIGH";

                                }
                                else if (ea.RoutingKey == RoutingKeyControl)
                                {
                                    resultLabel.Text = message.Text;
                                }
                            }

                        });
                    }
                    

                    await Task.CompletedTask;
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            };

            await Mqtt.Channel.BasicConsumeAsync(queue: ReceiverQueueName, autoAck: true, consumer: Consumer);
        }
        private async void OnRelayOnButtonClicked(object sender, EventArgs e)
        {
            var changeTo = "";
            if (relayOnButton.Text == "LOW")
            {
                changeTo = "HIGH";
                relayOnButton.Text = "HIGH";
            }
            else if (relayOnButton.Text == "HIGH")
            {
                changeTo = "LOW";
                relayOnButton.Text = "LOW";
            }
            var result = await Mqtt.SendMessageAsync(changeTo);

            SemanticScreenReader.Announce(relayOnButton.Text);
        }


    }

}

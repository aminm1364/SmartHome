using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SmartHome.Enums;
using SmartHome.Interfaces;
using SmartHome.Models;
using System.Text;

namespace SmartHome.Services
{
    public class MQTTConnection : IMQTTConnection
    {
        private string HOST_NAME = string.Empty;
        private int HOST_PORT = 5672;
        private string USERNAME = string.Empty;
        private string VIRTUAL_HOST = string.Empty;
        private string PASSWORD = string.Empty;
        private bool IsSSL = false;
        private string SSL_HOST_NAME = string.Empty;

        public string ExchangeName { get; private set; } = "amq.topic";
        public string ReceiverQueueName { get; private set; } = "relayQueue";
        public string RoutingKeyControl { get; private set; } = "relay.control";
        public string RoutingKeyStatus { get; private set; } = "relay.status";


        public IConnection? Connection { get; private set; }
        public IChannel? Channel { get; private set; }
        public IServerConfiguration Configuration { get; set; }

        public MQTTConnection(IServerConfiguration serverConfiguration)
        {
            this.ReceiverQueueName = $"{ReceiverQueueName}_{DeviceInfo.Current.Idiom}_{DeviceInfo.Current.Name}";
            this.Configuration = serverConfiguration;
            this.Configuration.GetConfigurations(() =>
            {
                this.SetLocalConfigurationParameters();
            });
        }

        public void RetrieveConfigurations(Action callback)
        {
            this.Configuration.GetConfigurations(callback);
        }

        private void SetLocalConfigurationParameters()
        {
            this.HOST_NAME = Configuration.Host;
            this.HOST_PORT = Configuration.HostPort;
            this.USERNAME = Configuration.Username;
            this.PASSWORD = Configuration.Password;
            this.VIRTUAL_HOST = Configuration.VirtualHost;
            this.IsSSL = Configuration.IsSSL;
            this.SSL_HOST_NAME = Configuration.SslHostName;
        }

        public async void InitConnection(Action<AsyncEventingBasicConsumer> callback)
        {

            if (this.Connection == null)
            {
                var factory = new ConnectionFactory() { HostName = HOST_NAME, UserName = USERNAME, Password = PASSWORD, Port = HOST_PORT, VirtualHost = VIRTUAL_HOST };
                if (IsSSL)
                {
                    factory.Ssl = new SslOption(serverName: SSL_HOST_NAME, enabled: IsSSL);
                }
                var connection = await factory.CreateConnectionAsync();
                Connection = connection;
                var channel = await connection.CreateChannelAsync();
                this.Channel = channel;
                ChannelExchangeDeclare(() =>
                {
                    DeclareQueueOnBroker(() =>
                    {
                        callback?.Invoke(new AsyncEventingBasicConsumer(Channel));
                    });
                });
            }
            else
                if (Channel != null) callback?.Invoke(new AsyncEventingBasicConsumer(Channel));
        }

        private async void ChannelExchangeDeclare(Action callback)
        {
            if (this.Channel == null) throw new Exception("Channel cannot be null");
            // Declare a topic exchange
            await Channel.ExchangeDeclareAsync(ExchangeName, ExchangeType.Topic, durable: true);

            callback?.Invoke();
        }

        private async void DeclareQueueOnBroker(Action callback)
        {
            if (this.Channel == null)
                throw new Exception("Channel cannot be null!");

            // Declare a topic exchange and a queue
            await Channel.ExchangeDeclareAsync(ExchangeName, ExchangeType.Topic, durable: true);
            await Channel.QueueDeclareAsync(
                                queue: ReceiverQueueName,
                                durable: true,
                                exclusive: false,
                                autoDelete: false);

            await Channel.QueueBindAsync(
                                queue: ReceiverQueueName,
                                exchange: ExchangeName,
                                routingKey: RoutingKeyControl);

            await Channel.QueueBindAsync(queue: ReceiverQueueName, exchange: ExchangeName, routingKey: RoutingKeyStatus);

            callback?.Invoke();
        }

        public async Task<Guid> SendMessageAsync(string message, bool isPrivate)
        {
            if (string.IsNullOrEmpty(message)) message = string.Empty;
            var props = new BasicProperties();
            var msg = new Message();
            var status = msg.ToJson(message, MessageDirectionType.Message, ReceiverQueueName, isPrivate);
            var body = Encoding.UTF8.GetBytes(status);

            if (this.Channel == null)
                throw new Exception("Channel cannot be null!");

            // Publish the message with a routing key
            await Channel.BasicPublishAsync(
                exchange: ExchangeName,
                routingKey: RoutingKeyControl,
                mandatory: false,
                props,
                body: body);

            return msg.Id;
        }

    }
}

using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SmartHome.Enums;
using SmartHome.Models;
using System.Text;
using System.Threading.Channels;

namespace SmartHome.Services
{
    public class MQTTConnection
    {
        private readonly string HOST_NAME = "192.168.64.20";
        private readonly string USERNAME = "amin";
        private readonly string PASSWORD = "amin";
        private readonly string ExchangeName = "amq.topic";
        private string ReceiverQueueName = "dotnet_receiver_queue";
        private readonly string QueueName = "mqtt-subscription-ESP8266Clientqos0";
        private readonly string RoutingKeyControl = "relay.control";
        private readonly string RoutingKeyStatus = "relay.status";

        public IConnection? Connection { get; private set; }
        public IChannel? Channel { get; private set; }

        public MQTTConnection(string receiverQueueName)
        {
            ReceiverQueueName = receiverQueueName;
        }
        public async Task Run()
        {
            await InitConnection();
        }

        public async Task InitConnection()
        {
            var factory = new ConnectionFactory() { HostName = HOST_NAME, UserName = USERNAME, Password = PASSWORD };

            var connection = await factory.CreateConnectionAsync();
            Connection = connection;

            var channel = await connection.CreateChannelAsync();
            this.Channel = channel;
            await ChannelExchangeDeclare();

            await Task.CompletedTask;
        }

        public async Task ChannelExchangeDeclare()
        {
            if (this.Channel == null) throw new Exception("Channel cannot be null");
            // Declare a topic exchange
            await Channel.ExchangeDeclareAsync(ExchangeName, ExchangeType.Topic, durable: true);
        }

        public async Task<Guid> SendMessageAsync(string message, bool isPrivate)
        {
            if (string.IsNullOrEmpty(message)) message = string.Empty;
            var props = new BasicProperties();
            var msg = new Message();
            var status = msg.ToJson(message, MessageDirectionType.Message, ReceiverQueueName, isPrivate);
            var body = Encoding.UTF8.GetBytes(status);

            // Publish the message with a routing key
            await Channel.BasicPublishAsync(
                exchange: ExchangeName,
                routingKey: RoutingKeyControl,
                mandatory: false,
                props,
                body: body);

            return msg.Id;
        }

        public async Task<AsyncEventingBasicConsumer> RunReceiver()
        {
            // Declare a topic exchange and a queue
            await Channel.ExchangeDeclareAsync(ExchangeName, ExchangeType.Topic, durable: true);
            await Channel.QueueDeclareAsync(
                                queue: ReceiverQueueName,
                                durable: true,
                                exclusive: false,
                                autoDelete: true);

            await Channel.QueueBindAsync(
                                queue: ReceiverQueueName,
                                exchange: ExchangeName,
                                routingKey: RoutingKeyControl);

            await Channel.QueueBindAsync(queue: ReceiverQueueName, exchange: ExchangeName, routingKey: RoutingKeyStatus);


            return new AsyncEventingBasicConsumer(Channel);

        }
    }
}

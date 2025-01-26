using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace SmartHome.Interfaces
{
    public interface IMQTTConnection
    {
        public string ExchangeName { get; }
        public string ReceiverQueueName { get; }
        public string RoutingKeyControl { get; }
        public string RoutingKeyStatus { get; }
        IChannel? Channel { get; }
        IConnection? Connection { get; }
        IServerConfiguration Configuration { get; }

        public void InitConnection(Action<AsyncEventingBasicConsumer> callback);

        public void RetrieveConfigurations(Action callback);

        public Task<Guid> SendMessageAsync(string message, bool isPrivate);
    }
}
using SmartHome.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SmartHome.Models
{

    public class Message
    {

        public Guid Id { get; private set; }
        public MessageDirectionType DirectionType { get; private set; }
        public string UserId { get; private set; }
        public DateTime Timestamp { get; private set; }
        public string Token { get; private set; }
        public string Text { get; private set; }    // to be used to be shown to users
        public string Status { get; private set; }  // to be used only for background tasks, or system commands

        public Message() { }

        [JsonConstructor]
        public Message(Guid id, MessageDirectionType directionType, string userId, DateTime timestamp, string token, string text, string status)
        {
            Id = id;
            DirectionType = directionType;
            UserId = userId;
            Timestamp = timestamp;
            Token = token;
            Text = text;
            Status = status;
        }

        public string ToJson(string text, MessageDirectionType directionType, string receiverQueueName)
        {
            Text = text;
            DirectionType = directionType;
            Token = "Auth2207|";
            Id = Guid.NewGuid();
            Timestamp = DateTime.UtcNow;
            UserId = receiverQueueName;
            return JsonSerializer.Serialize(this);
        }

        public static Message? Deserialize(string jsonMessage)
        {
            return JsonSerializer.Deserialize<Message>(jsonMessage);
        }
    }
}

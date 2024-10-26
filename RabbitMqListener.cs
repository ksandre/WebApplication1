using System;
using System.Text;
using Microsoft.AspNetCore.SignalR;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text.Json;
using System.Text.Json.Serialization;

public enum Status
{
    Init,
    Send,
    Completed,
}

// Define your message class to match the JSON structure
public class MyMessage
{
    [JsonPropertyName("shortTransactionId")]
    public string ShortTransactionId { get; set; }
    [JsonPropertyName("status")]
    public Status Status { get; set; }
    // Add other properties based on your JSON structure
}

public class RabbitMqListener
{
    private readonly IHubContext<ChatHub> _hubContext;
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public RabbitMqListener(IHubContext<ChatHub> hubContext)
    {
        _hubContext = hubContext;

        var factory = new ConnectionFactory() { HostName = "localhost" }; // Update with RabbitMQ settings
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.QueueDeclare(queue: "chat_queue", durable: false, exclusive: false, autoDelete: false, arguments: null);
    }

    public void StartListening()
    {
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            // Deserialize the JSON message body into an instance of MyMessage
            var messageString = Encoding.UTF8.GetString(body);
            var message = JsonSerializer.Deserialize<MyMessage>(messageString);

            // Send the received RabbitMQ message to all connected SignalR clients
            //await _hubContext.Clients.All.SendAsync("ReceiveMessage", message);

            if (message != null)
            {
                //var connectionId = ConnectionMappingCd.GetConnectionId(message.ShortTransactionId);
                var connectionId = await ConnectionMapping.GetConnectionIdAsync(message.ShortTransactionId);

                if (connectionId != null)
                {
                    await _hubContext.Clients.Client(connectionId).SendAsync("ReceiveMessage", JsonSerializer.Serialize(message));
                }
                System.Diagnostics.Debug.WriteLine($"connectionId {connectionId}");
                System.Diagnostics.Debug.WriteLine($"ReceiveMessage {JsonSerializer.Serialize(message)}");
            }
        };

        _channel.BasicConsume(queue: "chat_queue", autoAck: true, consumer: consumer);
    }

    public void StopListening()
    {
        _channel.Close();
        _connection.Close();
    }
}

using System;
using System.Text;
using Microsoft.AspNetCore.SignalR;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

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
            var message = Encoding.UTF8.GetString(body);

            // Send the received RabbitMQ message to all connected SignalR clients
            await _hubContext.Clients.All.SendAsync("ReceiveMessage", "RabbitMQ", message);
            System.Diagnostics.Debug.WriteLine($"ReceiveMessage {message}");
        };

        _channel.BasicConsume(queue: "chat_queue", autoAck: true, consumer: consumer);
    }

    public void StopListening()
    {
        _channel.Close();
        _connection.Close();
    }
}

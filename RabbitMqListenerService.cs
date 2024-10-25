using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

public class RabbitMqListenerService : BackgroundService
{
    private readonly RabbitMqListener _rabbitMqListener;

    public RabbitMqListenerService(RabbitMqListener rabbitMqListener)
    {
        _rabbitMqListener = rabbitMqListener;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _rabbitMqListener.StartListening();
        return Task.CompletedTask;
    }

    public override Task StopAsync(CancellationToken stoppingToken)
    {
        _rabbitMqListener.StopListening();
        return Task.CompletedTask;
    }
}

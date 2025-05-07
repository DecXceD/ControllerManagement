using ControllerManagement.Service;
using Microsoft.EntityFrameworkCore;
using System.Net.Sockets;
using System.Text;

namespace ControllerManagement.BackgroundServices
{
    public class UdpListenerService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private UdpClient? _udpClient;

        public UdpListenerService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _udpClient = new UdpClient(12345);

            while (!stoppingToken.IsCancellationRequested)
            {
                var result = await _udpClient.ReceiveAsync();
                string received = Encoding.UTF8.GetString(result.Buffer);

                try
                {
                    double[] numbers = received.Split(',').Select(double.Parse).ToArray();
                    int controllerId = (int)numbers[0];
                    double[] values = numbers.Skip(1).ToArray();

                    using var scope = _scopeFactory.CreateScope();
                    var service = scope.ServiceProvider.GetRequiredService<IControllerService>();

                    for (int i = 0; i < values.Length; i++)
                    {
                        service.LoadParameter(controllerId, i, values[i]);
                    }
                }
                catch (Exception)
                {
                    return;
                }
            }
        }

        public override void Dispose()
        {
            _udpClient?.Dispose();
            base.Dispose();
        }
    }
}

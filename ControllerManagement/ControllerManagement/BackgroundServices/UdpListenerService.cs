using ControllerManagement.Data;
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
            _udpClient = new UdpClient(12345); // Use your desired port

            while (!stoppingToken.IsCancellationRequested)
            {
                var result = await _udpClient.ReceiveAsync();
                string received = Encoding.UTF8.GetString(result.Buffer);
                //Processing information here:

                try
                {
                    int[] numbers = received.Split(',').Select(int.Parse).ToArray();
                    int virtualControllerId = numbers[0];
                    int[] values = numbers.Skip(1).ToArray();

                    using var scope = _scopeFactory.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<ControllerManagementContext>();

                    var controller = await db.Controllers
                        .FirstOrDefaultAsync(vc => vc.Id == virtualControllerId);

                    if (controller != null)
                    {
                        // Assuming the controller table has fixed columns like Value1, Value2, etc.
                        //controller.Value1 = values.ElementAtOrDefault(0);
                        //controller.Value2 = values.ElementAtOrDefault(1);
                        //controller.Value3 = values.ElementAtOrDefault(2);
                        // Continue as needed...

                        await db.SaveChangesAsync();
                    }
                }
                catch (Exception)
                {
                    
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

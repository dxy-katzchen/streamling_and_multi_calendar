

namespace Streamling.Service
{
    public class ReservationUpdateService(ILogger<ReservationUpdateService> logger, IServiceProvider serviceProvider) : IHostedService, IDisposable
    {
        private readonly ILogger<ReservationUpdateService> _logger = logger;
        private readonly IServiceProvider _serviceProvider = serviceProvider;
        private Timer _timer;

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Reservation Update Service is starting.");
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromMinutes(10));
            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            _logger.LogInformation("Reservation Update Service is working.");
            using var scope = _serviceProvider.CreateScope();
            var reservationService = scope.ServiceProvider.GetRequiredService<ReservationService>();
            reservationService.UpdateReservations().GetAwaiter().GetResult();
            _logger.LogInformation("Reservation Update Service completed successfully.");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Reservation Update Service is stopping.");
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
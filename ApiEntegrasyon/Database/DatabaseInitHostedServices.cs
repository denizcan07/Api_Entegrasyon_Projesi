using ApiEntegrasyon.Repository;

public class DatabaseInitHostedServices : IHostedService
{
    private readonly IServiceProvider _serviceProvider;

    public DatabaseInitHostedServices(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var initService = scope.ServiceProvider.GetRequiredService<IDatabaseInitServices>();

        await initService.InitializeAsync();
    }

    public Task StopAsync(CancellationToken cancellationToken)
        => Task.CompletedTask;
}

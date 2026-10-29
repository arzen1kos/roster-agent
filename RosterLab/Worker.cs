public class RosterWorker(IServiceScopeFactory sf, IConfiguration cfg, ILogger<RosterWorker> log) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        using var timer = new PeriodicTimer(TimeSpan.Parse(cfg["Roster:Interval"]!));
        do
        {
            try
            {
                using var scope = sf.CreateScope();
                await scope.ServiceProvider.GetRequiredService<RosterAgent>()
                    .RunAsync(cfg.GetValue<int>("Roster:WindowDays"), ct);
            }
            catch (Exception e) when (e is not OperationCanceledException) { log.LogError(e, "Run failed"); }
        } while (await timer.WaitForNextTickAsync(ct));
    }
}
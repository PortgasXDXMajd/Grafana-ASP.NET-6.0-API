using Prometheus;

public interface IMetricsService
{
    void Inc();
}

public class MetricsService : IMetricsService
{
    private readonly Gauge usersGauge;

    public MetricsService()
    {
        usersGauge = Metrics.CreateGauge("app_users_total", "Number of registered users in the application");
    }

    public void Inc()
    {
        usersGauge.Inc();
    }
}
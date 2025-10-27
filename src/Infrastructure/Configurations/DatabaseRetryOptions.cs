namespace Infrastructure.Configurations;

public class DatabaseRetryOptions
{
    public int MaxRetryCount { get; set; } = 5;
    public double MaxRetryDelaySeconds { get; set; } = 2;
    public string[] ErrorCodesToRetry { get; set; } = new[] { "40001", "40P01" };
}
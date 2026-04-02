namespace Tsubaki.Models;

public class ServiceStateModel
{
    public string ServiceId { get; set; } = string.Empty;
    public int RetryCount { get; set; } = 0;
    public CancellationTokenSource? CtsSource { get; set; } = null;
    public bool IsRunning { get; set; } = false;
}

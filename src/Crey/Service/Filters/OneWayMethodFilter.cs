namespace Crey.Service;

/// <summary>
/// Filter used to schedule further execution in background <see cref="Task"/> as <see cref="TaskCreationOptions.LongRunning"/>
/// </summary>
public class OneWayMethodFilter : IServiceMethodFilter
{
    private readonly ILogger<OneWayMethodFilter> _logger;

    /// <summary>
    /// Filter constructor
    /// </summary>
    /// <param name="logger">Current class logger instance</param>
    public OneWayMethodFilter(ILogger<OneWayMethodFilter> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public Task Execute(MessageInvoke message, NextServiceDelegate next)
    {
        if (_logger.IsEnabled(LogLevel.Information))
            _logger.LogInformation($"Perform {nameof(Task)} execution as {nameof(TaskCreationOptions.LongRunning)}");

        _ = Task.Factory.StartNew(() => next(), TaskCreationOptions.LongRunning);

        return Task.CompletedTask;
    }
}
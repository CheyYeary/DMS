namespace DMS.Logging
{
    /// <summary>
    /// Console json logger implementation class.
    /// </summary>
    public class ConsoleJsonLogger : ILogger
    {
        public IDisposable BeginScope<TState>(TState state) => default!;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }
            
            Console.WriteLine($"[{eventId.Id,2}: {logLevel,-12}]");
            Console.Write($"{formatter(state, exception)}");
            Console.WriteLine();
        }
    }
}

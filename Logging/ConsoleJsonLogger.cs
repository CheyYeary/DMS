using System.Text.Json;
using System;

namespace DMS.Logging
{
    /// <summary>
    /// Console json logger implementation class.
    /// </summary>
    public class ConsoleJsonLogger : ILogger
    {
        public IDisposable BeginScope<TState>(TState state) => default!;

        public bool IsEnabled(LogLevel logLevel) => true;

        /// <summary>
        /// Log event
        /// </summary>
        /// <typeparam name="T">Type of event</typeparam>
        /// <param name="e">Event to log</param>
        public void Log<T>(T e) where T : EventBase
        {
            
        }

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

    /// <summary>
    /// Event base class
    /// </summary>
    abstract public partial class EventBase 
    {
        /// <summary>
        /// Initializes an instance of the Event base class
        /// </summary>
        public EventBase()
            : this("DMS.Logging.EventBase", "EventBase")
        { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="fullName">Full event name</param>
        /// <param name="name">Event name</param>
        protected EventBase(string fullName, string name)
        {
            this.EventName = this.GetType().Name;
        }

        /// <summary>
        /// Gets the event name
        /// </summary>
        public string EventName { get; }

        /// <summary>
        /// Convert nulls to a string "null" as Geneva logging doesn't support null values.
        /// </summary>
        public virtual string ConvertDefault(string value) => value ?? "null";
    }
}

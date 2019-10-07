using System;
using System.Linq;

namespace KY.Core
{
    public static class Logger
    {
        public static event EventHandler<EventArgs<LogEntry>> Added
        {
            add => Event.Added += value;
            remove => Event.Added -= value;
        }

        public static LogTargets TraceTargets { get; }
        public static LogTargets WarningTargets { get; }
        public static LogTargets ErrorTargets { get; }
        public static AllLogTargets AllTargets { get; }

        public static ConsoleTarget Console { get; }
        public static FileTarget File { get; }
        public static EventLogTarget EventLog { get; set; }
        public static EventTarget Event { get; }
        public static VisualStudioOutputTarget VisualStudioOutput { get; }

        public static LoggerExtension Extension { get; }
        public static string Source { get; set; }

        static Logger()
        {
            Console = new ConsoleTarget();
            File = new FileTarget();
            Event = new EventTarget();
            VisualStudioOutput = new VisualStudioOutputTarget();
            TraceTargets = new LogTargets();
            WarningTargets = new LogTargets();
            ErrorTargets = new LogTargets();
            AllTargets = new AllLogTargets(TraceTargets, WarningTargets, ErrorTargets);
            AllTargets.Add(Console);
            AllTargets.Add(File);
            AllTargets.Add(Event);
            Extension = new LoggerExtension();
        }

        public static void Trace(string message)
        {
            TraceTargets.Write(LogEntry.Trace(message));
        }

        public static void TraceFull(string message)
        {
            TraceTargets.Write(LogEntry.Trace(message).Unshortable());
        }

        public static void Warning(string message)
        {
            WarningTargets.Write(LogEntry.Warning(message));
        }

        public static void Error(Exception exception)
        {
            ErrorTargets.Write(LogEntry.Error(exception));
        }

        public static void Error(string message, string customType = null, string source = null)
        {
            ErrorTargets.Write(LogEntry.Error(message, customType, source));
        }

        public static void Wait()
        {
            AllTargets.OfType<QueuedLogTarget>().ForEach(x => x.Wait());
        }

        public static void CatchAll()
        {
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
        }

        public static void CatchNone()
        {
            AppDomain.CurrentDomain.UnhandledException -= OnUnhandledException;
        }

        private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Error(e.ExceptionObject as Exception);
        }
    }
}
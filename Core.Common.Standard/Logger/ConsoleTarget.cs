using System;
using System.IO;
using KY.Core.Properties;

namespace KY.Core
{
    public class ConsoleTarget : LogTarget
    {
        public bool IsConsoleAvailable { get; private set; }
        public int StartLeft { get; private set; }
        public int StartTop { get; private set; }
        public bool ShortenEntries { get; set; } = true;

        public ConsoleTarget()
        {
            this.IsConsoleAvailable = true;
        }

        public override void Write(LogEntry entry)
        {
            if (!this.IsConsoleAvailable)
            {
                return;
            }
            try
            {
                string formattedMessage;
                if (entry.Type == LogType.Error)
                {
                    formattedMessage = string.Format(Resources.ConsoleErrorFormat, entry.Timestamp, entry.CustomType, entry.Message);
                }
                else
                {
                    formattedMessage = string.Format(Resources.ConsoleTraceFormat, entry.Timestamp, entry.Message);
                }

                if (entry.Shortable && this.ShortenEntries && formattedMessage.Length >= Console.WindowWidth && Console.WindowWidth > 0)
                {
                    formattedMessage = formattedMessage.Substring(0, Console.WindowWidth - 4) + "...";
                }

                Console.WriteLine(Console.WindowWidth > 0 ? formattedMessage.PadRight(Console.WindowWidth - 1) : formattedMessage);
            }
            catch (IOException)
            {
                this.IsConsoleAvailable = false;
                Logger.Warning("Console output is not available");
            }
        }

        public void Move(int left, int top)
        {
            Console.SetCursorPosition(left, top);
        }

        public void SetStart(int left, int top)
        {
            this.StartLeft = left;
            this.StartTop = top;
        }

        public void MoveToStart()
        {
            this.Move(this.StartLeft, this.StartTop);
        }
    }
}
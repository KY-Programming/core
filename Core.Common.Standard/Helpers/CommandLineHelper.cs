using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace KY.Core
{
    public static class CommandLineHelper
    {
        private static bool IsWindows => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        public static bool RunWithTrace(StringBuilder commands)
        {
            return RunWithTrace(commands.ToString());
        }

        public static bool RunWithTrace(string commands)
        {
            return Run(commands, out string _);
        }

        public static bool RunWithResult(string commands, out string result)
        {
            return Run(commands, out result, Mode.Output);
        }

        private static bool Run(string commands, out string result, Mode mode = Mode.Trace)
        {
            bool isWindows = IsWindows;
            StringBuilder errorBuilder = new();
            StringBuilder outputBuilder = new();
            Process cmd = new();
            // cmd.exe /k and sh -s both read the command list from stdin and exit once it is closed
            cmd.StartInfo.FileName = isWindows ? "cmd.exe" : "/bin/sh";
            cmd.StartInfo.Arguments = isWindows ? "/k" : "-s";
            cmd.StartInfo.RedirectStandardInput = true;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.RedirectStandardError = true;
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.UseShellExecute = false;
            if (mode == Mode.Trace)
            {
                cmd.OutputDataReceived += (sender, args) => Logger.Trace(args.Data!);
            }
            if (mode == Mode.Output)
            {
                if (isWindows)
                {
                    commands = "@echo OFF\nset PROMPT=$+\n" + commands;
                }
                cmd.OutputDataReceived += (sender, args) => outputBuilder.AppendLine(args.Data);
            }
            cmd.ErrorDataReceived += (sender, args) => errorBuilder.AppendLine(args.Data);
            cmd.Start();
            cmd.BeginOutputReadLine();
            cmd.BeginErrorReadLine();
            cmd.StandardInput.WriteLine(commands);
            cmd.StandardInput.Flush();
            cmd.StandardInput.Close();
            cmd.WaitForExit();
            result = outputBuilder.ToString().Replace("\r", string.Empty);
            if (isWindows)
            {
                int commandsIndex = result.IndexOf(commands.Replace("\r", string.Empty));
                if (commandsIndex >= 0)
                {
                    int commandsEnd = commandsIndex + commands.Length;
                    result = result.Substring(commandsEnd, result.Length - commandsEnd).Trim();
                }
            }
            else
            {
                result = result.Trim();
            }
            if (errorBuilder.Length > 0 && cmd.ExitCode != 0)
            {
                Logger.Error(errorBuilder.ToString());
                return false;
            }
            if (errorBuilder.Length > 0)
            {
                Logger.Warning(errorBuilder.ToString());
            }
            return true;
        }

        private enum Mode
        {
            Trace,
            Output
        }
    }
}

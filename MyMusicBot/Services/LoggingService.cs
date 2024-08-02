using Microsoft.Extensions.Logging;
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;

using System.Threading.Tasks;

namespace MyMusicBot.Services
{
    public class LoggingService
    {
        public static async Task LogAsync(string src, LogLevel severity, string message, Exception exception = null)
        {
            if (severity.Equals(null))
            {
                severity = LogLevel.Warning;
            }
            await Append($"{GetSeverityString(severity)}", GetConsoleColor(severity));
            await Append($" [{SourceToString(src)}] ", ConsoleColor.DarkGray);

            if (!string.IsNullOrWhiteSpace(message))
                await Append($"{message}\n", ConsoleColor.White);
            else if (exception == null)
            {
                await Append("Uknown Exception. Exception Returned Null.\n", ConsoleColor.DarkRed);
            }
            else if (exception.Message == null)
                await Append($"Unknownk \n{exception.StackTrace}\n", GetConsoleColor(severity));
            else
                await Append($"{exception.Message ?? "Unknownk"}\n{exception.StackTrace ?? "Unknown"}\n", GetConsoleColor(severity));
        }
        private static string GetSeverityString(LogLevel severity)
        {
            switch (severity)
            {
                case LogLevel.Critical:
                    return "CRIT";
                case LogLevel.Debug:
                    return "DBUG";
                case LogLevel.Error:
                    return "EROR";
                case LogLevel.Information:
                    return "INFO";
                case LogLevel.Trace:
                    return "VERB";
                case LogLevel.Warning:
                    return "WARN";
                default: return "UNKN";
            }
        }
        private static async Task Append(string message, ConsoleColor color)
        {
            await Task.Run(() => {
                Console.ForegroundColor = color;
                Console.Write(message);
            });
        }

        private static string SourceToString(string src)
        {
            switch (src.ToLower())
            {
                case "discord":
                    return "DISCD";
                case "audio":
                    return "AUDIO";
                case "admin":
                    return "ADMIN";
                case "gateway":
                    return "GTWAY";
                case "blacklist":
                    return "BLAKL";
                case "lavanode_0_socket":
                    return "LAVAS";
                case "lavanode_0":
                    return "LAVA#";
                case "bot":
                    return "BOTWN";
                default:
                    return src;
            }
        }
        public static async Task LogInformationAsync(string source, string message)
            => await LogAsync(source, LogLevel.Information, message);

        private static ConsoleColor GetConsoleColor(LogLevel severity)
        {
            switch (severity)
            {
                case LogLevel.Critical:
                    return ConsoleColor.Red;
                case LogLevel.Debug:
                    return ConsoleColor.Magenta;
                case LogLevel.Error:
                    return ConsoleColor.DarkRed;
                case LogLevel.Information:
                    return ConsoleColor.Green;
                case LogLevel.Trace:
                    return ConsoleColor.DarkCyan;
                case LogLevel.Warning:
                    return ConsoleColor.Yellow;
                default: return ConsoleColor.White;
            }
        }
    }
}

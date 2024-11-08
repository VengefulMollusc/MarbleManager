﻿using System;
using System.IO;
using System.Linq;

namespace MarbleManager.Config
{
    /**
     * Handles logging, writing to both console and file
     */
    internal static class LogManager
    {
        internal static void WriteLog(string _message, bool _useSecondary = false)
        {
            WriteLog(_message, null, _useSecondary);
        }
        internal static void WriteLog(string _message, string _extraDetails, bool _useSecondary = false)
        {
            // write message to console
            Console.WriteLine(_message);

            // Get the current timestamp in the desired format
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH:mm:ss");

            // Combine the timestamp and message
            string logEntry = $"{timestamp} - {_message}";
            if (_extraDetails != null)
            {
                // append extra details
                logEntry += $" :: {_extraDetails}";
            }

            // Define the path to the log file (assuming it's in the current directory)
            string logFilePath = _useSecondary ? PathManager.LogSecondaryFilePath : PathManager.LogFilePath;

            try
            {
                // Check if the log file exists; create it if it doesn't
                if (!File.Exists(logFilePath))
                {
                    File.Create(logFilePath).Close();
                }

                // Read the current lines from the log file
                string[] lines = File.ReadAllLines(logFilePath);

                // Append the log entry to the file
                File.AppendAllText(logFilePath, logEntry + Environment.NewLine);

                // Check if the file exceeds 100 lines
                if (lines.Length > 100)
                {
                    // Keep the last 50 lines and add "old log cleanup" at the end
                    var newLines = lines.Skip(lines.Length - 50).Concat(new[] { "old log cleanup" });
                    File.WriteAllLines(logFilePath, newLines);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error writing to log file: {e.Message}");
            }
        }
    }
}

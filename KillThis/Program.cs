using System;
using System.Diagnostics;
using System.IO;

class Program
{
    static void Main(string[] args)
    {
        try
        {
            // Get the directory where the executable is located
            string exeDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string killFilePath = Path.Combine(exeDirectory, "kill.txt");

            // Check if kill.txt exists
            if (!File.Exists(killFilePath))
            {
                Console.WriteLine("Error: kill.txt not found in the executable's directory.");
                return;
            }

            // Read process names from kill.txt
            string[] processNames = File.ReadAllLines(killFilePath);

            if (processNames.Length == 0)
            {
                Console.WriteLine("No process names found in kill.txt.");
                return;
            }

            // Iterate through each process name
            foreach (string processName in processNames)
            {
                // Skip empty lines
                if (string.IsNullOrWhiteSpace(processName))
                    continue;

                // Clean the process name (remove .exe if included)
                string cleanProcessName = processName.EndsWith(".exe", StringComparison.OrdinalIgnoreCase)
                    ? processName.Substring(0, processName.Length - 4)
                    : processName;

                try
                {
                    // Get all processes with the specified name
                    Process[] processes = Process.GetProcessesByName(cleanProcessName);

                    if (processes.Length == 0)
                    {
                        Console.WriteLine($"No running processes found for '{cleanProcessName}'.");
                        continue;
                    }

                    // Kill each process
                    foreach (Process process in processes)
                    {
                        try
                        {
                            process.Kill();
                            Console.WriteLine($"Successfully terminated process '{cleanProcessName}' (PID: {process.Id}).");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Failed to terminate process '{cleanProcessName}' (PID: {process.Id}): {ex.Message}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error accessing processes for '{cleanProcessName}': {ex.Message}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }
}

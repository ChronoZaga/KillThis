using System;
using System.Diagnostics;
using System.IO;

class Program
{
    static int Main(string[] args)
    {
        int exitCode = 0;

        try
        {
            string exePath = System.Reflection.Assembly.GetEntryAssembly().Location;
            string exeDirectory = Path.GetDirectoryName(exePath);
            string listFileName = Path.GetFileNameWithoutExtension(exePath) + ".txt";
            string killFilePath = Path.Combine(exeDirectory, listFileName);

            if (!File.Exists(killFilePath))
            {
                Console.WriteLine(string.Format("Error: {0} not found in the executable's directory.", listFileName));
                return 1;
            }

            string[] processNames = File.ReadAllLines(killFilePath);
            int selfId = Process.GetCurrentProcess().Id;

            if (processNames.Length == 0)
            {
                Console.WriteLine(string.Format("No process names found in {0}.", listFileName));
                return 0;
            }

            foreach (string rawProcessName in processNames)
            {
                string processName = rawProcessName.Trim();

                if (processName.Length == 0 || processName.StartsWith("#"))
                    continue;

                processName = processName.Trim('"').Trim();

                int lastSlash = processName.LastIndexOfAny(new char[] { '\\', '/' });
                if (lastSlash >= 0)
                    processName = processName.Substring(lastSlash + 1);

                if (processName.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
                    processName = processName.Substring(0, processName.Length - 4);

                string cleanProcessName = processName.Trim();

                if (cleanProcessName.Length == 0)
                    continue;

                try
                {
                    Process[] processes = Process.GetProcessesByName(cleanProcessName);

                    if (processes.Length == 0)
                    {
                        Console.WriteLine(string.Format("No running processes found for '{0}'.", cleanProcessName));
                        continue;
                    }

                    foreach (Process process in processes)
                    {
                        if (process.Id == selfId)
                            continue;

                        try
                        {
                            process.Kill();
                            Console.WriteLine(string.Format("Successfully terminated process '{0}' (PID: {1}).", cleanProcessName, process.Id));
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(string.Format("Failed to terminate process '{0}' (PID: {1}): {2}", cleanProcessName, process.Id, ex.Message));
                            exitCode = 1;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(string.Format("Error accessing processes for '{0}': {1}", cleanProcessName, ex.Message));
                    exitCode = 1;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(string.Format("An error occurred: {0}", ex.Message));
            exitCode = 1;
        }

        return exitCode;
    }
}

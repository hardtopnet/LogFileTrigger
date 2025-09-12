using System.Diagnostics;

internal class Program
{
    private static long _lastPosition = 0;
    private static string _logFilePath;
    private static string _pattern;
    private static string _command;

    private static void Main(string[] args)
    {
        try
        {
            var settings = Settings.Load("settings.json");
            _logFilePath = settings.LogFilePath;
            _pattern = settings.Pattern; // Read pattern from settings  
            _command = settings.Command; // Read command from settings

            using var fileWatcher = new FileSystemWatcher(Path.GetDirectoryName(_logFilePath))
            {
                Filter = Path.GetFileName(_logFilePath),
                NotifyFilter = NotifyFilters.LastWrite
            };

            _lastPosition = new FileInfo(_logFilePath).Length;
            fileWatcher.Changed += (sender, e) => OnChanged(sender, e);
            fileWatcher.EnableRaisingEvents = true;

            // Start a periodic check task
            Task.Run(() => PeriodicCheck());

            Console.WriteLine("Monitoring log file for new lines. Press [enter] to exit.");
            Console.ReadLine();
        }
        catch (FileNotFoundException ex)
        {
            Console.WriteLine($"Error: File not found. {ex.Message}");
        }
        catch (UnauthorizedAccessException ex)
        {
            Console.WriteLine($"Error: Access denied. {ex.Message}");
        }
        catch (IOException ex)
        {
            Console.WriteLine($"Error: IO exception. {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An unexpected error occurred: {ex.Message}");
        }
    }

    private static void OnChanged(object sender, FileSystemEventArgs e)
    {
        try
        {
            using var stream = new FileStream(e.FullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            stream.Seek(_lastPosition, SeekOrigin.Begin);

            using var reader = new StreamReader(stream);
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                Console.WriteLine(line);
                if (line.Contains(_pattern, StringComparison.InvariantCultureIgnoreCase)) // Check if line contains the pattern  
                {
                    Console.WriteLine($"Pattern {_pattern} found, executing command: {_command}");

                    // Execute command if needed
                    var process = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = "cmd.exe",
                            Arguments = $"/C {_command}",
                            RedirectStandardOutput = true,
                            UseShellExecute = false,
                            CreateNoWindow = true
                        }
                    };
                    process.Start();
                    Environment.Exit(0); // Quit application immediately after new process is spawned
                }
            }

            _lastPosition = stream.Position;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing file change: {ex.Message}");
        }
    }

    private static async Task PeriodicCheck()
    {
        while (true)
        {
            try
            {
                var fileInfo = new FileInfo(_logFilePath);
                if (fileInfo.Length > _lastPosition)
                {
                    OnChanged(null, new FileSystemEventArgs(WatcherChangeTypes.Changed, Path.GetDirectoryName(_logFilePath), Path.GetFileName(_logFilePath)));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during periodic check: {ex.Message}");
            }

            await Task.Delay(1000); // Check every second
        }
    }
}

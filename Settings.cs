using System.Text.Json;

public class Settings
{
    public string LogFilePath { get; set; }
    public string Pattern { get; set; }
    public string Command { get; set; }

    public static Settings Load(string filePath)
    {
        if (!File.Exists(filePath))
        {
            File.WriteAllText(filePath, "");
        }

        var json = File.ReadAllText(filePath);
        return JsonSerializer.Deserialize<Settings>(json);
    }
}

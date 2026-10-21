namespace SecureAiAgentDemo.Services;

public class AuditLogger
{
    private readonly string _filePath;

    public AuditLogger()
    {
        string folder = Path.Combine(
            Environment.GetFolderPath(
                Environment.SpecialFolder.LocalApplicationData),
            "SecureAiAgentDemo");

        Directory.CreateDirectory(folder);

        _filePath = Path.Combine(folder, "audit.log");
    }

    public void Log(
        string role,
        string action,
        bool successful)
    {
        string entry =
            $"{DateTimeOffset.UtcNow:O} | " +
            $"Role={Sanitize(role)} | " +
            $"Action={Sanitize(action)} | " +
            $"Success={successful}";

        File.AppendAllText(
            _filePath,
            entry + Environment.NewLine);
    }

    private static string Sanitize(string value)
    {
        return new string(
            value.Where(c =>
                char.IsLetterOrDigit(c) ||
                c == '_' || c == '-')
                .Take(80)
                .ToArray());
    }

    public string ReadLog()
    {
        if (!File.Exists(_filePath))
            return "No audit records available.";

        return File.ReadAllText(_filePath);
    }
}


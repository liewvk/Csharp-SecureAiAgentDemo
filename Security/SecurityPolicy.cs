namespace SecureAiAgentDemo.Security;

public class SecurityPolicy
{
    private readonly Dictionary<string, string[]> _permissions =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["Viewer"] = ["read_products"],
            ["Editor"] = ["read_products", "prepare_report"],
            ["Administrator"] =
                ["read_products", "prepare_report"]
        };

    public bool IsAuthorized(string role, string toolName)
    {
        if (!_permissions.TryGetValue(
            role, out string[]? allowedTools))
        {
            return false;
        }

        return allowedTools.Contains(
            toolName,
            StringComparer.OrdinalIgnoreCase);
    }

    public string ExecuteTool(string role, string toolName)
    {
        if (!IsAuthorized(role, toolName))
        {
            return "ACCESS DENIED: " +
                   $"Role '{role}' cannot execute '{toolName}'.";
        }

        return toolName.ToLowerInvariant() switch
        {
            "read_products" =>
                "Laptop: RM3500, Stock: 15\n" +
                "Monitor: RM850, Stock: 30",

            "prepare_report" =>
                "Report preparation is authorized. " +
                "The user must review the resulting draft.",

            _ => "Unknown or unsupported tool."
        };
    }
}

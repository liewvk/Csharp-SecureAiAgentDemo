using SecureAiAgentDemo.Security;
using SecureAiAgentDemo.Services;

Console.Title = "Secure AI Agent Demo";

var policy = new SecurityPolicy();
var audit = new AuditLogger();
using var aiService = new OllamaService();

string[] roles =
[
    "Viewer",
    "Editor",
    "Administrator"
];

Console.WriteLine("==================================");
Console.WriteLine("       SECURE AI AGENT DEMO");
Console.WriteLine("       Visual Studio 2026");
Console.WriteLine("       Ollama + Qwen 3.5");
Console.WriteLine("==================================");

Console.WriteLine();
Console.WriteLine("Select demonstration role:");
Console.WriteLine("1. Viewer");
Console.WriteLine("2. Editor");
Console.WriteLine("3. Administrator");

Console.Write("Choice: ");

string? roleChoice = Console.ReadLine();

string role = roleChoice switch
{
    "1" => "Viewer",
    "2" => "Editor",
    "3" => "Administrator",
    _ => "Viewer"
};

Console.WriteLine($"Active demo role: {role}");

while (true)
{
    Console.WriteLine();
    Console.WriteLine("1. Ask Qwen a Question");
    Console.WriteLine("2. Test Tool Authorization");
    Console.WriteLine("3. Create Report Draft");
    Console.WriteLine("4. View Audit Log");
    Console.WriteLine("0. Exit");
    Console.WriteLine();

    Console.Write("Select option: ");
    string? choice = Console.ReadLine();

    if (choice == "0")
        break;

    try
    {
        switch (choice)
        {
            case "1":
                Console.Write("Enter your question: ");

                string question =
                    Console.ReadLine() ?? "";

                string answer =
                    await aiService.GenerateAsync(question);

                Console.WriteLine();
                Console.WriteLine("Assistant:");
                Console.WriteLine(answer);

                audit.Log(role, "ai_question", true);
                break;

            case "2":
                Console.WriteLine("Available test tools:");
                Console.WriteLine("read_products");
                Console.WriteLine("prepare_report");
                Console.WriteLine("delete_products");

                Console.Write("Tool name: ");

                string tool =
                    (Console.ReadLine() ?? "").Trim();

                bool authorized =
                    policy.IsAuthorized(role, tool);

                string result =
                    policy.ExecuteTool(role, tool);

                Console.WriteLine();
                Console.WriteLine(result);

                audit.Log(role, tool, authorized);
                break;

            case "3":
                const string reportTool = "prepare_report";

                if (!policy.IsAuthorized(role, reportTool))
                {
                    Console.WriteLine(
                        "Access denied: report preparation.");

                    audit.Log(role, reportTool, false);
                    break;
                }

                Console.Write("Report topic: ");

                string topic =
                    Console.ReadLine() ?? "";

                if (string.IsNullOrWhiteSpace(topic))
                {
                    Console.WriteLine(
                        "Report topic is required.");
                    break;
                }

                string prompt =
                    "Prepare a short C# project report outline " +
                    "about the following topic. " +
                    "Use headings and avoid unsupported facts.\n" +
                    topic;

                string draft =
                    await aiService.GenerateAsync(prompt);

                Console.WriteLine();
                Console.WriteLine("REPORT DRAFT:");
                Console.WriteLine(draft);

                Console.WriteLine();
                Console.Write(
                    "Review complete. Save draft? Type YES: ");

                string? confirmation = Console.ReadLine();

                if (confirmation != "YES")
                {
                    Console.WriteLine("Draft not saved.");
                    audit.Log(role, "save_report", false);
                    break;
                }

                string folder = Path.Combine(
                    Environment.GetFolderPath(
                        Environment.SpecialFolder.LocalApplicationData),
                    "SecureAiAgentDemo",
                    "Drafts");

                Directory.CreateDirectory(folder);

                string filePath = Path.Combine(
                    folder,
                    $"Report_{Guid.NewGuid():N}.txt");

                using (var stream = new FileStream(
                    filePath,
                    FileMode.CreateNew,
                    FileAccess.Write,
                    FileShare.None))
                using (var writer = new StreamWriter(stream))
                {
                    writer.Write(draft);
                }

                Console.WriteLine("Draft saved:");
                Console.WriteLine(filePath);

                audit.Log(role, "save_report", true);
                break;

            case "4":
                Console.WriteLine();
                Console.WriteLine("Security Audit Log:");
                Console.WriteLine(audit.ReadLog());

                break;

            default:
                Console.WriteLine("Invalid selection.");
                break;
        }
    }
    catch (HttpRequestException ex)
    {
        Console.WriteLine(
            "Ollama API error: " + ex.Message);

        audit.Log(role, "api_error", false);
    }
    catch (OperationCanceledException)
    {
        Console.WriteLine("AI request timed out.");
        audit.Log(role, "timeout", false);
    }
    catch (Exception ex)
    {
        Console.WriteLine("Error: " + ex.Message);
        audit.Log(role, "operation_error", false);
    }
}

Console.WriteLine("Goodbye!");


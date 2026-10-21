# Secure AI Agent Demo

This repository is a small C# demonstration of a secure AI agent pattern using Ollama and a role-based access control model. It highlights how an AI assistant can be constrained to a limited set of tools and how every operation can be logged for auditability.

## What this demo shows

- Role-based access control for AI actions
- Tool authorization checks before execution
- Restricted AI behavior using a system prompt
- Audit logging of user actions and outcomes
- A simple local workflow that calls Ollama at `http://localhost:11434`

## Project structure

- `Program.cs` - console application entry point and menu flow
- `Security/SecurityPolicy.cs` - permission policy and tool execution gates
- `Services/OllamaService.cs` - HTTP client wrapper for the Ollama chat API
- `Services/AuditLogger.cs` - writes audit entries to a local log file
- `SecureAiAgentDemo.csproj` - .NET project configuration

## Roles and permissions

The demo supports three roles:

- `Viewer`
  - Can read products
- `Editor`
  - Can read products
  - Can prepare a report draft
- `Administrator`
  - Can read products
  - Can prepare a report draft

The `SecurityPolicy` class enforces these permissions and returns a denial message if a role attempts an unauthorized action.

## Example menu options

The console app allows users to:

1. Ask Qwen a question
2. Test tool authorization
3. Create a report draft
4. View the audit log
0. Exit

## Local environment notes

This demo is designed for a local developer machine and uses:

- Ollama running locally on port 11434
- The model `qwen3.5:9b`
- A local application data folder for audit logs and generated drafts

## Prerequisites

- .NET 10 SDK
- Ollama installed and running locally
- Pull/download the model:

```bash
ollama pull qwen3.5:9b
```

## Run the project

From the repository root:

```bash
dotnet restore
dotnet run
```

## Security design goals

This sample is intentionally simplified and designed for learning. It illustrates a few important guardrails:

- Treat AI-generated output as untrusted input
- Keep tool access behind explicit authorization checks
- Log security decisions and operations for review
- Force a human review before saving generated drafts

## Notes

The project is a lightweight educational example for a secure AI-agent architecture pattern in C#. It is not intended to be a production-ready security boundary by itself.

## License

This project does not currently declare a license. If you are publishing or reusing this code, check whether a license file is required for your environment.

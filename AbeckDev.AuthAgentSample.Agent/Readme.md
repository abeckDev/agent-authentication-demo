## AbeckDev.AuthAgentSample.Agent

A simple AI agent that acquires a token from the currently signed-in Azure CLI user and calls the [`AbeckDev.AuthAgentSample.DebugFunction`](../AbeckDev.AuthAgentSample.DebugFunction/Readme.md) on behalf of that user.

This project acts as a **placeholder for a real frontend-based user authentication flow** (e.g., OAuth2 / MSAL). In production, the token would be obtained from a browser-based sign-in rather than the Azure CLI session.

### Overview

The agent:

1. Acquires the signed-in user's access token using `AzureCliCredential`.
2. Registers a tool (`DoTheThing`) that attaches the token as a `Bearer` header and POSTs to the debug function.
3. Creates an AI agent (Azure OpenAI) and asks it: *"Would you please do the thing?"*
4. The AI model decides to invoke the tool, which calls the debug function.
5. The debug function logs and returns the full request details — including the decoded JWT — so you can verify credential delegation is working.

### Prerequisites

| Tool | Version |
|---|---|
| .NET SDK | 9.0+ |
| Azure CLI | Latest (must be signed in via `az login`) |
| Azure OpenAI resource | — |

### Configuration

Set the following environment variables before running:

| Variable | Required | Description |
|---|---|---|
| `AZURE_OPENAI_ENDPOINT` | ✅ | Azure OpenAI endpoint URL, e.g. `https://<resource>.openai.azure.com/` |
| `AZURE_OPENAI_DEPLOYMENT_NAME` | ❌ | Model deployment name (default: `gpt-4o-mini`) |
| `API_SCOPE` | ❌ | OAuth2 scope to request the token for (default: `https://management.azure.com/.default`) |

### Running Locally

Make sure the debug function is running first (see its [Readme](../AbeckDev.AuthAgentSample.DebugFunction/Readme.md)), then:

```bash
az login
cd AbeckDev.AuthAgentSample.Agent
dotnet run
```

Expected console output:

```
Token acquired for: Azure CLI User
Expires: <expiry timestamp>
<agent response confirming it did the thing>
```

The decoded JWT and full request details will appear in the debug function's console output.

### Key Files

| File | Purpose |
|---|---|
| `Program.cs` | Agent entry point — token acquisition, tool registration, agent invocation |
| `appsettings.json` | Default application settings |
| `Properties/launchSettings.json` | Local run profile |

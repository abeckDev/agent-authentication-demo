# Simple Agent Authentication Sample

A demonstration project that shows how credential delegation works when an AI agent calls an Azure Function on behalf of a user. The function inspects the incoming HTTP request and surfaces all authentication details — including decoded JWT bearer tokens — as a human-readable HTML page.

> **Status:** Work in Progress

---

## Table of Contents

- [What are Azure Functions?](#what-are-azure-functions)
- [Project Overview](#project-overview)
- [Architecture](#architecture)
- [Prerequisites](#prerequisites)
- [Getting Started](#getting-started)
  - [Clone the repository](#clone-the-repository)
  - [Run locally](#run-locally)
  - [Test the endpoint](#test-the-endpoint)
- [Deploy to Azure](#deploy-to-azure)
- [Project Structure](#project-structure)
- [Contributing](#contributing)

---

## What are Azure Functions?

[Azure Functions](https://learn.microsoft.com/azure/azure-functions/functions-overview) is a serverless compute service that lets you run event-driven code without having to explicitly provision or manage infrastructure. You write small, focused pieces of code (called *functions*) that are triggered by events such as HTTP requests, timers, queue messages, or database changes.

Key characteristics:

| Feature | Description |
|---|---|
| **Serverless** | No server management required — Azure scales resources automatically |
| **Event-driven** | Functions are invoked by configurable triggers (HTTP, Timer, Queue, …) |
| **Pay-per-use** | You are billed only for the time your code runs |
| **Polyglot** | Supports C#, JavaScript/TypeScript, Python, Java, PowerShell, and more |
| **Integrated security** | Built-in support for Azure Active Directory, managed identities, and token validation |

This project uses **Azure Functions v4** with the [.NET isolated worker model](https://learn.microsoft.com/azure/azure-functions/dotnet-isolated-process-guide) targeting **.NET 8**.

---

## Project Overview

When an AI agent (e.g., a Copilot, AutoGen, or Semantic Kernel agent) calls a backend API on behalf of a user, it typically forwards an **On-Behalf-Of (OBO)** token or a delegated access token in the `Authorization` header.

The `AuthDebugFunction` in this repository acts as that backend API endpoint. It:

1. Accepts an anonymous `HTTP POST` request.
2. Reads all incoming request headers, method, path, and query string.
3. Extracts the `Bearer` token from the `Authorization` header (if present).
4. Base64-decodes and pretty-prints the JWT payload so you can inspect the claims.
5. Returns all of the above as a styled HTML page — ideal for debugging delegation flows.

---

## Architecture

```
AI Agent / Client
      │
      │  POST /api/HttpCallDetailsViewer
      │  Authorization: Bearer <JWT>
      ▼
Azure Function (HttpCallDetailsViewer)
      │
      │  1. Extract Authorization header
      │  2. Decode JWT payload (base64)
      │  3. Build HTML response
      ▼
HTML page with request details & decoded token claims
```

---

## Prerequisites

| Tool | Purpose | Install |
|---|---|---|
| [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) | Build & run the function | `winget install Microsoft.DotNet.SDK.8` |
| [Azure Functions Core Tools v4](https://learn.microsoft.com/azure/azure-functions/functions-run-local) | Run functions locally | `npm install -g azure-functions-core-tools@4` |
| [Azure CLI](https://learn.microsoft.com/cli/azure/install-azure-cli) *(optional)* | Deploy to Azure | `winget install Microsoft.AzureCLI` |
| [Visual Studio Code](https://code.visualstudio.com/) *(optional)* | IDE with Azure Functions extension | — |

Verify your tools are ready:

```bash
dotnet --version        # should show 8.x.x
func --version          # should show 4.x.x
```

---

## Getting Started

### Clone the repository

```bash
git clone https://github.com/abeckDev/agent-authentication-demo.git
cd agent-authentication-demo
```

> **Note:** If you forked the repository, replace the URL above with your own fork URL.

### Run locally

Navigate to the function project folder and start the Functions host:

```bash
cd AuthDebugFunction
func start --port 7178
```

Alternatively, use the .NET CLI (which also starts the Functions host via `launchSettings.json`):

```bash
cd AuthDebugFunction
dotnet run
```

Or open the solution in **Visual Studio** / **VS Code** and press **F5** — the `launchSettings.json` profile is pre-configured to use port `7178`.

Once started you should see output similar to:

```
Azure Functions Core Tools
Core Tools Version:       4.x.x ...
...
Functions:
    HttpCallDetailsViewer: [POST] http://localhost:7178/api/HttpCallDetailsViewer
```

### Test the endpoint

**Without a token** (inspect raw request details):

```bash
curl -X POST http://localhost:7178/api/HttpCallDetailsViewer
```

**With a bearer token** (inspect decoded JWT claims):

```bash
curl -X POST http://localhost:7178/api/HttpCallDetailsViewer \
     -H "Authorization: Bearer <your-jwt-token>"
```

Open the returned HTML in a browser or pipe to a file for a nicely formatted view:

```bash
curl -s -X POST http://localhost:7178/api/HttpCallDetailsViewer \
     -H "Authorization: Bearer <your-jwt-token>" \
     -o response.html && open response.html
```

---

## Deploy to Azure

1. **Login to Azure:**

   ```bash
   az login
   ```

2. **Create a Function App** (skip if you already have one):

   ```bash
   az group create --name rg-agent-auth-demo --location westeurope
   az storage account create --name <unique-storage-name> --resource-group rg-agent-auth-demo --sku Standard_LRS
   az functionapp create \
     --resource-group rg-agent-auth-demo \
     --consumption-plan-location westeurope \
     --runtime dotnet-isolated \
     --functions-version 4 \
     --name <unique-function-app-name> \
     --storage-account <unique-storage-name>
   ```

   > **Note:** Both `<unique-storage-name>` and `<unique-function-app-name>` must be globally unique across Azure. Choose names that include a distinctive suffix (e.g., your initials or a short random string).

3. **Publish the function:**

   ```bash
   cd AuthDebugFunction
   func azure functionapp publish <unique-function-app-name>
   ```

4. **Call the deployed endpoint:**

   ```bash
   curl -X POST https://<unique-function-app-name>.azurewebsites.net/api/HttpCallDetailsViewer \
        -H "Authorization: Bearer <your-jwt-token>"
   ```

---

## Project Structure

```
agent-authentication-demo/
├── Readme.md                          # This file
└── AuthDebugFunction/
    ├── HttpCallDetailsViewer.cs       # Azure Function implementation
    ├── Program.cs                     # .NET isolated worker entry point
    ├── agent-authentication-demo.csproj
    ├── agent-authentication-demo.sln
    ├── host.json                      # Functions host configuration
    ├── Properties/
    │   └── launchSettings.json        # Local run profile (port 7178)
    └── Readme.md                      # Function-specific documentation
```

---

## Contributing

Contributions are welcome! Please open an issue or submit a pull request.
 
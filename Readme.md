# Simple Agent Authentication Sample - Work in Progress

A demonstration project that shows how credential delegation works when an AI agent calls an Azure Function on behalf of a user. The function inspects the incoming HTTP request and surfaces all authentication details — including decoded JWT bearer tokens — as a human-readable HTML page.

> **Status:** Work in Progress


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
 

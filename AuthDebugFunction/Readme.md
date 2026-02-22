## AuthDebugFunction

An Azure Function that provides detailed information about an incoming HTTP request to demonstrate and debug credential delegation mechanisms used by AI agents.

### Overview

When an AI agent calls a backend API on behalf of a user, it forwards a delegated access token (e.g., an On-Behalf-Of / OBO token) in the `Authorization` header. This function makes it easy to inspect that token and all other request metadata.

The function:

1. Accepts an anonymous `HTTP POST` request on `/api/HttpCallDetailsViewer`.
2. Reads and displays all request headers, method, path, and query string.
3. Extracts the `Bearer` token from the `Authorization` header.
4. Base64-decodes the JWT payload and pretty-prints the claims as JSON.
5. Returns everything as a styled HTML page.

### Endpoint

| Property | Value |
|---|---|
| **Trigger** | HTTP |
| **Methods** | `POST` |
| **Route** | `/api/HttpCallDetailsViewer` |
| **Authorization level** | Anonymous (no function key required) |

### Returns

An HTML page containing:

- **Request information** – HTTP method, protocol, path, query string
- **Headers** – all request headers as `Key: Value` pairs
- **Bearer token** – the raw JWT string (if present in the `Authorization` header)
- **Decoded token** – the JWT payload decoded from Base64 and formatted as indented JSON

### Prerequisites

| Tool | Version |
|---|---|
| .NET SDK | 8.0+ |
| Azure Functions Core Tools | v4+ |

### Running Locally

From the `AuthDebugFunction` directory:

```bash
func start --port 7178
```

Or using the .NET CLI:

```bash
dotnet run
```

The function will be available at:

```
http://localhost:7178/api/HttpCallDetailsViewer
```

### Example Requests

**Basic request (no token):**

```bash
curl -X POST http://localhost:7178/api/HttpCallDetailsViewer
```

**Request with a bearer token:**

```bash
curl -X POST http://localhost:7178/api/HttpCallDetailsViewer \
     -H "Authorization: Bearer eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9..."
```

**Save the response as an HTML file and open it:**

```bash
curl -s -X POST http://localhost:7178/api/HttpCallDetailsViewer \
     -H "Authorization: Bearer <your-jwt>" \
     -o response.html && open response.html
```

### Example Response

The function returns an HTML page structured as follows:

```
HTTP Request Details

Request Information
  Method:      POST
  Protocol:    HTTP/1.1
  Path:        /api/HttpCallDetailsViewer
  QueryString: (empty)

Headers
  Content-Type: application/json
  Authorization: Bearer eyJ...

Bearer Token
  Token:    eyJhbGciOi...
  Decoded:
  {
    "aud": "api://my-app",
    "iss": "https://login.microsoftonline.com/<tenant-id>/v2.0",
    "sub": "user-object-id",
    "name": "Jane Doe",
    "roles": ["user"],
    ...
  }
```

### Configuration

| File | Purpose |
|---|---|
| `host.json` | Azure Functions host settings (logging, Application Insights sampling) |
| `Properties/launchSettings.json` | Local run profile — sets port `7178` |

### Deployment

See the [root README](../Readme.md#deploy-to-azure) for full deployment instructions.


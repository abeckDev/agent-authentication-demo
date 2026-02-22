## AuthDebugFunction

This Azure Function provides detailed information about the HTTP request to demonstrate credential delegation mechanisms.

### Overview
The function examines incoming HTTP requests and extracts credential-related information, including headers, authentication tokens, and other delegation details.

### Returns
An object containing:
- HTTP headers
- Authentication tokens
- Credential information
- Request context for delegation purposes

### Usage
Deploy this function to your Azure environment and send HTTP requests to inspect the credential delegation flow.

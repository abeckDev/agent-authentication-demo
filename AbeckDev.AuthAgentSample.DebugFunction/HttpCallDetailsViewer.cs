using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace AbeckDev.AuthAgentSample.DebugFunction;

public class HttpCallDetailsViewer
{
    private readonly ILogger<HttpCallDetailsViewer> _logger;

    public HttpCallDetailsViewer(ILogger<HttpCallDetailsViewer> logger)
    {
        _logger = logger;
    }

    [Function("HttpCallDetailsViewer")]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req)
    {

        string token = null;
        string decodedToken = null;

        // Extract bearer token from Authorization header
        var authHeader = req.Headers["Authorization"].ToString();
        if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
        {
            token = authHeader["Bearer ".Length..];
            try
            {
                var parts = token.Split('.');
                if (parts.Length == 3)
                {
                    // Decode the payload (second part)
                    var payload = parts[1];
                    // Add padding if needed
                    payload += new string('=', (4 - payload.Length % 4) % 4);
                    var base64Decoded = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(payload));
                    var json = System.Text.Json.JsonDocument.Parse(base64Decoded);
                    decodedToken = System.Text.Json.JsonSerializer.Serialize(json, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
                }
                else
                {
                    decodedToken = "Invalid JWT format";
                }
            }
            catch
            {
                decodedToken = "Failed to decode token";
            }
        }

        // Build HTML response
        var html = $@"
        <html>
        <head><style>body {{ font-family: Arial; margin: 20px; }} pre {{ background: #f4f4f4; padding: 10px; overflow-x: auto; }}</style></head>
        <body>
        <h1>HTTP Request Details</h1>
        <h2>Request Information</h2>
        <p><strong>Method:</strong> {req.Method}</p>
        <p><strong>Protocol:</strong> {req.Protocol}</p>
        <p><strong>Path:</strong> {req.Path}</p>
        <p><strong>QueryString:</strong> {req.QueryString}</p>
        <h2>Headers</h2>
        <pre>{string.Join("\n", req.Headers.Select(h => $"{h.Key}: {h.Value}"))}</pre>
        <h2>Bearer Token</h2>
        <p><strong>Token:</strong> {(token ?? "Not provided")}</p>
        <p><strong>Decoded:</strong></p>
        <pre>{(decodedToken ?? "N/A")}</pre>
        </body>
        </html>";

        _logger.LogInformation("Received request. Details: {details}", html);

        return new ContentResult { Content = html, ContentType = "text/html" };

    }
}
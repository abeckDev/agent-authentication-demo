using System;
using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;
using OpenAI.Chat;
using System.ComponentModel;
using Microsoft.Extensions.AI;

// Acquire token using existing Azure CLI session - This simulates token obtainment -> Should be more sophisticated in production (OAuth2 Flow)
var credential = new AzureCliCredential();
var scopes = new[] { Environment.GetEnvironmentVariable("API_SCOPE") ?? "https://management.azure.com/.default" };

var tokenRequestContext = new Azure.Core.TokenRequestContext(scopes);
var token = await credential.GetTokenAsync(tokenRequestContext);

var accessToken = token.Token;
var tokenResult = new { Account = new { Username = "Azure CLI User" }, ExpiresOn = token.ExpiresOn };
Console.WriteLine($"Token acquired for: {tokenResult.Account?.Username}");
Console.WriteLine($"Expires: {tokenResult.ExpiresOn}");


//Define a Tool. Will use the users token from above
[Description("Does the thing. The user will know.")]
async Task<string> DoTheThing()
{
    using var httpClient = new HttpClient();
    httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
    var response = await httpClient.PostAsync("http://localhost:7071/api/HttpCallDetailsViewer", new StringContent("{\"message\": \"I am here to do the thing.\"}", System.Text.Encoding.UTF8, "application/json"));

    return await response.Content.ReadAsStringAsync();
}

//Define the Foundry Config
var endpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT")
    ?? throw new InvalidOperationException("Set AZURE_OPENAI_ENDPOINT");
var deploymentName = Environment.GetEnvironmentVariable("AZURE_OPENAI_DEPLOYMENT_NAME") ?? "gpt-4o-mini";

//Define the Agent
AIAgent agent = new AzureOpenAIClient(new Uri(endpoint), new AzureCliCredential())
    .GetChatClient(deploymentName)
    .AsAIAgent(instructions: "You are a friendly assistant.",tools:[AIFunctionFactory.Create(DoTheThing)], name: "HelloAgent");

//Execute the Agent and see the result. Token Details will be in Debug Log of Functions.
Console.WriteLine(await agent.RunAsync("Would you please do the thing?"));
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace FgcGames.IntegrationTests.TestHelpers;

public static class HttpResponseExtensions
{
    public static async Task<T?> ReadContentAsync<T>(this HttpResponseMessage response, CancellationToken cancellationToken) 
        => await response.Content.ReadFromJsonAsync<T>(cancellationToken);

    public static async Task<ProblemDetails> ReadProblemDetailsAsync(this HttpResponseMessage response, CancellationToken cancellationToken)
        => (await response.Content.ReadFromJsonAsync<ProblemDetails>(cancellationToken))!;

    public static async Task<Dictionary<string, string[]>> ReadValidationErrorsAsync(this HttpResponseMessage response, CancellationToken cancellationToken)
    {
        var problem = await response.ReadProblemDetailsAsync(cancellationToken);

        if (problem?.Extensions.TryGetValue("errors", out var errorsObj) == true && errorsObj is not null)
        {
            var element = (JsonElement)errorsObj;
            return JsonSerializer.Deserialize<Dictionary<string, string[]>>(element.GetRawText()) ?? [];
        }

        return [];
    }
}

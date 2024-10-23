using static System.Net.HttpStatusCode;

namespace Header.Propagation.Repro;

/// <summary>Operates on widgets by means of an HTTP client.</summary>
sealed class HttpWidgetClient(HttpClient httpClient)
    : IWidgetClient
{
    /// <inheritdoc/>
    async ValueTask<Widget?> IWidgetClient.GetWidgetAsync(int id, CancellationToken cancellationToken)
    {
        try
        {
            using var resp = await httpClient.GetAsync($"https://example.invalid/widgets/{id}", ResponseHeadersRead, cancellationToken);
            return resp switch
            {
                { StatusCode: OK, Content: { } c } => await c.ReadFromJsonAsync(WebContext.Default.Widget, cancellationToken),
                { StatusCode: NotFound } => null,
                _ => throw new Exception("I didn't expect you to get this far outside of the debugger, sorry!"),
            };
        }
        catch (HttpRequestException hre) when (hre.StatusCode is null)
        {
            throw new Exception("I didn't expect you to get this far outside of the debugger, sorry!", hre);
        }
    }
}

using WidgetResult = Microsoft.AspNetCore.Http.HttpResults.Results<Microsoft.AspNetCore.Http.HttpResults.Ok<Header.Propagation.Repro.IWidgetClient.Widget>, Microsoft.AspNetCore.Http.HttpResults.NotFound>;

var builder = WebApplication.CreateSlimBuilder(args);

_ = builder.Services
    .ConfigureHttpJsonOptions(static o => o.SerializerOptions.TypeInfoResolverChain.Insert(0, WebContext.Default))
    .AddHeaderPropagation(static o => o.Headers.Add(ETag));

_ = builder.Services.AddHttpClient<HttpWidgetClient>()
    .AddHeaderPropagation(static o => o.Headers.Add(ETag));
_ = builder.Services.AddTransient<IWidgetClient, CachingWidgetClient>();
_ = builder.Services.AddHybridCache()
    .AddSerializer<Widget, CachingWidgetClient.Serializer>();

await using var app = builder.Build();
_ = app.UseHeaderPropagation();
_ = app.MapGet("/widgets/{id}", static async ValueTask<WidgetResult> (int id, IWidgetClient client, CancellationToken ct) =>
{
    var widget = await client.GetWidgetAsync(id, ct);
    return widget is { } w
        ? TypedResults.Ok(w)
        : TypedResults.NotFound();
});
await app.RunAsync();

/// <summary>The JSON serializer context for the application.</summary>
[JsonSourceGenerationOptions(Web)]
[JsonSerializable(typeof(Widget))]
sealed partial class WebContext
    : JsonSerializerContext;

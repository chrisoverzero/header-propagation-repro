namespace Header.Propagation.Repro;

/// <summary>Operates on widgets with caching.</summary>
sealed partial class CachingWidgetClient(HttpWidgetClient innerClient, HybridCache cache, ILogger<CachingWidgetClient> logger)
    : IWidgetClient
{
    readonly IWidgetClient _innerClient = innerClient;

    /// <inheritdoc/>
    async ValueTask<Widget?> IWidgetClient.GetWidgetAsync(int id, CancellationToken cancellationToken)
    {
        GettingWidget(id);

        // note(cosborn) This call will succeed. (Well, -ish. It'll make the request; I hope that's enough.)
        _ = await _innerClient.GetWidgetAsync(id, cancellationToken);
        return await cache.GetOrCreateAsync(
            id.ToString(CI.InvariantCulture),
            ct => _innerClient.GetWidgetAsync(id, ct), // note(cosborn) This call will fail.
            /*
             * …and this one, too.
             * async ct => await _innerClient.GetWidgetAsync(id, ct),
             */
            new()
            {
                Expiration = TimeSpan.FromSeconds(10),
            },
            cancellationToken: cancellationToken);
    }

    [LoggerMessage(LogLevel.Debug, "About to try to get widget {id}.")]
    partial void GettingWidget(int id);

    /// <summary>Serializes and deserializes fulfillment locations for caching.</summary>
    public sealed class Serializer
        : IHybridCacheSerializer<Widget>
    {
        /// <inheritdoc/>
        Widget IHybridCacheSerializer<Widget>.Deserialize(ReadOnlySequence<byte> source)
        {
            var reader = new Utf8JsonReader(source);
            return JsonSerializer.Deserialize(ref reader, CacheContext.Default.Widget)
                ?? throw new Exception("Failed to deserialize!");
        }

        /// <inheritdoc/>
        void IHybridCacheSerializer<Widget>.Serialize(Widget value, IBufferWriter<byte> target)
        {
            var writer = new Utf8JsonWriter(target);
            JsonSerializer.Serialize(writer, value, CacheContext.Default.Widget);
        }
    }

    [JsonSourceGenerationOptions(
        PropertyNamingPolicy = CamelCase,
        RespectNullableAnnotations = true,
        RespectRequiredConstructorParameters = true)]
    [JsonSerializable(typeof(Widget))]
    sealed partial class CacheContext
        : JsonSerializerContext;
}

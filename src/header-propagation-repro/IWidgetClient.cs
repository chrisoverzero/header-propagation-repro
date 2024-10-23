namespace Header.Propagation.Repro;

/// <summary>Operates on widgets.</summary>
public interface IWidgetClient
{
    /// <summary>Gets a widget by its unique identifier.</summary>
    /// <param name="id">The unique identifier of the widget.</param>
    /// <param name="cancellationToken">A token to watch for operation cancellation.</param>
    /// <returns>
    /// A <see cref="ValueTask{TResult}"/> that represents the asynchronous operation.
    /// The task result contains the widget if a widget with the specified identifier exists;
    /// otherwise, <see langword="null"/>.
    /// </returns>
    public ValueTask<Widget?> GetWidgetAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>An example widget.</summary>
    /// <param name="Id">The unique identifier of the widget.</param>
    /// <param name="Description">A description of the widget.</param>
    public sealed record class Widget(int Id, string Description);
}

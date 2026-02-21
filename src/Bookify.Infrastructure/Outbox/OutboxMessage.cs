namespace Bookify.Infrastructure.Outbox;
public sealed class OutboxMessage
{
    public OutboxMessage(Guid id, DateTime occurredOnUtc, string type, string content)
    {
        Id = id;
        OccurredOnUtc = occurredOnUtc;
        Content = content;
        Type = type;
    }

    public Guid Id { get; init; }

    public DateTime OccurredOnUtc { get; init; }

    //full qualified name of the event, e.g. Bookify.Application.Bookings.ReserveBooking.ReserveBookingEvent, serialized
    public string Type { get; init; }

    //json string - represent main domain instance
    public string Content { get; init; }

    public DateTime? ProcessedOnUtc { get; init; }

    public string? Error { get; init; }
}
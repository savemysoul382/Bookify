// Bookify.Domain

using Bookify.Domain.Apartments;

namespace Bookify.Domain.Bookings;

public interface IBookingRepository
{
    Task<Booking?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// If somebody tries to book an apartment for a date range that overlaps with an existing booking, we should prevent it.
    /// </summary>
    /// <param name="apartment"></param>
    /// <param name="duration"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> IsOverlappingAsync(
        Apartment apartment,
        DateRange duration,
        CancellationToken cancellationToken = default);

    void Add(Booking booking);
}
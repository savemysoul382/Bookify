// Bookify.Application

using Bookify.Application.Abstractions.Messaging;

namespace Bookify.Application.Apartments;

public record SearchApartmentsQuery(DateOnly StartDate, DateOnly EndDate) 
    : IQuery<IReadOnlyList<ApartmentResponse>>;
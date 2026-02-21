using Bookify.Application.Bookings.GetBooking;
using Bookify.Application.Bookings.ReserveBooking;
using Bookify.Domain.Abstractions;
using MediatR;

namespace Bookify.Api.Controllers.Bookings
{
    public static class BookingsEndpoints
    {
        public static IEndpointRouteBuilder MapBookingEndpoints(this IEndpointRouteBuilder builder)
        {
            builder.MapGet("bookings/{id}", GetBooking)
                .RequireAuthorization() // RequireAuthorization("bookings:read")?
                .WithName(nameof(GetBooking));

            builder.MapPost("bookings", ReserveBooking)
                .RequireAuthorization(); // RequireAuthorization("bookings:write")?
            return builder;
        }


        public static async Task<IResult> GetBooking(Guid id, ISender sender, CancellationToken cancellationToken)
        {
            GetBookingQuery query = new GetBookingQuery(id);

            Result<BookingResponse> result = await sender.Send(query, cancellationToken);
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.NotFound();
        }

        public static async Task<IResult> ReserveBooking(
            ReserveBookingRequest request,
            ISender sender,
            CancellationToken cancellationToken)
        {
            ReserveBookingCommand command = new ReserveBookingCommand(
                request.ApartmentId,
                request.UserId,
                request.StartDate,
                request.EndDate);

            Result<Guid> result = await sender.Send(command, cancellationToken);

            if (result.IsFailure)
            {
                return Results.BadRequest(result.Error);
            }

            //return CreatedAtAction(nameof(GetBooking), new {id = result.Value}, result.Value); this for Controller
            return Results.CreatedAtRoute(nameof(GetBooking), new {id = result.Value}, result.Value);
        }
    }
}
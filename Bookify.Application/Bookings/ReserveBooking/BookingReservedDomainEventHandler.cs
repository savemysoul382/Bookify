// Bookify.Application

using Bookify.Application.Abstractions.Email;
using Bookify.Domain.Bookings;
using Bookify.Domain.Bookings.Events;
using Bookify.Domain.Users;
using MediatR;

namespace Bookify.Application.Bookings.ReserveBooking;

internal sealed class BookingReservedDomainEventHandler : INotificationHandler<BookingReservedDomainEvent>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IUserRepository _userRepository;
    private readonly IEmailService _emailService;


    public BookingReservedDomainEventHandler(IBookingRepository bookingRepository, IUserRepository userRepository, IEmailService emailService)
    {
        this._bookingRepository = bookingRepository;
        this._userRepository = userRepository;
        this._emailService = emailService;
    }

    public async Task Handle(BookingReservedDomainEvent notification, CancellationToken cancellationToken)
    {
        Booking? booking = await this._bookingRepository.GetByIdAsync(notification.BookingId, cancellationToken);

        if (booking is null)
        {
            return; 
        }

        User? user = await this._userRepository.GetByIdAsync(booking.UserId, cancellationToken);
        if (user is null)
        {
            return;
        }

        await this._emailService.SendAsync(user.Email, "Booking Reserved", $"You have 10 minutes to confirm this booking.");   
    }
}
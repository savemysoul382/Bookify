// Bookify.Application

using FluentValidation;

namespace Bookify.Application.Bookings.ReserveBooking;

public class ReserveBookingCommandValidator : AbstractValidator<ReserveBookingCommand>
{
    public ReserveBookingCommandValidator()
    {
        RuleFor(command => command.ApartmentId)
            .NotEmpty()
            .WithMessage("Apartment ID is required.");

        RuleFor(command => command.UserId)
            .NotEmpty()
            .WithMessage("User ID is required.");

        RuleFor(command => command.StartDate)
            .NotEmpty()
            .WithMessage("Start date is required.")
            .LessThan(command => command.EndDate)
            .WithMessage("Start date must be in the future.");

        RuleFor(command => command.EndDate)
            .NotEmpty()
            .WithMessage("End date is required.")
            .GreaterThan(command => command.StartDate)
            .WithMessage("End date must be after start date.");
    }
}
using Bookify.Application.Apartments;
using Bookify.Application.IntegrationTests.Infrastructure;
using Bookify.Domain.Abstractions;
using FluentAssertions;

namespace Bookify.Application.IntegrationTests.Apartments;

public class SearchApartmentsTests : BaseIntegrationTest
{
    public SearchApartmentsTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task SearchApartments_ShouldReturnEmptyList_WhenDateRangeInvalid()
    {
        // Arrange
        SearchApartmentsQuery query = new SearchApartmentsQuery(
            new DateOnly(
                2026, 1, 10),
            new DateOnly(
                2026, 1, 1));

        // Act
        Result<IReadOnlyList<ApartmentResponse>> result = await this.Sender.Send(query);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task SearchApartments_ShouldReturnApartments_WhenDateRangeIsValid()
    {
        // Arrange
        SearchApartmentsQuery query = new SearchApartmentsQuery(
            new DateOnly(2026, 1, 1),
            new DateOnly(2026, 1, 10));

        // Act
        Result<IReadOnlyList<ApartmentResponse>> result = await this.Sender.Send(query);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
    }
}
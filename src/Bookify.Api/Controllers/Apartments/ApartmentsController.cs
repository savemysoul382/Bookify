using Asp.Versioning;
using Bookify.Application.Apartments;
using Bookify.Domain.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bookify.Api.Controllers.Apartments;

[Authorize]
[ApiController]
[ApiVersion(ApiVersions.V1)]
[Route(template: "api/v{version:apiVersion}/apartments")]
public class ApartmentsController : ControllerBase
{
    private readonly ISender _sender;

    public ApartmentsController(ISender sender)
    {
        this._sender = sender;
    }

    [HttpGet]
    public async Task<IActionResult> SearchApartments(DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken)
    {
        SearchApartmentsQuery query = new SearchApartmentsQuery(startDate, endDate);

        Result<IReadOnlyList<ApartmentResponse>> result = await this._sender.Send(query, cancellationToken);

        return this.Ok(result.Value);
    }
}
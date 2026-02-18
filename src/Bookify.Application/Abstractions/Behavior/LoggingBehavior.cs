// Bookify.Application

using Bookify.Application.Abstractions.Messaging;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Bookify.Application.Abstractions.Behavior;

public class LoggingBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IBaseCommand
{
    private readonly ILogger<TRequest> _logger;

    public LoggingBehavior(ILogger<TRequest> logger)
    {
        this._logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        string name = request.GetType().Name;

        try
        {
            this._logger.LogInformation("Processing command {Name}", name);

            var response = await next();

            this._logger.LogInformation("Command {Name} processed successfully", name);

            return response;
        }
        catch (Exception e)
        {
            this._logger.LogError(e, "Command {Name} processing failed", name);
            throw;
        }
    }
}
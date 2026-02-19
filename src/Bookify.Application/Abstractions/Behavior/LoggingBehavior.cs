// Bookify.Application

using Bookify.Domain.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;
using Serilog.Context;

#pragma warning disable CA1873

namespace Bookify.Application.Abstractions.Behavior;

public class LoggingBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IBaseRequest // IBaseCommand
    where TResponse : Result
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        this._logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        string name = request.GetType().Name;

        try
        {
            this._logger.LogInformation("Executing request {Name}", name);

            var result = await next();

            if (result.IsSuccess)
            {
                this._logger.LogInformation("Request {Name} processed successfully", name);
            }
            else
            {
                // V1
                // this._logger.LogError("Request {Name} processed with {@Error}", name, result.Error); // {@Error} - @ Serializes the object to Json.

                // Prefer way
                using (LogContext.PushProperty("Error", result.Error, true))
                {
                    this._logger.LogError("Request {Name} processed with error", name);
                }
                
            }

            this._logger.LogInformation("Request {Name} processed successfully", name);


            return result;
        }
        catch (Exception e)
        {
            this._logger.LogError(e, "Request {Name} processing failed", name);
            throw;
        }
    }
}
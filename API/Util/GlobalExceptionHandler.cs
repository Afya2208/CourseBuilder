using System.Security.Authentication;
using Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog.Context;

namespace API.Util
{
    public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken ct)
        {
            logger.LogError(exception, "{ExceptionType} {Message} | {Method} {Path}",
                exception.GetType().Name, exception.Message, context.Request.Path, context.Request.Method);
            using var a = LogContext.PushProperty("HandledException", true);
            ProblemDetails problemDetails = CreateProblemDetails(exception);
            problemDetails.Instance = context.Request.Path;
            context.Response.StatusCode = problemDetails.Status ?? StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(problemDetails, ct);
            return true;
        }
        public ProblemDetails CreateProblemDetails(Exception exception)
        {
            var title = "Error in the server";
            string details = null;
            var statusCode = StatusCodes.Status500InternalServerError;
            switch (exception)
            {
                case AuthenticationException:
                    title = exception.Message;
                    statusCode = StatusCodes.Status401Unauthorized;
                    break;
                case InvalidCastException:
                case ArgumentException:
                    title = exception.Message;
                    statusCode = StatusCodes.Status400BadRequest;
                    break;
                case NotFoundException:
                    title = exception.Message;
                    statusCode = StatusCodes.Status404NotFound;
                    break;
                case NullReferenceException:
                    title = exception.Message;
                    statusCode = StatusCodes.Status400BadRequest;
                    break;
                case DbUpdateException:
                title = "Ошибка в базе данных: нельзя удалить используемые данные";
                statusCode = StatusCodes.Status400BadRequest;
                break;
            }

            ProblemDetails problemDetails = new ProblemDetails()
            {
                Title = title,
                Detail = details,
                Status = statusCode, 
            };
            return problemDetails;
        }
    }
}
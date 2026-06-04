using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Threading.Tasks;
using API.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog.Context;

namespace API.Service
{
    public class GeneralExceptionHandler(ILogger<GeneralExceptionHandler> logger) : IExceptionHandler
    {
        public ProblemDetails CreateDetails(Exception exception)
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
        public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
        {
            logger.LogError(exception, "{ExceptionType} - {Message} | Path:{Path} | Method:{Method}",
            exception.GetType().Name, exception.Message, context.Request.Path, context.Request.Method);
            using var a = LogContext.PushProperty("HandledException", true);
            var problemDetails = CreateDetails(exception);
            problemDetails.Instance = context.Request.Path;
            context.Response.StatusCode = problemDetails.Status ?? StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(problemDetails);
            return true;
        }
    }
}
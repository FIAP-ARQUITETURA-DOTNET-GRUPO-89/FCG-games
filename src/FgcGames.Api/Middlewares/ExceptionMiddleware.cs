using FgcGames.Shared.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace FgcGames.Api.Middlewares;

public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<ExceptionMiddleware> _logger = logger;

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException ex)
        {
            var errors = ex.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).ToArray()
                );

            await WriteProblemDetails(context, StatusCodes.Status400BadRequest, errors);
        }
        catch (BusinessException ex)
        {
            _logger.LogWarning(ex, "Erro de negócio na requisição {Method} {Path}", context.Request.Method, context.Request.Path);

            var statusCode = ex switch
            {
                AlreadyExistsException => StatusCodes.Status409Conflict,
                NotFoundException => StatusCodes.Status404NotFound,
                _ => StatusCodes.Status400BadRequest
            };

            await WriteProblemDetails(context, statusCode, ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            var path = context.Request.Path;
            var method = context.Request.Method;
            var user = context.User?.Identity?.Name ?? "anonymous";

            _logger.LogWarning(ex, "Acesso não autorizado. User: {User} | {Method} {Path}", user, method, path);

            var statusCode = context.User?.Identity?.IsAuthenticated == true
                ? StatusCodes.Status403Forbidden   
                : StatusCodes.Status401Unauthorized; 

            if (statusCode == StatusCodes.Status401Unauthorized)
            {
                context.Response.Headers.WWWAuthenticate = "Bearer";
            }

            await WriteProblemDetails(
                context,
                statusCode,
                statusCode == 401
                    ? "Não autenticado. Faça login para acessar este recurso."
                    : "Você não tem permissão para acessar este recurso."
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado na requisição {Method} {Path}", context.Request.Method, context.Request.Path);

            await WriteProblemDetails(context, StatusCodes.Status500InternalServerError, "Ocorreu um erro inesperado. Tente novamente mais tarde!");
        }
    }

    private static async Task WriteProblemDetails(
        HttpContext context,
        int statusCode,
        object? details)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/problem+json";

        var problem = new ProblemDetails
        {
            Title = GetTitle(statusCode),
            Status = statusCode
        };

        if (details is string str)
        {
            problem.Detail = str;
        }
        else if (details is not null)
        {
            problem.Extensions["errors"] = details;
        }

        await context.Response.WriteAsJsonAsync(problem);
    }

    private static string GetTitle(int statusCode) => statusCode switch
    {
        StatusCodes.Status400BadRequest => "Bad Request",
        StatusCodes.Status401Unauthorized => "Unauthorized",
        StatusCodes.Status403Forbidden => "Forbidden",
        StatusCodes.Status404NotFound => "Not Found",
        StatusCodes.Status409Conflict => "Conflict",
        StatusCodes.Status500InternalServerError => "Internal Server Error",
        _ => "Error"
    };
}

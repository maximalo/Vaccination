using System.Net;
using System.Text.Json;
using Vaccination.Api.Exceptions;
using Vaccination.Application.Dtos;
using KeyNotFoundException = Vaccination.Api.Exceptions.KeyNotFoundException;
using NotImplementedException = Vaccination.Api.Exceptions.NotImplementedException;
using UnauthorizedAccessException = Vaccination.Api.Exceptions.UnauthorizedAccessException;

namespace Vaccination.Api.Middlewares
{
    public class GlobalErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public GlobalErrorHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            HttpStatusCode status;
            string message;

            var exceptionType = exception.GetType();

            if (exceptionType == typeof(BadRequestException))
            {
                message = exception.Message;
                status = HttpStatusCode.BadRequest;
            }
            else if (exceptionType == typeof(NotFoundException))
            {
                message = exception.Message;
                status = HttpStatusCode.NotFound;
            }
            else if (exceptionType == typeof(NotImplementedException))
            {
                status = HttpStatusCode.NotImplemented;
                message = exception.Message;
            }
            else if (exceptionType == typeof(UnauthorizedAccessException))
            {
                status = HttpStatusCode.Unauthorized;
                message = exception.Message;
            }
            else if (exceptionType == typeof(SecurityTokenException))
            {
                status = HttpStatusCode.Unauthorized;
                message = exception.Message;
            }
            else if (exceptionType == typeof(KeyNotFoundException))
            {
                status = HttpStatusCode.NotFound;
                message = exception.Message;
            }
            else if (exceptionType == typeof(InvalidOperationException))
            {
                status = HttpStatusCode.Conflict;
                message = exception.Message;
            }
            else if (exceptionType == typeof(FluentValidation.ValidationException))
            {
                //TODO - Add FluentValidation exception handling
                status = HttpStatusCode.BadRequest;

                FluentValidation.ValidationException validationException = (FluentValidation.ValidationException)exception;
                message = JsonSerializer.Serialize(validationException.Errors.Select(x => new { x.PropertyName, x.ErrorMessage }));
            }
            else
            {
                status = HttpStatusCode.InternalServerError;
                message = exception.Message;
            }

            string exceptionResult = JsonSerializer.Serialize(new ApiResponse<string>() { Message = message, Success = false });
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)status;

            return context.Response.WriteAsync(exceptionResult);
        }
    }
}
using API.Errors;
using API.Interfaces;
using System.Net;
using System.Text.Json;

namespace API.Middleware
{
    public class ExceptionMiddleWare
    {
        private readonly RequestDelegate _next;       
        private readonly IHostEnvironment _env;

        public ExceptionMiddleWare(RequestDelegate next, IHostEnvironment env)
        {
            _next = next;          
            _env = env;           
        }
        
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                // Add exception to ApiException table
                var exception = new Entities.ApiException();
                exception.StatusCode = 500;
                exception.Message = ex.Message;
                exception.Details = ex.StackTrace?.ToString();
                var apiExceptionRepository = context.RequestServices.GetService<IApiExceptionRepository>();
                apiExceptionRepository.AddException(exception);
                await apiExceptionRepository.SaveAllAsync();
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                var response = _env.IsDevelopment() ? new ApiException(context.Response.StatusCode,
                    ex.Message, ex.StackTrace?.ToString()) : new ApiException(context.Response.StatusCode, "Internal Server Error");
                var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                var json = JsonSerializer.Serialize(response, options);
                await context.Response.WriteAsync(json);
            }
        }
    }
}

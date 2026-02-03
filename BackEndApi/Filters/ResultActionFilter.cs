using BackEnd.Application.Common.ResponseFormat;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;
using IResult = BackEnd.Application.Common.ResponseFormat.IResult;

namespace BackEnd.Api.Filters
{
    public sealed class ResultActionFilter : IAsyncResultFilter
    {
        public async Task OnResultExecutionAsync(
            ResultExecutingContext context,
            ResultExecutionDelegate next)
        {
            if (context.Result is ObjectResult objectResult &&
                objectResult.Value is IResult result)
            {
                context.Result = CreateActionResult(result);
            }

            await next();
        }

        private IActionResult CreateActionResult(IResult result)
        {
            if (result.IsSuccess)
            {
                return new OkObjectResult(
                    ApiResponse<object>.Success(result.Value, HttpStatusCode.OK)
                );
            }

            var statusCode = result.ErrorType switch
            {
                ErrorType.NotFound => HttpStatusCode.NotFound,
                ErrorType.BadRequest => HttpStatusCode.BadRequest,
                ErrorType.Conflict => HttpStatusCode.Conflict,
                ErrorType.UnprocessableEntity => HttpStatusCode.UnprocessableEntity,
                ErrorType.Unauthorized => HttpStatusCode.Unauthorized,
                ErrorType.Forbidden => HttpStatusCode.Forbidden,
                _ => HttpStatusCode.InternalServerError
            };

            return new ObjectResult(
                ApiResponse<object>.Fail(result.Message, statusCode))
            {
                StatusCode = (int)statusCode
            };
        }
    }
}

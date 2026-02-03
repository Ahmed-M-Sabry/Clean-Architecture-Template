using BackEnd.Application.Common.ResponseFormat;
using FluentValidation;
using MediatR;

namespace BackEnd.Application.Behaviors
{
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
        where TResponse : class
    {
        private readonly IValidator<TRequest>? _validator;

        public ValidationBehavior(IValidator<TRequest>? validator = null)
        {
            _validator = validator;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            if (_validator != null)
            {
                var validationResult = await _validator.ValidateAsync(request, cancellationToken);

                if (!validationResult.IsValid)
                {
                    var errors = string.Join(" | ", validationResult.Errors.Select(e => e.ErrorMessage));

                    var responseType = typeof(TResponse);
                    if (responseType.IsGenericType &&
                        responseType.GetGenericTypeDefinition() == typeof(Result<>))
                    {
                        var innerType = responseType.GetGenericArguments()[0];
                        var failureMethod = typeof(Result<>)
                            .MakeGenericType(innerType)
                            .GetMethod("Failure", new[] { typeof(string), typeof(ErrorType) });

                        if (failureMethod != null)
                        {
                            var failureResult = failureMethod.Invoke(null, new object[] { errors, ErrorType.BadRequest });
                            return (TResponse)failureResult!;
                        }
                    }

                    throw new InvalidOperationException($"Validation failed for {typeof(TRequest).Name} but could not construct a valid Result<T> response.");
                }
            }

            return await next();
        }
    }
}

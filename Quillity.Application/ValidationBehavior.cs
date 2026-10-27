using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace Quillity.Application;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
            return await next(cancellationToken);

        var context = new ValidationContext<TRequest>(request);
        var failures = _validators
            .Select(validator => validator.Validate(context))
            .Select(result => result.Errors)
            .Where(failure => failure != null)
            .ToList();

        if (failures.Count != 0)
            throw new ValidationException((IEnumerable<ValidationFailure>)failures);

        return await next(cancellationToken);
    }
}

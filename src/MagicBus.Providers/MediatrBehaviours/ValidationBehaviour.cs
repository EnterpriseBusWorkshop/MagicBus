using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using FluentValidation;
using FluentValidation.Results;

namespace MagicBus.Providers.MediatrBehaviours
{
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {

        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var context = new ValidationContext(request);

            // start validation tasks
            var failures = new List<ValidationFailure>();
            foreach (var validator in _validators)
            {
                // we inject dbcontext into async validators - which does not like running from multiple concurrent threads.
                // we use await to run them in order, one at a time
                var validationResult = await validator.ValidateAsync(context, cancellationToken);
                if (!validationResult.IsValid) failures.AddRange(validationResult.Errors);
            }

            if (failures.Count != 0)
            {
                throw new ValidationException(failures);
            }

            return await next();
        }
    }
}
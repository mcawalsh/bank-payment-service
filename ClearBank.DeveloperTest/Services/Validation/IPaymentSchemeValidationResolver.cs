using ClearBank.DeveloperTest.Types;
using System.Collections.Generic;
using System.Linq;

namespace ClearBank.DeveloperTest.Services.Validation;

public interface IPaymentSchemeValidationResolver
{
    /// <summary>
    /// Gets the validator for the specified payment scheme.
    /// </summary>
    /// <param name="paymentScheme">The payment scheme for which to get the validator.</param>
    /// <returns>The validator for the specified payment scheme.</returns>
    IPaymentSchemeValidator GetValidator(PaymentScheme paymentScheme);
}

public class PaymentSchemeValidationResolver : IPaymentSchemeValidationResolver
{
    private readonly IReadOnlyDictionary<PaymentScheme, IPaymentSchemeValidator> _validators;

    public PaymentSchemeValidationResolver(IEnumerable<IPaymentSchemeValidator> validators)
    {
        _validators = validators.ToDictionary(v => v.PaymentScheme, v => v);
    }

    public IPaymentSchemeValidator GetValidator(PaymentScheme paymentScheme)
        => _validators.TryGetValue(paymentScheme, out var validator)
            ? validator
            : null;
}

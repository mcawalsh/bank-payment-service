using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Services.Validation;

public interface IPaymentSchemeValidator
{
    PaymentScheme PaymentScheme { get; }

    /// <summary>
    /// Validates the payment request against the specified account.
    /// </summary>
    /// <param name="account">The account to validate against.</param>
    /// <param name="paymentRequest">The payment request to validate.</param>
    /// <returns>True if the payment request is valid; otherwise, false.</returns>
    ValidationResult Validate(Account? account, MakePaymentRequest paymentRequest);
}

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
    /// <returns>A ValidationResult indicating whether the validation was successful and any error messages if it was not.</returns>
    ValidationResult Validate(Account? account, MakePaymentRequest paymentRequest);
}

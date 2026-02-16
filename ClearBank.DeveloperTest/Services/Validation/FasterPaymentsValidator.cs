using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Services.Validation;

public class FasterPaymentsValidator : IPaymentSchemeValidator
{
    public PaymentScheme PaymentScheme => PaymentScheme.FasterPayments;
    public ValidationResult Validate(Account? account, MakePaymentRequest paymentRequest)
    {
        if (account == null)
            return ValidationResult.Failure("Account cannot be null.");

        if (!account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.FasterPayments))
            return ValidationResult.Failure("Account does not allow Faster Payments.");

        if (account.Balance < paymentRequest.Amount)
            return ValidationResult.Failure("Insufficient funds for Faster Payments.");

        return ValidationResult.Success();
    }
}
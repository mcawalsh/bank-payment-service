using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Services.Validation;

public class BacsPaymentValidator : IPaymentSchemeValidator
{
    public PaymentScheme PaymentScheme => PaymentScheme.Bacs;

    public ValidationResult Validate(Account? account, MakePaymentRequest paymentRequest)
    {
        if (account == null)
            return ValidationResult.Failure("Account cannot be null.");

        if (!account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.Bacs))
            return ValidationResult.Failure("Account does not allow Bacs payments.");

        return ValidationResult.Success();
    }
}
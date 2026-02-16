using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Services.Validation;

public class ChapsPaymentValidator : IPaymentSchemeValidator
{
    public PaymentScheme PaymentScheme => PaymentScheme.Chaps;
    public ValidationResult Validate(Account? account, MakePaymentRequest paymentRequest)
    {
        if (account == null)
            return ValidationResult.Failure("Account cannot be null.");

        if (!account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.Chaps))
            return ValidationResult.Failure("Account does not allow Chaps payments.");

        if (account.Status != AccountStatus.Live)
            return ValidationResult.Failure("Account status must be Live for Chaps payments.");

        return ValidationResult.Success();
    }
}
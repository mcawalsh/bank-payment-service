namespace ClearBank.DeveloperTest.Services.Validation;

public class ValidationResult
{
    public bool IsValid { get; }
    public string FailureReason { get; }

    private ValidationResult(bool isValid, string failureReason)
    {
        IsValid = isValid;
        FailureReason = failureReason;
    }

    public static ValidationResult Success() => new ValidationResult(true, string.Empty);
    public static ValidationResult Failure(string reason) => new ValidationResult(false, reason);
}
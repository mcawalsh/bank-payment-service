using ClearBank.DeveloperTest.Services.Validation;
using ClearBank.DeveloperTest.Types;
using System;

namespace ClearBank.DeveloperTest.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IDataStoreProvider _dataStoreProvider;
        private readonly IPaymentSchemeValidationResolver _validationResolver;

        public PaymentService(IDataStoreProvider dataStoreProvider, IPaymentSchemeValidationResolver paymentSchemeValidationResolver)
        {
            _dataStoreProvider = dataStoreProvider;
            _validationResolver = paymentSchemeValidationResolver;
        }

        public MakePaymentResult MakePayment(MakePaymentRequest request)
        {
            if (request is null)
                throw new ArgumentNullException(nameof(request));

            if (request.Amount <= 0)
            {
                // Invalid amount
                return new MakePaymentResult { Success = false };
            }

            var dataStore = _dataStoreProvider.Get();
            Account account = dataStore.GetAccount(request.DebtorAccountNumber);

            var validator = _validationResolver.GetValidator(request.PaymentScheme);
            if (validator == null)
            {
                // No validator found for the payment scheme
                return new MakePaymentResult { Success = false };
            }

            var validationResult = validator.Validate(account, request);
            if (!validationResult.IsValid)
            {
                // Validation failed
                return new MakePaymentResult { Success = false };
            }

            account!.Balance -= request.Amount;
            dataStore.UpdateAccount(account);

            return new MakePaymentResult { Success = true };
        }
    }
}

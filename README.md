# Refactor Plan - Overview

PaymentService currently mixes three responsibilities together. (DataStore selection, Scheme Validation, and making the Payment).

This creates tight coupling, makes testing difficult, and increases risk when adding new payment schemes.

The refactor separates these three responsibilities while keeping the original behaviour unless there is a clear correctness issue.

## Planned Changes

1. Behaviour Tests  
    - Add tests to cover the existing behaviour for each payment scheme / data store combination before refactoring.
    - Reasoning: Safeguard against accidental changes in expected behaviour during the refactor.
    - Note: Small changes will be made to allow for easier creation of tests.  
2. Extract DataStore Responsibility  
    - Move DataStore selection and construction out of `PaymentService`.
    - Introduce `IAccountDataStore` and `IDataStoreProvider` abstractions.
    - `PaymentService` will depend on the abstractions.
    - Reasoning: The service should execute a payment, not decide how persistence is managed. Separating this reduces the complexity and allows the service to be tested without worrying about storage concerns.
3. Extract Scheme validation using the strategy pattern. 
    - Move validation logic out of the `PaymentService`.
    - Introduced abstractions `IPaymentSchemeValidator`, `IPaymentSchemeValidationResolver`, `ValidationResult`.
    - Each payment scheme has its own concrete implementation of `IPaymentSchemeValidator` with its own rules.
    - `PaymentService` then calls the validators instead of handling the logic itself.
    - Reasoning: 
      - Removes complexity with a growing switch statement when new payment schemes are added.
      - Validation rules are separated from the service. These rules are tested via the service tests but because of the separation would allow for isolated testing if it was deemed valuable.

## Noted Behaviour Changes introduced during refactor
- Unknown Payment Scheme
   - Previously an unsupported enum value bypassed validation and executed the payment.
   - Now unsupported schemes fail.
   - Reasoning: The payment shouldn't update the balance if the payment scheme has no rules defined.
- <=0 Payment Amount
    - Previously negative amounts could increase the balance.
    - Now if the amount is <= 0 then we reject the request.
    - Reasoning: This method is called `MakePayment` and a negative amount aligns more with ReceivePayment.

## Future improvements
1. Return Failure Information  
    - Currently `MakePaymentResult` only returns a success value. Extending this to return a failure reason will allow the caller to understand why a payment failed.
2. Logging  
    - Add structured logging around validation failures and persistence issues.  
    - Include a correlation ID to trace a request across the services.
2. Update `MakePaymentResult` 
    - At the moment this includes a Success bool but I would also like to extend this by padding a FailureReason property that would be populated on failure.
2. Concurrency Safety  
    - Currently we validate and update in separate steps.  
    - Concurrent requests can update the same balance and succeed.
    - The update should be atomic.



using ClearBank.DeveloperTest.Data;
using ClearBank.DeveloperTest.Services;
using ClearBank.DeveloperTest.Services.Validation;
using ClearBank.DeveloperTest.Types;
using Moq;
using System;
using Xunit;

namespace ClearBank.DeveloperTest.Tests;

public class PaymentServiceTests
{
    private static IPaymentSchemeValidationResolver CreateValidationResolver()
    {
        return new PaymentSchemeValidationResolver(new IPaymentSchemeValidator[]
        {
            new BacsPaymentValidator(),
            new FasterPaymentsValidator(),
            new ChapsPaymentValidator()
        });
    }

    private static (PaymentService sut, Mock<IAccountDataStore> store) CreateSut(Account? account)
    {
        var store = new Mock<IAccountDataStore>();
        store.Setup(s => s.GetAccount("12345678")).Returns(account);

        var provider = new Mock<IDataStoreProvider>();
        provider.Setup(p => p.Get()).Returns(store.Object);

        var sut = new PaymentService(provider.Object, CreateValidationResolver());

        return (sut, store);
    }

    [Fact]
    public void Bacs_AccountMissing_ReturnsFailure()
    {
        // Arrange
        var (sut, store) = CreateSut(null);

        var request = new MakePaymentRequest
        {
            DebtorAccountNumber = "12345678",
            PaymentScheme = PaymentScheme.Bacs,
            Amount = 100m
        };

        // Act
        var result = sut.MakePayment(request);

        // Assert
        Assert.False(result.Success);
        store.Verify(s => s.UpdateAccount(It.IsAny<Account>()), Times.Never);
    }

    [Fact]
    public void Bacs_AccountNotAllowed_ReturnsFailure()
    {
        // Arrange
        var account = new Account
        {
            AllowedPaymentSchemes = AllowedPaymentSchemes.FasterPayments
        };

        var (sut, store) = CreateSut(account);

        var request = new MakePaymentRequest
        {
            DebtorAccountNumber = "12345678",
            PaymentScheme = PaymentScheme.Bacs,
            Amount = 100m
        };

        // Act
        var result = sut.MakePayment(request);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(0m, account.Balance);
        store.Verify(s => s.UpdateAccount(It.IsAny<Account>()), Times.Never);
    }

    [Fact]
    public void Bacs_AccountAllowed_ReturnsSuccess_UpdatesAccount()
    {
        // Arrange
        var account = new Account
        {
            AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs,
            Balance = 200m
        };

        var (sut, store) = CreateSut(account);

        var request = new MakePaymentRequest
        {
            DebtorAccountNumber = "12345678",
            PaymentScheme = PaymentScheme.Bacs,
            Amount = 100m
        };

        // Act
        var result = sut.MakePayment(request);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(100m, account.Balance);
        store.Verify(store => store.UpdateAccount(It.Is<Account>(a => a == account)), Times.Once);
    }

    [Fact]
    public void FasterPayments_AccountMissing_ReturnsFailure()
    {
        // Arrange
        var (sut, store) = CreateSut(null);

        var request = new MakePaymentRequest
        {
            DebtorAccountNumber = "12345678",
            PaymentScheme = PaymentScheme.FasterPayments,
            Amount = 100m
        };

        // Act
        var result = sut.MakePayment(request);

        // Assert
        Assert.False(result.Success);
        store.Verify(s => s.UpdateAccount(It.IsAny<Account>()), Times.Never);
    }

    [Fact]
    public void FasterPayments_AccountNotAllowed_ReturnsFailure()
    {
        // Arrange
        var account = new Account
        {
            AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs,
            Balance = 200m
        };

        var (sut, store) = CreateSut(account);

        var request = new MakePaymentRequest
        {
            DebtorAccountNumber = "12345678",
            PaymentScheme = PaymentScheme.FasterPayments,
            Amount = 100m
        };

        // Act
        var result = sut.MakePayment(request);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(200m, account.Balance);
        store.Verify(s => s.UpdateAccount(It.IsAny<Account>()), Times.Never);
    }

    [Fact]
    public void FasterPayments_InsufficientFunds_ReturnsFailure()
    {
        // Arrange
        var account = new Account
        {
            AllowedPaymentSchemes = AllowedPaymentSchemes.FasterPayments,
            Balance = 50m
        };

        var (sut, store) = CreateSut(account);

        var request = new MakePaymentRequest
        {
            DebtorAccountNumber = "12345678",
            PaymentScheme = PaymentScheme.FasterPayments,
            Amount = 100m
        };

        // Act
        var result = sut.MakePayment(request);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(50m, account.Balance);
        store.Verify(s => s.UpdateAccount(It.IsAny<Account>()), Times.Never);
    }

    [Fact]
    public void FasterPayments_AccountAllowed_ReturnsSuccess_UpdatesAccount()
    {
        // Arrange
        var account = new Account
        {
            AllowedPaymentSchemes = AllowedPaymentSchemes.FasterPayments,
            Balance = 200m
        };

        var (sut, store) = CreateSut(account);

        var request = new MakePaymentRequest
        {
            DebtorAccountNumber = "12345678",
            PaymentScheme = PaymentScheme.FasterPayments,
            Amount = 100m
        };

        // Act
        var result = sut.MakePayment(request);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(100m, account.Balance);
        store.Verify(store => store.UpdateAccount(It.Is<Account>(a => a == account)), Times.Once);
    }

    [Fact]
    public void Chaps_AccountMissing_ReturnsFailure()
    {
        // Arrange
        var (sut, store) = CreateSut(null);

        var request = new MakePaymentRequest
        {
            DebtorAccountNumber = "12345678",
            PaymentScheme = PaymentScheme.Chaps,
            Amount = 100m
        };

        // Act
        var result = sut.MakePayment(request);

        // Assert
        Assert.False(result.Success);
        store.Verify(s => s.UpdateAccount(It.IsAny<Account>()), Times.Never);
    }

    [Fact]
    public void Chaps_AccountNotAllowed_ReturnsFailure()
    {
        // Arrange
        var account = new Account
        {
            AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs,
            Balance = 200m,
            Status = AccountStatus.Live
        };

        var (sut, store) = CreateSut(account);

        var request = new MakePaymentRequest
        {
            DebtorAccountNumber = "12345678",
            PaymentScheme = PaymentScheme.Chaps,
            Amount = 100m
        };

        // Act
        var result = sut.MakePayment(request);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(200m, account.Balance);
        store.Verify(s => s.UpdateAccount(It.IsAny<Account>()), Times.Never);
    }

    [Fact]
    public void Chaps_AccountNotLive_ReturnsFailure()
    {
        // Arrange
        var account = new Account
        {
            AllowedPaymentSchemes = AllowedPaymentSchemes.Chaps,
            Balance = 200m,
            Status = AccountStatus.Disabled
        };

        var (sut, store) = CreateSut(account);

        var request = new MakePaymentRequest
        {
            DebtorAccountNumber = "12345678",
            PaymentScheme = PaymentScheme.Chaps,
            Amount = 100m
        };

        // Act
        var result = sut.MakePayment(request);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(200m, account.Balance);
        store.Verify(s => s.UpdateAccount(It.IsAny<Account>()), Times.Never);
    }

    [Fact]
    public void Chaps_AccountAllowed_ReturnsSuccess_UpdatesAccount()
    {
        // Arrange
        var account = new Account
        {
            AllowedPaymentSchemes = AllowedPaymentSchemes.Chaps,
            Balance = 200m,
            Status = AccountStatus.Live
        };

        var (sut, store) = CreateSut(account);

        var request = new MakePaymentRequest
        {
            DebtorAccountNumber = "12345678",
            PaymentScheme = PaymentScheme.Chaps,
            Amount = 100m
        };

        // Act
        var result = sut.MakePayment(request);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(100m, account.Balance);
        store.Verify(store => store.UpdateAccount(It.Is<Account>(a => a == account)), Times.Once);
    }

    [Fact]
    public void UnknownPaymentScheme_ReturnsFailure()
    {
        // Arrange
        var account = new Account
        {
            AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs | AllowedPaymentSchemes.FasterPayments | AllowedPaymentSchemes.Chaps,
            Balance = 200m,
            Status = AccountStatus.Live
        };

        var (sut, store) = CreateSut(account);

        var request = new MakePaymentRequest
        {
            DebtorAccountNumber = "12345678",
            PaymentScheme = (PaymentScheme)999, // Invalid payment scheme
            Amount = 100m
        };

        // Act
        var result = sut.MakePayment(request);

        // Assert
        Assert.False(result.Success);
        store.Verify(store => store.GetAccount("12345678"), Times.Once);
        store.Verify(store => store.UpdateAccount(It.IsAny<Account>()), Times.Never);
        Assert.Equal(200m, account.Balance);
    }

    [Fact]
    public void MakePayment_NullRequest_ThrowsArgumentNullException()
    {
        // Arrange
        var (sut, store) = CreateSut(null);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => sut.MakePayment(null));
        store.Verify(store => store.GetAccount(It.IsAny<string>()), Times.Never);
        store.Verify(store => store.UpdateAccount(It.IsAny<Account>()), Times.Never);
    }

    [Fact]
    public void MakePayment_InvalidAmount_ReturnsFailure()
    {
        // Arrange
        var account = new Account
        {
            AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs | AllowedPaymentSchemes.FasterPayments | AllowedPaymentSchemes.Chaps,
            Balance = 200m,
            Status = AccountStatus.Live
        };

        var (sut, store) = CreateSut(account);

        var request = new MakePaymentRequest
        {
            DebtorAccountNumber = "12345678",
            PaymentScheme = PaymentScheme.Chaps,
            Amount = -100m
        };

        // Act
        var result = sut.MakePayment(request);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(200m, account.Balance);
        store.Verify(store => store.UpdateAccount(It.IsAny<Account>()), Times.Never);
    }
}

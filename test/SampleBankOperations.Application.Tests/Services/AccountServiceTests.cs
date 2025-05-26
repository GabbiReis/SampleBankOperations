using Bogus;
using FluentAssertions;
using Moq;
using SampleBankOperations.App.Services.Operations;
using SampleBankOperations.Application.Interfaces;
using SampleBankOperations.Core.Entities;
using SampleBankOperations.Core.Interfaces;

namespace SampleBankOperations.Tests;

public class AccountServiceTests
{
    private readonly Mock<IAccountService> _accountServiceMock;
    private readonly Mock<IAccountRepository> _accountRepositoryMock;
    private readonly Mock<ILogger> _loggerMock;
    private readonly BankOperations _bankOperations;
    private readonly Faker _faker;

    public AccountServiceTests()
    {
        _accountServiceMock = new Mock<IAccountService>();
        _accountRepositoryMock = new Mock<IAccountRepository>();
        _loggerMock = new Mock<ILogger>();
        _bankOperations = new BankOperations(_accountServiceMock.Object, _accountRepositoryMock.Object, _loggerMock.Object);
        _faker = new Faker();
    }

    [Fact]
    public void GetAccountByNumber_ShouldReturnAccount_WhenAccountExists()
    {
        // Arrange
        var accountNumber = _faker.Finance.Account();
        var expectedAccount = new Account(accountNumber, 100, Core.Enums.AccountType.Checking);

        _accountRepositoryMock.Setup(r => r.GetByAccountNumber(accountNumber))
            .Returns(expectedAccount);

        // Act
        var result = _bankOperations.GetAccountByNumber(accountNumber);

        // Assert
        result.Should().Be(expectedAccount);
    }

    [Theory]
    [InlineData(500, 100)]
    [InlineData(1000, 50)]
    public void ViewBalance_ShouldOutputCorrectBalance_WhenAccountIsValid(decimal initialBalance, decimal expectedBalance)
    {
        // Arrange
        var account = new Account(_faker.Finance.Account(), initialBalance, Core.Enums.AccountType.Checking);
        _accountServiceMock.Setup(s => s.GetBalance(account)).Returns(expectedBalance);

        using var sw = new StringWriter();
        Console.SetOut(sw);

        // Act
        _bankOperations.ViewBalance(account);

        // Assert
        var output = sw.ToString().Trim();
        output.Should().Contain($"Saldo atual da conta {account.AccountNumber}:");
        output.Should().Contain($"{expectedBalance:C}");
    }

    [Theory]
    [InlineData(200)]
    [InlineData(123.45)]
    public void Deposit_ShouldCallServiceAndOutputAmount_WhenInputIsValid(decimal depositAmount)
    {
        // Arrange
        var account = new Account(_faker.Finance.Account(), 0, Core.Enums.AccountType.Checking);

        using var sr = new StringReader(depositAmount.ToString());
        Console.SetIn(sr);
        using var sw = new StringWriter();
        Console.SetOut(sw);

        _accountServiceMock.Setup(s => s.Deposit(account, depositAmount, It.IsAny<Action<decimal>>()))
            .Callback<Account, decimal, Action<decimal>>((_, amt, callback) => callback(amt));

        // Act
        _bankOperations.Deposit(account);

        // Assert
        var output = sw.ToString().Trim();
        output.Should().Contain($"Depositado: {depositAmount:C}");
    }

    [Theory]
    [InlineData(100, true)]
    [InlineData(9999, false)]
    public void Withdraw_ShouldReturnExpectedResult_BasedOnBalance(decimal amount, bool expectedSuccess)
    {
        // Arrange
        var account = new Account(_faker.Finance.Account(), 1000, Core.Enums.AccountType.Checking);

        using var sr = new StringReader(amount.ToString());
        Console.SetIn(sr);
        using var sw = new StringWriter();
        Console.SetOut(sw);

        _accountServiceMock
            .Setup(s => s.Withdraw(account, amount, It.IsAny<Predicate<decimal>>()
))
            .Returns(expectedSuccess);

        // Act
        _bankOperations.Withdraw(account);

        // Assert
        var output = sw.ToString().Trim();
        if (expectedSuccess)
            output.Should().Contain($"Sacado: {amount:C}");
        else
            output.Should().Contain("Saldo insuficiente.");
    }

    [Theory]
    [InlineData(300, true)]
    [InlineData(9999, false)]
    public void Transfer_ShouldDisplayMessage_BasedOnTransferSuccess(decimal transferAmount, bool success)
    {
        // Arrange
        var from = new Account(_faker.Finance.Account(), 1000, Core.Enums.AccountType.Checking);
        var to = new Account(_faker.Finance.Account(), 0, Core.Enums.AccountType.Checking);

        using var sr = new StringReader(transferAmount.ToString());
        Console.SetIn(sr);
        using var sw = new StringWriter();
        Console.SetOut(sw);

        _accountServiceMock.Setup(s =>
            s.Transfer(from, to, transferAmount,
                It.IsAny<Predicate<decimal>>(),
                It.IsAny<Predicate<decimal>>()
))
            .Returns(success);

        // Act
        _bankOperations.Transfer(from, to);

        // Assert
        var output = sw.ToString().Trim();
        if (success)
            output.Should().Contain($"Transferido: {transferAmount:C} da conta {from.AccountNumber} para {to.AccountNumber}");
        else
            output.Should().Contain("Falha na transferência por saldo insuficiente.");
    }

    [Theory]
    [InlineData(1000, 1.5)]
    [InlineData(500, 10)]
    public void CalculateInterest_ShouldOutputCorrectAmount_WhenRateIsValid(decimal balance, decimal rate)
    {
        // Arrange
        var account = new Account(_faker.Finance.Account(), balance, Core.Enums.AccountType.Checking);
        var expectedInterest = balance * (rate / 100);

        using var sr = new StringReader(rate.ToString());
        Console.SetIn(sr);
        using var sw = new StringWriter();
        Console.SetOut(sw);

        _accountServiceMock
            .Setup(s => s.CalculateInterest(account, It.IsAny<Func<decimal, decimal, decimal>>(), rate))
            .Returns(expectedInterest);

        // Act
        _bankOperations.CalculateInterest(account);

        // Assert
        var output = sw.ToString().Trim();
        output.Should().Contain($"Juros calculado: {expectedInterest:C}");
    }
}

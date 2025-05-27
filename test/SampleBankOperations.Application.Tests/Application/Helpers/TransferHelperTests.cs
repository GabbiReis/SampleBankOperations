using System;
using Moq;
using SampleBankOperations.Application.Services.Helpers;
using SampleBankOperations.Core.Entities;
using SampleBankOperations.Core.Enums;
using SampleBankOperations.Core.Interfaces;
using Xunit;
using FluentAssertions;

public class TransferHelperTests
{
    private readonly Mock<IAccountRepository> _accountRepositoryMock;
    private readonly Mock<ILogger> _loggerMock;
    private readonly TransferHelper _helper;

    public TransferHelperTests()
    {
        _accountRepositoryMock = new Mock<IAccountRepository>();
        _loggerMock = new Mock<ILogger>();
        _helper = new TransferHelper(_accountRepositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public void GetValidAccount_ShouldReturnAccount_WhenAccountExists()
    {
        var account = new Account("123", 1000, AccountType.Checking);
        _accountRepositoryMock.Setup(r => r.GetById(account.AccountId)).Returns(account);

        var result = _helper.GetValidAccount(account, "Origem");

        result.Should().Be(account);
        _loggerMock.Verify(l => l.Log(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public void GetValidAccount_ShouldThrow_WhenAccountDoesNotExist()
    {
        var account = new Account("123", 1000, AccountType.Checking);
        _accountRepositoryMock.Setup(r => r.GetById(account.AccountId)).Returns((Account?)null);

        Action act = () => _helper.GetValidAccount(account, "Origem");

        act.Should().Throw<InvalidOperationException>().WithMessage("Origem account not found.");
        _loggerMock.Verify(l => l.Log("Origem account 123 not found."), Times.Once);
    }

    [Theory]
    [InlineData(500, true, true, true)]
    [InlineData(500, false, true, false)]
    [InlineData(500, true, false, false)]
    [InlineData(500, false, false, false)]
    public void HasSufficientBalance_ShouldReturnExpectedResult(
        decimal balance,
        bool canWithdraw,
        bool canTransfer,
        bool expectedResult)
    {
        var account = new Account("123", balance, AccountType.Checking);

        bool result = _helper.HasSufficientBalance(
            account,
            200,
            b => canWithdraw,
            b => canTransfer
        );

        result.Should().Be(expectedResult);

        if (!expectedResult)
        {
            _loggerMock.Verify(l => l.Log(It.Is<string>(s =>
                s.Contains("Insufficient balance in account 123") &&
                s.Contains("for transfer of"))), Times.Once);
        }
        else
        {
            _loggerMock.Verify(l => l.Log(It.IsAny<string>()), Times.Never);
        }
    }

    [Fact]
    public void ExecuteTransfer_ShouldCallActionsAndUpdateAccountsAndLog()
    {
        // Arrange
        var fromAccount = new Account("from123", 500, AccountType.Checking);
        var toAccount = new Account("to456", 300, AccountType.Savings);
        decimal amount = 200;

        var accountRepoMock = new Mock<IAccountRepository>();
        var loggerMock = new Mock<ILogger>();

        var helper = new TransferHelper(accountRepoMock.Object, loggerMock.Object);

        bool withdrawCalled = false;
        bool depositCalled = false;

        void Withdraw(decimal amt) => withdrawCalled = amt == amount;
        void Deposit(decimal amt) => depositCalled = amt == amount;

        // Act
        helper.ExecuteTransfer(fromAccount, toAccount, amount, Withdraw, Deposit);

        // Assert
        withdrawCalled.Should().BeTrue();
        depositCalled.Should().BeTrue();

        accountRepoMock.Verify(r => r.Update(fromAccount), Times.Once);
        accountRepoMock.Verify(r => r.Update(toAccount), Times.Once);

        loggerMock.Verify(l => l.Log(It.Is<string>(s =>
            s.Contains("Transferred") &&
            s.Contains("from account from123") &&
            s.Contains("to account to456")
        )), Times.Once);
    }
}

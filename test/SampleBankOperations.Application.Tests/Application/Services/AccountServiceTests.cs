using Moq;
using SampleBankOperations.Application.Services;
using SampleBankOperations.Core.Entities;
using SampleBankOperations.Core.Enums;
using SampleBankOperations.Core.Interfaces;
using System;
using Xunit;

namespace SampleBankOperations.Tests;

public class AccountServiceTests
{
    private readonly Mock<IAccountRepository> _accountRepoMock;
    private readonly Mock<ILogger> _loggerMock;
    private readonly AccountService _accountService;

    public AccountServiceTests()
    {
        _accountRepoMock = new Mock<IAccountRepository>();
        _loggerMock = new Mock<ILogger>();
        _accountService = new AccountService(_accountRepoMock.Object, _loggerMock.Object);
    }

    [Fact]
    public void CalculateInterest_ReturnsCorrectInterest()
    {
        var account = new Account("123", 1000m, AccountType.Checking);
        decimal rate = 0.05m;

        var result = _accountService.CalculateInterest(account, (balance, r) => balance * r, rate);

        Assert.Equal(50m, result);
        _loggerMock.Verify(l => l.Log(It.Is<string>(s => s.Contains("Calculated interest"))), Times.Once);
    }

    [Fact]
    public void Deposit_UpdatesAccount_LogsAndInvokesCallback()
    {
        var account = new Account("123", 100m, AccountType.Savings);
        decimal amount = 200m;
        bool callbackCalled = false;

        _accountService.Deposit(account, amount, _ => callbackCalled = true);

        Assert.Equal(300m, account.Balance);
        Assert.True(callbackCalled);
        _accountRepoMock.Verify(r => r.Update(account), Times.Once);
        _loggerMock.Verify(l => l.Log(It.Is<string>(s => s.Contains("Deposited"))), Times.Once);
    }

    [Fact]
    public void Withdraw_WhenAllowedAndSufficientBalance_ReturnsTrue()
    {
        var account = new Account("123", 500m, AccountType.Savings);
        decimal amount = 200m;

        bool result = _accountService.Withdraw(account, amount, bal => bal >= amount);

        Assert.True(result);
        Assert.Equal(300m, account.Balance);
        _accountRepoMock.Verify(r => r.Update(account), Times.Once);
        _loggerMock.Verify(l => l.Log(It.Is<string>(s => s.Contains("Withdraw:"))), Times.Once);
    }

    [Fact]
    public void Withdraw_WhenNotAllowed_ReturnsFalse()
    {
        var account = new Account("123", 100m, AccountType.Checking);
        decimal amount = 200m;

        bool result = _accountService.Withdraw(account, amount, bal => bal >= amount);

        Assert.False(result);
        Assert.Equal(100m, account.Balance);
        _loggerMock.Verify(l => l.Log(It.Is<string>(s => s.Contains("Failed to withdraw"))), Times.Once);
    }

    [Fact]
    public void GetBalance_ReturnsBalance_WhenAccountExists()
    {
        var account = new Account("123", 0m, AccountType.Checking);
        var stored = new Account("123", 600m, AccountType.Checking);
        typeof(Account).GetProperty("AccountId")!.SetValue(stored, account.AccountId); // to match IDs
        _accountRepoMock.Setup(r => r.GetById(account.AccountId)).Returns(stored);

        var result = _accountService.GetBalance(account);

        Assert.Equal(600m, result);
        _loggerMock.Verify(l => l.Log(It.Is<string>(s => s.Contains("Checked balance"))), Times.Once);
    }

    [Fact]
    public void GetBalance_ReturnsZero_WhenAccountDoesNotExist()
    {
        var account = new Account("123", 0m, AccountType.Savings);
        _accountRepoMock.Setup(r => r.GetById(account.AccountId)).Returns((Account)null);

        var result = _accountService.GetBalance(account);

        Assert.Equal(0, result);
        _loggerMock.Verify(l => l.Log(It.Is<string>(s => s.Contains("not found"))), Times.Once);
    }

    [Fact]
    public void Transfer_WhenValid_PerformsTransferAndReturnsTrue()
    {
        var from = new Account("1", 1000m, AccountType.Savings);
        var to = new Account("2", 500m, AccountType.Checking);

        _accountRepoMock.Setup(r => r.GetById(from.AccountId)).Returns(from);
        _accountRepoMock.Setup(r => r.GetById(to.AccountId)).Returns(to);

        bool result = _accountService.Transfer(
            from, to, 200m,
            bal => bal >= 200m,
            bal => true
        );

        Assert.True(result);
        Assert.Equal(800m, from.Balance);
        Assert.Equal(700m, to.Balance);
    }

    [Fact]
    public void Transfer_WhenInsufficientBalance_ReturnsFalse()
    {
        var from = new Account("1", 100m, AccountType.Savings);
        var to = new Account("2", 500m, AccountType.Checking);

        _accountRepoMock.Setup(r => r.GetById(from.AccountId)).Returns(from);
        _accountRepoMock.Setup(r => r.GetById(to.AccountId)).Returns(to);

        var result = _accountService.Transfer(
            from, to, 200m,
            bal => bal >= 200m,
            bal => true
        );

        Assert.False(result);
    }
}

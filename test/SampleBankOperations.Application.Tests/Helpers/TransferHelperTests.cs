using FluentAssertions;
using Moq;
using SampleBankOperations.Application.Services.Helpers;
using SampleBankOperations.Core.Entities;
using SampleBankOperations.Core.Interfaces;
using Xunit;

namespace SampleBankOperations.Application.Tests.Services;

public class TransferHelperTests
{
    private readonly Mock<IAccountRepository> _accountRepositoryMock;
    private readonly Mock<ILogger> _loggerMock;
    private readonly TransferHelper _transferHelper;

    public TransferHelperTests()
    {
        _accountRepositoryMock = new Mock<IAccountRepository>();
        _loggerMock = new Mock<ILogger>();
        _transferHelper = new TransferHelper(_accountRepositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public void GetValidAccount_ShouldReturnAccount_WhenAccountExists()
    {
        // Arrange
        var account = new Account("123", 1000, Core.Enums.AccountType.Checking);
        _accountRepositoryMock.Setup(r => r.GetById(account.AccountId)).Returns(account);

        // Act
        var result = _transferHelper.GetValidAccount(account, "Source");

        // Assert
        result.Should().Be(account);
    }

    [Fact]
    public void GetValidAccount_ShouldThrowException_WhenAccountDoesNotExist()
    {
        // Arrange
        var account = new Account("123", 1000, Core.Enums.AccountType.Checking);
        _accountRepositoryMock.Setup(r => r.GetById(account.AccountId)).Returns((Account?)null);

        // Act
        Action act = () => _transferHelper.GetValidAccount(account, "Source");

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Source account not found.");
        _loggerMock.Verify(l => l.Log(It.Is<string>(msg => msg.Contains("not found"))), Times.Once);
    }

    [Theory]
    [InlineData(1000, 500, true, true, true)]
    [InlineData(1000, 500, false, true, false)]
    [InlineData(1000, 500, true, false, false)]
    public void HasSufficientBalance_ShouldReturnExpectedResult(
        decimal balance, decimal amount, bool canWithdraw, bool canTransfer, bool expectedResult)
    {
        // Arrange
        var account = new Account("123", balance, Core.Enums.AccountType.Checking);

        // Act
        var result = _transferHelper.HasSufficientBalance(
            account,
            amount,
            b => canWithdraw,
            b => canTransfer
        );

        // Assert
        result.Should().Be(expectedResult);
        if (!expectedResult)
        {
            _loggerMock.Verify(l => l.Log(It.Is<string>(msg => msg.Contains("Insufficient balance"))), Times.Once);
        }
    }

    [Fact]
    public void ExecuteTransfer_ShouldCallActionsAndUpdateAccountsAndLog()
    {
        // Arrange
        var from = new Account("123", 1000, Core.Enums.AccountType.Checking);
        var to = new Account("456", 500, Core.Enums.AccountType.Checking);
        decimal transferred = 200;
        bool withdrew = false, deposited = false;

        // Act
        _transferHelper.ExecuteTransfer(
            from,
            to,
            transferred,
            amt => withdrew = amt == transferred,
            amt => deposited = amt == transferred
        );

        // Assert
        withdrew.Should().BeTrue();
        deposited.Should().BeTrue();
        _accountRepositoryMock.Verify(r => r.Update(from), Times.Once);
        _accountRepositoryMock.Verify(r => r.Update(to), Times.Once);
        _loggerMock.Verify(l => l.Log(It.Is<string>(msg => msg.Contains($"Transferred"))), Times.Once);
    }
}

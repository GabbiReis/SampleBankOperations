using FluentAssertions;
using Moq;
using SampleBankOperations.App.Interfaces;
using SampleBankOperations.Core.Entities;
using Xunit;

namespace SampleBankOperations.App.Tests
{
    public class IBankOperationsTests
    {
        private readonly Mock<IBankOperations> _bankOperationsMock;

        public IBankOperationsTests()
        {
            _bankOperationsMock = new Mock<IBankOperations>();
        }

        [Fact]
        public void GetAccountByNumber_ShouldReturnAccount()
        {
            // Arrange
            var account = new Account("123", 1000, Core.Enums.AccountType.Checking);
            _bankOperationsMock.Setup(x => x.GetAccountByNumber("123")).Returns(account);

            // Act
            var result = _bankOperationsMock.Object.GetAccountByNumber("123");

            // Assert
            result.Should().Be(account);
        }

        [Fact]
        public void OpenAccount_ShouldBeCalled()
        {
            // Act
            _bankOperationsMock.Object.OpenAccount();

            // Assert
            _bankOperationsMock.Verify(x => x.OpenAccount(), Times.Once);
        }

        [Fact]
        public void ViewBalance_ShouldBeCalled()
        {
            var account = new Account("123", 500, Core.Enums.AccountType.Checking);

            // Act
            _bankOperationsMock.Object.ViewBalance(account);

            // Assert
            _bankOperationsMock.Verify(x => x.ViewBalance(account), Times.Once);
        }

        [Fact]
        public void Deposit_ShouldBeCalled()
        {
            var account = new Account("123", 500, Core.Enums.AccountType.Checking);

            // Act
            _bankOperationsMock.Object.Deposit(account);

            // Assert
            _bankOperationsMock.Verify(x => x.Deposit(account), Times.Once);
        }

        [Fact]
        public void Withdraw_ShouldBeCalled()
        {
            var account = new Account("123", 500, Core.Enums.AccountType.Checking);

            // Act
            _bankOperationsMock.Object.Withdraw(account);

            // Assert
            _bankOperationsMock.Verify(x => x.Withdraw(account), Times.Once);
        }

        [Fact]
        public void Transfer_ShouldBeCalled()
        {
            var fromAccount = new Account("123", 1000, Core.Enums.AccountType.Checking);
            var toAccount = new Account("456", 500, Core.Enums.AccountType.Savings);

            // Act
            _bankOperationsMock.Object.Transfer(fromAccount, toAccount);

            // Assert
            _bankOperationsMock.Verify(x => x.Transfer(fromAccount, toAccount), Times.Once);
        }

        [Fact]
        public void CalculateInterest_ShouldBeCalled()
        {
            var account = new Account("123", 1500, Core.Enums.AccountType.Checking);

            // Act
            _bankOperationsMock.Object.CalculateInterest(account);

            // Assert
            _bankOperationsMock.Verify(x => x.CalculateInterest(account), Times.Once);
        }
    }
}

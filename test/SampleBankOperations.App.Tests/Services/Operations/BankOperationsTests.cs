using Moq;
using SampleBankOperations.App.Services.Operations;
using SampleBankOperations.Application.Interfaces;
using SampleBankOperations.Core.Interfaces;
using SampleBankOperations.Core.Entities;
using FluentAssertions;

namespace SampleBankOperations.Tests.App.Services.Operations
{
    public class BankOperationsTests
    {
        private readonly Mock<IAccountService> _accountServiceMock = new();
        private readonly Mock<IAccountRepository> _accountRepoMock = new();
        private readonly Mock<ILogger> _loggerMock = new();

        private BankOperations CreateService() =>
            new BankOperations(_accountServiceMock.Object, _accountRepoMock.Object, _loggerMock.Object);

        private static string Normalize(string input) =>
            input.Replace('\u00A0', ' ').Trim();

        [Fact]
        public void GetAccountByNumber_ShouldReturnAccount()
        {
            var expected = new Account("123", 100, SampleBankOperations.Core.Enums.AccountType.Checking);
            _accountRepoMock.Setup(r => r.GetByAccountNumber("123")).Returns(expected);

            var service = CreateService();
            var result = service.GetAccountByNumber("123");

            result.Should().Be(expected);
        }

        [Fact]
        public void ViewBalance_ShouldWriteBalance()
        {
            var account = new Account("123", 500, SampleBankOperations.Core.Enums.AccountType.Checking);
            _accountServiceMock.Setup(s => s.GetBalance(account)).Returns(500m);
            var sw = new StringWriter();
            Console.SetOut(sw);

            var service = CreateService();
            service.ViewBalance(account);

            var output = Normalize(sw.ToString());
            output.Should().Contain("Saldo atual da conta 123: R$ 500,00");
        }

        [Fact]
        public void Deposit_ShouldCallDepositMethod_WhenInputIsValid()
        {
            var account = new Account("123", 100, SampleBankOperations.Core.Enums.AccountType.Checking);
            var input = new StringReader("150");
            Console.SetIn(input);

            var service = CreateService();
            service.Deposit(account);

            _accountServiceMock.Verify(x => x.Deposit(account, 150, It.IsAny<Action<decimal>>()), Times.Once);
        }

        [Fact]
        public void Withdraw_ShouldCallWithdraw_WhenValidAmount()
        {
            var account = new Account("123", 200, SampleBankOperations.Core.Enums.AccountType.Checking);
            var input = new StringReader("100");
            Console.SetIn(input);
            _accountServiceMock
                .Setup(s => s.Withdraw(It.IsAny<Account>(), It.IsAny<decimal>(), It.IsAny<Predicate<decimal>>()))
                .Returns(true);

            var sw = new StringWriter();
            Console.SetOut(sw);

            var service = CreateService();
            service.Withdraw(account);

            var output = Normalize(sw.ToString());
            output.Should().Contain("Sacado: R$ 100,00");
        }

        [Fact]
        public void Transfer_ShouldCallTransferMethod_WhenValidInput()
        {
            var from = new Account("111", 300, SampleBankOperations.Core.Enums.AccountType.Checking);
            var to = new Account("222", 100, SampleBankOperations.Core.Enums.AccountType.Checking);
            var input = new StringReader("200");
            Console.SetIn(input);
            _accountServiceMock
                .Setup(s => s.Transfer(from, to, 200, It.IsAny<Predicate<decimal>>(), It.IsAny<Predicate<decimal>>()))
                .Returns(true);

            var sw = new StringWriter();
            Console.SetOut(sw);

            var service = CreateService();
            service.Transfer(from, to);

            var output = Normalize(sw.ToString());
            output.Should().Contain("Transferido: R$ 200,00 da conta 111 para 222");
        }

        [Fact]
        public void CalculateInterest_ShouldPrintInterest()
        {
            var account = new Account("333", 1000, SampleBankOperations.Core.Enums.AccountType.Checking);
            var input = new StringReader("10");
            Console.SetIn(input);
            _accountServiceMock
                .Setup(s => s.CalculateInterest(account, It.IsAny<Func<decimal, decimal, decimal>>(), 10))
                .Returns(100);

            var sw = new StringWriter();
            Console.SetOut(sw);

            var service = CreateService();
            service.CalculateInterest(account);

            var output = Normalize(sw.ToString());
            output.Should().Contain("Juros calculado: R$ 100,00");
        }

        [Fact]
        public void OpenAccount_ShouldCreateAccount_WhenInputIsValid()
        {
            var input = new StringReader("12345\n500");
            Console.SetIn(input);
            var sw = new StringWriter();
            Console.SetOut(sw);

            var service = CreateService();
            service.OpenAccount();

            _accountRepoMock.Verify(r => r.Add(It.Is<Account>(a =>
                a.AccountNumber == "12345" && a.Balance == 500m)), Times.Once);
        }
    }
}

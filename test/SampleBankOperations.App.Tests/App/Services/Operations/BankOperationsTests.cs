using Moq;
using SampleBankOperations.App.Interfaces;
using SampleBankOperations.App.Services.Operations;
using SampleBankOperations.Application.Interfaces;
using SampleBankOperations.Core.Entities;
using SampleBankOperations.Core.Enums;
using SampleBankOperations.Core.Interfaces;
using System;
using System.IO;
using Xunit;

namespace SampleBankOperations.App.Tests
{
    public class BankOperationsTests : IDisposable
    {
        private readonly Mock<IAccountService> _accountServiceMock;
        private readonly Mock<IAccountRepository> _accountRepositoryMock;
        private readonly Mock<ILogger> _loggerMock;
        private readonly BankOperations _bankOperations;

        private readonly TextReader _originalIn;
        private readonly TextWriter _originalOut;

        public BankOperationsTests()
        {
            _originalIn = Console.In;
            _originalOut = Console.Out;

            _accountServiceMock = new Mock<IAccountService>();
            _accountRepositoryMock = new Mock<IAccountRepository>();
            _loggerMock = new Mock<ILogger>();

            _bankOperations = new BankOperations(_accountServiceMock.Object, _accountRepositoryMock.Object, _loggerMock.Object);
        }

        public void Dispose()
        {
            Console.SetIn(_originalIn);
            Console.SetOut(_originalOut);
        }

        [Fact]
        public void OpenAccount_ValidInput_AddsAccountAndLogs()
        {
            Console.SetIn(new StringReader("12345\n1000\n"));

            _bankOperations.OpenAccount();

            _accountRepositoryMock.Verify(ar => ar.Add(It.Is<Account>(a => a.AccountNumber == "12345" && a.Balance == 1000m && a.AccountType == AccountType.Checking)), Times.Once);
            _loggerMock.Verify(l => l.Log(It.Is<string>(s => s.Contains("Conta 12345 criada"))), Times.Once);
        }

        [Fact]
        public void OpenAccount_InvalidInitialBalance_WritesInvalidValue()
        {
            var output = new StringWriter();
            Console.SetIn(new StringReader("12345\nabc\n"));
            Console.SetOut(output);

            _bankOperations.OpenAccount();

            Assert.Contains("Valor inválido", output.ToString());
            _accountRepositoryMock.Verify(ar => ar.Add(It.IsAny<Account>()), Times.Never);
            _loggerMock.Verify(l => l.Log(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public void GetAccountByNumber_ReturnsAccountFromRepository()
        {
            var account = new Account("123", 500, AccountType.Checking);
            _accountRepositoryMock.Setup(ar => ar.GetByAccountNumber("123")).Returns(account);

            var result = _bankOperations.GetAccountByNumber("123");

            Assert.Equal(account, result);
        }

        [Fact]
        public void ViewBalance_PrintsBalance()
        {
            var account = new Account("123", 500, AccountType.Checking);
            _accountServiceMock.Setup(s => s.GetBalance(account)).Returns(500m);

            var output = new StringWriter();
            Console.SetOut(output);

            _bankOperations.ViewBalance(account);

            Assert.Contains("Saldo atual da conta 123: R$ 500,00", output.ToString());
        }

        [Fact]
        public void Deposit_ValidInput_CallsDepositAndPrints()
        {
            var account = new Account("123", 500, AccountType.Checking);
            Console.SetIn(new StringReader("200\n"));
            var output = new StringWriter();
            Console.SetOut(output);

            _accountServiceMock.Setup(s => s.Deposit(account, 200m, It.IsAny<Action<decimal>>()))
                .Callback<Account, decimal, Action<decimal>>((acc, amt, action) => action(amt));

            _bankOperations.Deposit(account);

            _accountServiceMock.Verify(s => s.Deposit(account, 200m, It.IsAny<Action<decimal>>()), Times.Once);
            Assert.Contains("Depositado: R$ 200,00", output.ToString());
        }

        [Fact]
        public void Deposit_InvalidInput_PrintsInvalidValue()
        {
            var account = new Account("123", 500, AccountType.Checking);
            Console.SetIn(new StringReader("abc\n"));
            var output = new StringWriter();
            Console.SetOut(output);

            _bankOperations.Deposit(account);

            Assert.Contains("Valor inválido", output.ToString());
            _accountServiceMock.Verify(s => s.Deposit(It.IsAny<Account>(), It.IsAny<decimal>(), It.IsAny<Action<decimal>>()), Times.Never);
        }

        [Fact]
        public void Withdraw_ValidInput_SuccessfulWithdrawal()
        {
            var account = new Account("123", 500, AccountType.Checking);
            Console.SetIn(new StringReader("200\n"));
            var output = new StringWriter();
            Console.SetOut(output);

            _accountServiceMock.Setup(s => s.Withdraw(account, 200m, It.IsAny<Predicate<decimal>>())).Returns(true);

            _bankOperations.Withdraw(account);

            Assert.Contains("Sacado: R$ 200,00", output.ToString());
        }

        [Fact]
        public void Withdraw_ValidInput_InsufficientBalance()
        {
            var account = new Account("123", 500, AccountType.Checking);
            Console.SetIn(new StringReader("600\n"));
            var output = new StringWriter();
            Console.SetOut(output);

            _accountServiceMock.Setup(s => s.Withdraw(account, 600m, It.IsAny<Predicate<decimal>>())).Returns(false);

            _bankOperations.Withdraw(account);

            Assert.Contains("Saldo insuficiente", output.ToString());
        }

        [Fact]
        public void Withdraw_InvalidInput_PrintsInvalidValue()
        {
            var account = new Account("123", 500, AccountType.Checking);
            Console.SetIn(new StringReader("abc\n"));
            var output = new StringWriter();
            Console.SetOut(output);

            _bankOperations.Withdraw(account);

            Assert.Contains("Valor inválido", output.ToString());
            _accountServiceMock.Verify(s => s.Withdraw(It.IsAny<Account>(), It.IsAny<decimal>(), It.IsAny<Predicate<decimal>>()), Times.Never);
        }

        [Fact]
        public void Transfer_ValidInput_SuccessfulTransfer()
        {
            var fromAccount = new Account("123", 500, AccountType.Checking);
            var toAccount = new Account("456", 200, AccountType.Checking);
            Console.SetIn(new StringReader("100\n"));
            var output = new StringWriter();
            Console.SetOut(output);

            _accountServiceMock.Setup(s => s.Transfer(
                fromAccount,
                toAccount,
                100m,
                It.IsAny<Predicate<decimal>>(),
                It.IsAny<Predicate<decimal>>()))
                .Returns(true);

            _bankOperations.Transfer(fromAccount, toAccount);

            Assert.Contains("Transferido: R$ 100,00 da conta 123 para 456", output.ToString());
        }

        [Fact]
        public void Transfer_ValidInput_FailedTransfer()
        {
            var fromAccount = new Account("123", 500, AccountType.Checking);
            var toAccount = new Account("456", 200, AccountType.Checking);
            Console.SetIn(new StringReader("100\n"));
            var output = new StringWriter();
            Console.SetOut(output);

            _accountServiceMock.Setup(s => s.Transfer(
                fromAccount,
                toAccount,
                100m,
                It.IsAny<Predicate<decimal>>(),
                It.IsAny<Predicate<decimal>>()))
                .Returns(false);

            _bankOperations.Transfer(fromAccount, toAccount);

            Assert.Contains("Falha na transferência por saldo insuficiente", output.ToString());
        }

        [Fact]
        public void Transfer_InvalidInput_PrintsInvalidValue()
        {
            var fromAccount = new Account("123", 500, AccountType.Checking);
            var toAccount = new Account("456", 200, AccountType.Checking);
            Console.SetIn(new StringReader("abc\n"));
            var output = new StringWriter();
            Console.SetOut(output);

            _bankOperations.Transfer(fromAccount, toAccount);

            Assert.Contains("Valor inválido", output.ToString());
            _accountServiceMock.Verify(s => s.Transfer(It.IsAny<Account>(), It.IsAny<Account>(), It.IsAny<decimal>(), It.IsAny<Predicate<decimal>>(), It.IsAny<Predicate<decimal>>()), Times.Never);
        }

        [Fact]
        public void CalculateInterest_ValidInput_PrintsInterest()
        {
            var account = new Account("123", 1000, AccountType.Checking);
            Console.SetIn(new StringReader("5\n")); // 5%
            var output = new StringWriter();
            Console.SetOut(output);

            _accountServiceMock.Setup(s => s.CalculateInterest(account, It.IsAny<Func<decimal, decimal, decimal>>(), 5m))
                .Returns(50m);

            _bankOperations.CalculateInterest(account);

            Assert.Contains("Juros calculado: R$ 50,00", output.ToString());
        }

        [Fact]
        public void CalculateInterest_InvalidInput_PrintsInvalidRate()
        {
            var account = new Account("123", 1000, AccountType.Checking);
            Console.SetIn(new StringReader("abc\n"));
            var output = new StringWriter();
            Console.SetOut(output);

            _bankOperations.CalculateInterest(account);

            Assert.Contains("Taxa inválida", output.ToString());
            _accountServiceMock.Verify(s => s.CalculateInterest(It.IsAny<Account>(), It.IsAny<Func<decimal, decimal, decimal>>(), It.IsAny<decimal>()), Times.Never);
        }
    }
}

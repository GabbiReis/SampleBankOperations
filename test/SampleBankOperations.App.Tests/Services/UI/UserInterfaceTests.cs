using System;
using System.IO;
using System.Threading;
using FluentAssertions;
using Moq;
using SampleBankOperations.App.Interfaces;
using SampleBankOperations.App.Services.UI;
using SampleBankOperations.Core.Entities;
using SampleBankOperations.Core.Enums;
using Xunit;

namespace SampleBankOperations.App.Tests
{
    public class UserInterfaceTests
    {
        private readonly Mock<IBankOperations> _bankOperationsMock;

        public UserInterfaceTests()
        {
            _bankOperationsMock = new Mock<IBankOperations>();
        }

        private void RunUiWithInputs(string inputText, out string outputText, Action<UserInterface>? setup = null)
        {
            var input = new StringReader(inputText);
            var output = new StringWriter();
            var exited = false;

            var ui = new UserInterface(
                _bankOperationsMock.Object,
                input,
                output,
                () => exited = true
            );

            setup?.Invoke(ui);

            var thread = new Thread(() =>
            {
                try { ui.Run(); } catch { /* ignore */ }
            });

            thread.Start();

            while (!exited) Thread.Sleep(10);
            thread.Interrupt();

            outputText = output.ToString();
        }

        [Fact]
        public void Should_Call_OpenAccount_When_Option1()
        {
            RunUiWithInputs("1\n9\n", out _, ui => { });

            _bankOperationsMock.Verify(x => x.OpenAccount(), Times.Once);
        }

        [Fact]
        public void Should_Call_ViewBalance_When_Option2_With_ValidAccount()
        {
            var acc = new Account("123", 100, AccountType.Savings);
            _bankOperationsMock.Setup(x => x.GetAccountByNumber("123")).Returns(acc);

            RunUiWithInputs("2\n123\n9\n", out _);

            _bankOperationsMock.Verify(x => x.ViewBalance(acc), Times.Once);
        }

        [Fact]
        public void Should_Show_Error_When_Option_Invalid()
        {
            RunUiWithInputs("42\n9\n", out var output);

            output.Should().Contain("Opção inválida, tente novamente.");
        }

        [Fact]
        public void Should_Show_Error_When_Account_Not_Found()
        {
            _bankOperationsMock.Setup(x => x.GetAccountByNumber("000")).Returns((Account?)null);

            RunUiWithInputs("2\n000\n9\n", out var output);

            output.Should().Contain("Conta não encontrada.");
        }

        [Fact]
        public void Should_Call_Deposit_When_Option3_With_ValidAccount()
        {
            var acc = new Account("abc", 200, AccountType.Checking);
            _bankOperationsMock.Setup(x => x.GetAccountByNumber("abc")).Returns(acc);

            RunUiWithInputs("3\nabc\n9\n", out _);

            _bankOperationsMock.Verify(x => x.Deposit(acc), Times.Once);
        }

        [Fact]
        public void Should_Call_Withdraw_When_Option4_With_ValidAccount()
        {
            var acc = new Account("xyz", 300, AccountType.Checking);
            _bankOperationsMock.Setup(x => x.GetAccountByNumber("xyz")).Returns(acc);

            RunUiWithInputs("4\nxyz\n9\n", out _);

            _bankOperationsMock.Verify(x => x.Withdraw(acc), Times.Once);
        }

        [Fact]
        public void Should_Call_Transfer_When_Option5_With_ValidAccounts()
        {
            var from = new Account("111", 1000, AccountType.Checking);
            var to = new Account("222", 500, AccountType.Savings);

            _bankOperationsMock.Setup(x => x.GetAccountByNumber("111")).Returns(from);
            _bankOperationsMock.Setup(x => x.GetAccountByNumber("222")).Returns(to);

            RunUiWithInputs("5\n111\n222\n9\n", out _);

            _bankOperationsMock.Verify(x => x.Transfer(from, to), Times.Once);
        }

        [Fact]
        public void Should_Show_Error_When_Transfer_From_NotFound()
        {
            _bankOperationsMock.Setup(x => x.GetAccountByNumber("x")).Returns((Account?)null);

            RunUiWithInputs("5\nx\n9\n", out var output);

            output.Should().Contain("Conta de origem não encontrada.");
        }

        [Fact]
        public void Should_Show_Error_When_Transfer_To_NotFound()
        {
            var from = new Account("a", 100, AccountType.Savings);
            _bankOperationsMock.Setup(x => x.GetAccountByNumber("a")).Returns(from);
            _bankOperationsMock.Setup(x => x.GetAccountByNumber("b")).Returns((Account?)null);

            RunUiWithInputs("5\na\nb\n9\n", out var output);

            output.Should().Contain("Conta de destino não encontrada.");
        }

        [Fact]
        public void Should_Call_CalculateInterest_When_Option6()
        {
            var acc = new Account("xyz", 3000, AccountType.Savings);
            _bankOperationsMock.Setup(x => x.GetAccountByNumber("xyz")).Returns(acc);

            RunUiWithInputs("6\nxyz\n9\n", out _);

            _bankOperationsMock.Verify(x => x.CalculateInterest(acc), Times.Once);
        }

        [Fact]
        public void Should_Write_Exit_Message_When_Option9()
        {
            RunUiWithInputs("9\n", out var output);

            output.Should().Contain("Obrigado por utilizar o SampleBankOperations!");
        }
    }
}

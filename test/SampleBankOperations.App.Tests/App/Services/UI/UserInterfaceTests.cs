using System;
using System.IO;
using System.Threading.Tasks;
using Moq;
using Xunit;
using SampleBankOperations.App.Services.UI;
using SampleBankOperations.App.Interfaces;
using SampleBankOperations.Core.Entities;
using SampleBankOperations.Core.Enums;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace SampleBankOperations.App.Tests
{
    public class UserInterfaceTests
    {
        private readonly Mock<IBankOperations> _bankOperationsMock;
        private readonly Mock<IAppExit> _appExitMock;
        private readonly UserInterface _userInterface;

        public UserInterfaceTests()
        {
            _bankOperationsMock = new Mock<IBankOperations>();
            _appExitMock = new Mock<IAppExit>();
            _userInterface = new UserInterface(_bankOperationsMock.Object, _appExitMock.Object);
        }

        private void RunWithTimeout(Action runAction, int timeoutMilliseconds = 1000)
        {
            var task = Task.Run(runAction);
            if (!task.Wait(timeoutMilliseconds))
            {
                throw new TimeoutException("O método Run excedeu o tempo limite.");
            }
        }

        [Fact]
        public void Run_InvalidOption_ShowsInvalidOptionMessage()
        {
            var input = new StringReader("0\n9\n");
            var output = new StringWriter();
            Console.SetIn(input);
            Console.SetOut(output);

            try { RunWithTimeout(() => _userInterface.Run()); } catch { }

            var consoleOutput = output.ToString();
            Assert.Contains("Opção inválida", consoleOutput);
        }

        [Fact]
        public void Run_Option1_CallsOpenAccount()
        {
            var input = new StringReader("1\n9\n");
            Console.SetIn(input);
            Console.SetOut(new StringWriter());

            try { RunWithTimeout(() => _userInterface.Run()); } catch { }

            _bankOperationsMock.Verify(b => b.OpenAccount(), Times.Once);
        }

        [Fact]
        public void Run_Option2_CallsViewBalance()
        {
            var account = new Account("123", 1000m, AccountType.Checking);
            _bankOperationsMock.Setup(b => b.GetAccountByNumber("123")).Returns(account);

            var input = new StringReader("2\n123\n9\n");
            Console.SetIn(input);
            Console.SetOut(new StringWriter());

            try { RunWithTimeout(() => _userInterface.Run()); } catch { }

            _bankOperationsMock.Verify(b => b.ViewBalance(account), Times.Once);
        }

        [Fact]
        public void Run_Option3_CallsDeposit()
        {
            var account = new Account("123", 1000m, AccountType.Checking);
            _bankOperationsMock.Setup(b => b.GetAccountByNumber("123")).Returns(account);

            var input = new StringReader("3\n123\n9\n");
            Console.SetIn(input);
            Console.SetOut(new StringWriter());

            try { RunWithTimeout(() => _userInterface.Run()); } catch { }

            _bankOperationsMock.Verify(b => b.Deposit(account), Times.Once);
        }

        [Fact]
        public void Run_Option4_CallsWithdraw()
        {
            var account = new Account("123", 1000m, AccountType.Checking);
            _bankOperationsMock.Setup(b => b.GetAccountByNumber("123")).Returns(account);

            var input = new StringReader("4\n123\n9\n");
            Console.SetIn(input);
            Console.SetOut(new StringWriter());

            try { RunWithTimeout(() => _userInterface.Run()); } catch { }

            _bankOperationsMock.Verify(b => b.Withdraw(account), Times.Once);
        }

        [Fact]
        public void Run_Option5_CallsTransfer()
        {
            var fromAccount = new Account("from", 1000m, AccountType.Checking);
            var toAccount = new Account("to", 500m, AccountType.Checking);

            _bankOperationsMock.Setup(b => b.GetAccountByNumber("from")).Returns(fromAccount);
            _bankOperationsMock.Setup(b => b.GetAccountByNumber("to")).Returns(toAccount);

            var input = new StringReader("5\nfrom\nto\n9\n");
            Console.SetIn(input);
            Console.SetOut(new StringWriter());

            try { RunWithTimeout(() => _userInterface.Run()); } catch { }

            _bankOperationsMock.Verify(b => b.Transfer(fromAccount, toAccount), Times.Once);
        }

        [Fact]
        public void Run_Option6_CallsCalculateInterest()
        {
            var account = new Account("123", 1000m, AccountType.Checking);
            _bankOperationsMock.Setup(b => b.GetAccountByNumber("123")).Returns(account);

            var input = new StringReader("6\n123\n9\n");
            Console.SetIn(input);
            Console.SetOut(new StringWriter());

            try { RunWithTimeout(() => _userInterface.Run()); } catch { }

            _bankOperationsMock.Verify(b => b.CalculateInterest(account), Times.Once);
        }

        [Fact]
        public void ExitApplication_OutputsThankYouMessageAndCallsExit()
        {
            var output = new StringWriter();
            Console.SetOut(output);

            var method = typeof(UserInterface).GetMethod("ExitApplication", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            Assert.NotNull(method);

            method.Invoke(_userInterface, null);

            _appExitMock.Verify(a => a.Exit(0), Times.Once);
            Assert.Contains("Obrigado por utilizar o SampleBankOperations", output.ToString());
        }
    }
}

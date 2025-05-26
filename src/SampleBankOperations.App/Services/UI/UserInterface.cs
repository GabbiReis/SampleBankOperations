using SampleBankOperations.App.Interfaces;
using SampleBankOperations.App.Services.Operations;
using SampleBankOperations.Core.Entities;

namespace SampleBankOperations.App.Services.UI;

public class UserInterface
{
    private readonly IBankOperations _bankOperations;
    private readonly Dictionary<string, Action> _actions;
    private readonly TextReader _input;
    private readonly TextWriter _output;
    private readonly Action _exit;

    public UserInterface(IBankOperations bankOperations,
                         TextReader? input = null,
                         TextWriter? output = null,
                         Action? exit = null)
    {
        _bankOperations = bankOperations;
        _input = input ?? Console.In;
        _output = output ?? Console.Out;
        _exit = exit ?? (() => Environment.Exit(0));

        _actions = new Dictionary<string, Action>
        {
            { "1", () => _bankOperations.OpenAccount() },
            { "2", () => ExecuteWithAccount(_bankOperations.ViewBalance) },
            { "3", () => ExecuteWithAccount(_bankOperations.Deposit) },
            { "4", () => ExecuteWithAccount(_bankOperations.Withdraw) },
            { "5", () => ExecuteWithAccounts(_bankOperations.Transfer) },
            { "6", () => ExecuteWithAccount(_bankOperations.CalculateInterest) },
            { "9", ExitApplication }
        };
    }

    public void Run()
    {
        while (true)
        {
            _output.WriteLine("Bem-vindo ao SampleBankOperations! Escolha uma opção:");
            _output.WriteLine("1. Abrir Conta");
            _output.WriteLine("2. Ver Saldo");
            _output.WriteLine("3. Depositar");
            _output.WriteLine("4. Sacar");
            _output.WriteLine("5. Transferir entre Contas");
            _output.WriteLine("6. Calcular Juros");
            _output.WriteLine("9. Sair");

            var choice = _input.ReadLine();

            if (_actions.TryGetValue(choice!, out var action))
            {
                action();
            }
            else
            {
                _output.WriteLine("Opção inválida, tente novamente.");
            }
        }
    }

    private void ExecuteWithAccount(Action<Account> accountAction)
    {
        _output.Write("Informe o número da conta: ");
        var accountNumber = _input.ReadLine();
        var account = _bankOperations.GetAccountByNumber(accountNumber!);
        if (account == null)
        {
            _output.WriteLine("Conta não encontrada.");
            return;
        }
        accountAction(account);
    }

    private void ExecuteWithAccounts(Action<Account, Account> transferAction)
    {
        _output.Write("Informe o número da conta de origem: ");
        var fromAccountNumber = _input.ReadLine();
        var fromAccount = _bankOperations.GetAccountByNumber(fromAccountNumber!);
        if (fromAccount == null)
        {
            _output.WriteLine("Conta de origem não encontrada.");
            return;
        }

        _output.Write("Informe o número da conta de destino: ");
        var toAccountNumber = _input.ReadLine();
        var toAccount = _bankOperations.GetAccountByNumber(toAccountNumber!);
        if (toAccount == null)
        {
            _output.WriteLine("Conta de destino não encontrada.");
            return;
        }

        transferAction(fromAccount, toAccount);
    }

    private void ExitApplication()
    {
        _output.WriteLine("Obrigado por utilizar o SampleBankOperations!");
        _exit();
    }
}


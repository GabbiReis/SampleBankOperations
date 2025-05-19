using SampleBankOperations.App.Interfaces;
using SampleBankOperations.App.Services.Operations;
using SampleBankOperations.Core.Entities;

namespace SampleBankOperations.App.Services.UI;

public class UserInterface
{
    private readonly IBankOperations _bankOperations;
    private readonly Dictionary<string, Action> _actions;

    public UserInterface(IBankOperations bankOperations)
    {
        _bankOperations = bankOperations;

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
            Console.WriteLine("Bem-vindo ao SampleBankOperations! Escolha uma opção:");
            Console.WriteLine("1. Abrir Conta");
            Console.WriteLine("2. Ver Saldo");
            Console.WriteLine("3. Depositar");
            Console.WriteLine("4. Sacar");
            Console.WriteLine("5. Transferir entre Contas");
            Console.WriteLine("6. Calcular Juros");
            Console.WriteLine("9. Sair");

            var choice = Console.ReadLine();

            if (_actions.TryGetValue(choice!, out var action))
            {
                action();
            }
            else
            {
                Console.WriteLine("Opção inválida, tente novamente.");
            }
        }
    }

    private void ExecuteWithAccount(Action<Account> accountAction)
    {
        Console.Write("Informe o número da conta: ");
        var accountNumber = Console.ReadLine();
        var account = _bankOperations.GetAccountByNumber(accountNumber!);
        if (account == null)
        {
            Console.WriteLine("Conta não encontrada.");
            return;
        }
        accountAction(account);
    }

    private void ExecuteWithAccounts(Action<Account, Account> transferAction)
    {
        Console.Write("Informe o número da conta de origem: ");
        var fromAccountNumber = Console.ReadLine();
        var fromAccount = _bankOperations.GetAccountByNumber(fromAccountNumber!);
        if (fromAccount == null)
        {
            Console.WriteLine("Conta de origem não encontrada.");
            return;
        }

        Console.Write("Informe o número da conta de destino: ");
        var toAccountNumber = Console.ReadLine();
        var toAccount = _bankOperations.GetAccountByNumber(toAccountNumber!);
        if (toAccount == null)
        {
            Console.WriteLine("Conta de destino não encontrada.");
            return;
        }
        transferAction(fromAccount, toAccount);
    }

    private void ExitApplication()
    {
        Console.WriteLine("Obrigado por utilizar o SampleBankOperations!");
        Environment.Exit(0);
    }
}

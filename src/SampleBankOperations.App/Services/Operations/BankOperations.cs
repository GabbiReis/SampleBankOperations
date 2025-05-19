using SampleBankOperations.App.Interfaces;
using SampleBankOperations.Application.Interfaces;
using SampleBankOperations.Application.Validations;
using SampleBankOperations.Core.Entities;
using SampleBankOperations.Core.Interfaces;

namespace SampleBankOperations.App.Services.Operations;

public class BankOperations : IBankOperations
{
    private readonly IAccountService _accountService;
    private readonly IAccountRepository _accountRepository;
    private readonly ILogger _logger;

    public BankOperations(IAccountService accountService, IAccountRepository accountRepository, ILogger logger)
    {
        _accountService = accountService;
        _accountRepository = accountRepository;
        _logger = logger;
    }

    public void OpenAccount()
    {
        Console.Write("Informe o número da conta: ");
        var accountNumber = Console.ReadLine();
        Console.Write("Informe o saldo inicial: ");
        if (decimal.TryParse(Console.ReadLine(), out var initialBalance))
        {
            var account = new Account(accountNumber!, initialBalance, Core.Enums.AccountType.Checking);
            _accountRepository.Add(account);
            _logger.Log($"Conta {accountNumber} criada com saldo de {initialBalance:C}.");
        }
        else
        {
            Console.WriteLine("Valor inválido.");
        }
    }

    public Account GetAccountByNumber(string accountNumber)
    {
        return _accountRepository.GetByAccountNumber(accountNumber);
    }

    public void ViewBalance(Account account)
    {
        var balance = _accountService.GetBalance(account);
        Console.WriteLine($"Saldo atual da conta {account.AccountNumber}: {balance:C}");
    }

    public void Deposit(Account account)
    {
        Console.Write("Informe o valor para depósito: ");
        if (decimal.TryParse(Console.ReadLine(), out var amount))
        {
            _accountService.Deposit(account, amount, amt => Console.WriteLine($"Depositado: {amt:C}"));
        }
        else
        {
            Console.WriteLine("Valor inválido.");
        }
    }

    public void Withdraw(Account account)
    {
        Console.Write("Informe o valor para saque: ");
        if (decimal.TryParse(Console.ReadLine(), out var amount))
        {
            bool success = _accountService.Withdraw(account, amount, AccountValidator.MinimumBalanceValidator(0));
            Console.WriteLine(success ? $"Sacado: {amount:C}" : "Saldo insuficiente.");
        }
        else
        {
            Console.WriteLine("Valor inválido.");
        }
    }

    public void Transfer(Account fromAccount, Account toAccount)
    {
        Console.Write("Informe o valor para transferência: ");
        if (decimal.TryParse(Console.ReadLine(), out var amount))
        {
            bool success = _accountService.Transfer(fromAccount, toAccount, amount,
                AccountValidator.MinimumBalanceValidator(0), AccountValidator.RequestedAmountValidator(amount));

            Console.WriteLine(success ? $"Transferido: {amount:C} da conta {fromAccount.AccountNumber} para {toAccount.AccountNumber}"
                : "Falha na transferência por saldo insuficiente.");
        }
        else
        {
            Console.WriteLine("Valor inválido.");
        }
    }

    public void CalculateInterest(Account account)
    {
        Console.Write("Informe a taxa de juros (%): ");
        if (decimal.TryParse(Console.ReadLine(), out var rate))
        {
            decimal interest = _accountService.CalculateInterest(account, (balance, r) => balance * (r / 100), rate);
            Console.WriteLine($"Juros calculado: {interest:C}");
        }
        else
        {
            Console.WriteLine("Taxa inválida.");
        }
    }
}

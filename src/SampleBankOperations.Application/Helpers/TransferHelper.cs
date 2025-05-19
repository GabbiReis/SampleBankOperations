using SampleBankOperations.Core.Entities;
using SampleBankOperations.Core.Interfaces;

namespace SampleBankOperations.Application.Services.Helpers;

public class TransferHelper
{
    private readonly IAccountRepository _accountRepository;
    private readonly ILogger _logger;

    public TransferHelper(IAccountRepository accountRepository, ILogger logger)
    {
        _accountRepository = accountRepository;
        _logger = logger;
    }

    public Account GetValidAccount(Account account, string accountType)
    {
        var existingAccount = _accountRepository.GetById(account.AccountId);
        if (existingAccount == null)
        {
            _logger.Log($"{accountType} account {account.AccountNumber} not found.");
            throw new InvalidOperationException($"{accountType} account not found.");
        }
        return existingAccount;
    }

    public bool HasSufficientBalance(Account account, decimal amount, Predicate<decimal> canWithdraw, Predicate<decimal> canTransfer)
    {
        if (canWithdraw(account.Balance) && canTransfer(account.Balance)) return true;

        _logger.Log($"Insufficient balance in account {account.AccountNumber} for transfer of {amount:C}.");
        return false;
    }

    public void ExecuteTransfer(Account fromAccount, Account toAccount, decimal amount, Action<decimal> withdrawAction, Action<decimal> depositAction)
    {
        withdrawAction(amount);
        depositAction(amount);

        _accountRepository.Update(fromAccount);
        _accountRepository.Update(toAccount);

        _logger.Log($"Transferred {amount:C} from account {fromAccount.AccountNumber} to account {toAccount.AccountNumber}.");
    }
}

using SampleBankOperations.Application.Interfaces;
using SampleBankOperations.Application.Services.Helpers;
using SampleBankOperations.Core.Entities;
using SampleBankOperations.Core.Interfaces;

namespace SampleBankOperations.Application.Services;

public class AccountService : IAccountService
{
    private readonly IAccountRepository _accountRepository;
    private readonly ILogger _logger;
    private readonly TransferHelper _transferHelper;

    public AccountService(IAccountRepository accountRepository, ILogger logger)
    {
        _accountRepository = accountRepository;
        _logger = logger;
        _transferHelper = new TransferHelper(_accountRepository, _logger);
    }

    public decimal CalculateInterest(Account account, Func<decimal, decimal, decimal> interestCalculator, decimal rate)
    {
        var interest = interestCalculator(account.Balance, rate);
        _logger.Log($"Calculated interest: {interest:C} for account {account.AccountNumber}");
        return interest;
    }

    public void Deposit(Account account, decimal amount, Action<decimal> onDeposit)
    {
        account.Deposit(amount);
        _accountRepository.Update(account);
        onDeposit(amount);
        _logger.Log($"Deposited: {amount:C} to account {account.AccountNumber}. New balance: {account.Balance:C}");
    }

    public bool Withdraw(Account account, decimal amount, Predicate<decimal> canWithdraw)
    {
        if (canWithdraw(account.Balance))
        {
            if (account.Withdraw(amount))
            {
                _accountRepository.Update(account);
                _logger.Log($"Withdraw: {amount:C} from account {account.AccountNumber}. New balance: {account.Balance:C}");
                return true;
            }
        }
        _logger.Log($"Failed to withdraw: {amount:C} from account {account.AccountNumber}. Insufficient balance.");
        return false;
    }

    public decimal GetBalance(Account account)
    {
        var existingAccount = _accountRepository.GetById(account.AccountId);
        if (existingAccount == null)
        {
            _logger.Log($"Account {account.AccountNumber} not found.");
            return 0;
        }

        _logger.Log($"Checked balance for account {account.AccountNumber}. Current balance: {existingAccount.Balance:C}");
        return existingAccount.Balance;
    }

    public bool Transfer(Account fromAccount, Account toAccount, decimal amount, 
        Predicate<decimal> canWithdraw, 
        Predicate<decimal> canTransfer)
    {
        var existingFromAccount = _transferHelper.GetValidAccount(fromAccount, "Source");
        var existingToAccount = _transferHelper.GetValidAccount(toAccount, "Destination");

        if (!_transferHelper.HasSufficientBalance(existingFromAccount, amount, canWithdraw, canTransfer)) return false;

        _transferHelper.ExecuteTransfer(existingFromAccount, existingToAccount, amount,
            amt => Withdraw(existingFromAccount, amt, canWithdraw),
            amt => Deposit(existingToAccount, amt, _ => { }));

        return true;
    }
}

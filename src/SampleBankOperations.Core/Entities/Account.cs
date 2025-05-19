using SampleBankOperations.Core.Enums;

namespace SampleBankOperations.Core.Entities;

public class Account
{
    public Guid AccountId { get; private set; }
    public string AccountNumber { get; private set; }
    public decimal Balance { get; private set; }
    public AccountType AccountType { get; private set; }

    public Account(string accountNumber, decimal initialBalance, AccountType accountType)
    {
        AccountId = Guid.NewGuid();
        AccountNumber = accountNumber;
        Balance = initialBalance;
        AccountType = accountType;
    }

    public void Deposit(decimal amount)
    {
        Balance += amount;
    }

    public bool Withdraw(decimal amount)
    {
        if (amount <= Balance)
        {
            Balance -= amount;
            return true;
        }
        return false;
    }
}

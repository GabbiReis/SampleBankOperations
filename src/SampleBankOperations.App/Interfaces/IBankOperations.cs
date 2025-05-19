using SampleBankOperations.Core.Entities;

namespace SampleBankOperations.App.Interfaces;

public interface IBankOperations
{
    void OpenAccount();
    Account GetAccountByNumber(string accountNumber);
    void ViewBalance(Account account);
    void Deposit(Account account);
    void Withdraw(Account account);
    void Transfer(Account fromAccount, Account toAccount);
    void CalculateInterest(Account account);
}

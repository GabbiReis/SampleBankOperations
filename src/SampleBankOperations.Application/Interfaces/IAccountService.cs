using SampleBankOperations.Core.Entities;

namespace SampleBankOperations.Application.Interfaces;

public interface IAccountService
{
    decimal GetBalance(Account account);
    void Deposit(Account account, decimal amount, Action<decimal> onDeposit);
    bool Withdraw(Account account, decimal amount, Predicate<decimal> canWithdraw);
    bool Transfer(Account fromAccount, Account toAccount, decimal amount, Predicate<decimal> canWithdraw, Predicate<decimal> canTransfer);
    decimal CalculateInterest(Account account, Func<decimal, decimal, decimal> interestCalculator, decimal rate);
}

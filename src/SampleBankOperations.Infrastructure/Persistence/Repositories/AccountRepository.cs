using SampleBankOperations.Core.Entities;
using SampleBankOperations.Core.Interfaces;

namespace SampleBankOperations.Infrastructure.Persistence.Repositories;

public class AccountRepository : IAccountRepository
{
    private readonly BankingDbContext _context;

    public AccountRepository(BankingDbContext context)
    {
        _context = context;
    }

    public Account GetById(Guid id)
    {
        _context.Accounts.TryGetValue(id, out var account);
        return account!;
    }

    public IEnumerable<Account> GetAll()
    {
        return _context.Accounts.Values;
    }

    public void Add(Account account)
    {
        _context.Accounts[account.AccountId] = account;
    }

    public void Update(Account account)
    {
        _context.Accounts[account.AccountId] = account;
    }

    public void Remove(Account account)
    {
        _context.Accounts.Remove(account.AccountId);
    }

    public Account GetByAccountNumber(string accountNumber)
    {
        return _context.Accounts.Values.FirstOrDefault(a => a.AccountNumber == accountNumber)!;
    }
}

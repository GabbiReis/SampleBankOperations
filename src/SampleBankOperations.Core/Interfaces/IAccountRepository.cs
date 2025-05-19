using SampleBankOperations.Core.Entities;

namespace SampleBankOperations.Core.Interfaces;

public interface IAccountRepository : IRepository<Account>
{
    Account GetByAccountNumber(string accountNumber);
}

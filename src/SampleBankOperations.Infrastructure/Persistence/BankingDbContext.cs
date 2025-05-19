using SampleBankOperations.Core.Entities;

namespace SampleBankOperations.Infrastructure.Persistence
{
    public class BankingDbContext
    {
        private readonly Dictionary<Guid, Account> _accounts = new();

        public Dictionary<Guid, Account> Accounts => _accounts;
    }
}

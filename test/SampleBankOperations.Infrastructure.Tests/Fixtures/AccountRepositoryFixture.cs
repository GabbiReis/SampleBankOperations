using SampleBankOperations.Core.Entities;
using SampleBankOperations.Infrastructure.Persistence;
using SampleBankOperations.Infrastructure.Persistence.Repositories;
using System;
using System.Collections.Generic;

namespace SampleBankOperations.Tests.Fixtures
{
    public class AccountRepositoryFixture : IDisposable
    {
        public BankingDbContext Context { get; private set; }
        public AccountRepository Repository { get; private set; }

        public AccountRepositoryFixture()
        {
            var accounts = new Dictionary<Guid, Account>();
            Context = new BankingDbContext(accounts);
            Repository = new AccountRepository(Context);
        }

        public Account CreateAccount(string? number = null)
        {
            var account = new Account
            {
                AccountId = Guid.NewGuid(),
                AccountNumber = number ?? "123456789"
            };

            Context.Accounts[account.AccountId] = account;
            return account;
        }

        public void Dispose()
        {
            Context.Accounts.Clear();
        }
    }
}

using SampleBankOperations.Core.Entities;
using SampleBankOperations.Core.Enums;
using SampleBankOperations.Infrastructure.Persistence;
using System;
using Xunit;

public class BankingDbContextTests
{
    [Fact]
    public void Accounts_ShouldBeEmptyInitially()
    {
        var context = new BankingDbContext();
        var accounts = context.Accounts;
        Assert.NotNull(accounts);
        Assert.Empty(accounts);
    }

    [Fact]
    public void CanAddAccountToAccountsDictionary()
    {
        var context = new BankingDbContext();
        var accountId = Guid.NewGuid().ToString();

        var account = new Account(accountId, 1000m, AccountType.Checking);

        context.Accounts.Add(Guid.Parse(accountId), account);

        Assert.Single(context.Accounts);
        Assert.True(context.Accounts.ContainsKey(Guid.Parse(accountId)));
        Assert.Equal(account, context.Accounts[Guid.Parse(accountId)]);
    }

    [Fact]
    public void AccountsProperty_ReturnsSameDictionaryInstance()
    {
        var context = new BankingDbContext();
        var accounts1 = context.Accounts;
        var accounts2 = context.Accounts;
        Assert.Same(accounts1, accounts2);
    }
}

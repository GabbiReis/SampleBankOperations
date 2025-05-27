using SampleBankOperations.Core.Entities;
using SampleBankOperations.Core.Enums;
using SampleBankOperations.Infrastructure.Persistence;
using SampleBankOperations.Infrastructure.Persistence.Repositories;
using System;
using System.Linq;
using Xunit;

public class AccountRepositoryTests
{
    private Account CreateSampleAccount(string? accountNumber = null)
    {
        accountNumber ??= $"ACC-{new Random().Next(1000, 9999)}";
        return new Account(accountNumber, 1000m, AccountType.Checking);
    }

    [Fact]
    public void Add_ShouldAddAccount()
    {
        var context = new BankingDbContext();
        var repository = new AccountRepository(context);
        var account = CreateSampleAccount();

        repository.Add(account);

        Assert.Single(context.Accounts);
        Assert.Equal(account, context.Accounts[account.AccountId]);
    }

    [Fact]
    public void GetById_ShouldReturnAccount_WhenExists()
    {
        var context = new BankingDbContext();
        var repository = new AccountRepository(context);
        var account = CreateSampleAccount();
        context.Accounts.Add(account.AccountId, account);

        var result = repository.GetById(account.AccountId);

        Assert.NotNull(result);
        Assert.Equal(account, result);
    }

    [Fact]
    public void GetById_ShouldReturnNull_WhenNotExists()
    {
        var context = new BankingDbContext();
        var repository = new AccountRepository(context);

        var result = repository.GetById(Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public void GetAll_ShouldReturnAllAccounts()
    {
        var context = new BankingDbContext();
        var repository = new AccountRepository(context);
        var account1 = CreateSampleAccount();
        var account2 = CreateSampleAccount();

        context.Accounts.Add(account1.AccountId, account1);
        context.Accounts.Add(account2.AccountId, account2);

        var allAccounts = repository.GetAll().ToList();

        Assert.Equal(2, allAccounts.Count);
        Assert.Contains(account1, allAccounts);
        Assert.Contains(account2, allAccounts);
    }

    [Fact]
    public void Update_ShouldReplaceExistingAccount()
    {
        var context = new BankingDbContext();
        var repository = new AccountRepository(context);
        var account = CreateSampleAccount();
        context.Accounts.Add(account.AccountId, account);

        var updatedAccount = new Account(account.AccountNumber, 2000m, AccountType.Checking);
        // Ajusta o AccountId para ser o mesmo do original, para simular atualização
        typeof(Account).GetProperty("AccountId")!.SetValue(updatedAccount, account.AccountId);

        repository.Update(updatedAccount);

        Assert.Equal(2000m, context.Accounts[account.AccountId].Balance);
    }

    [Fact]
    public void Remove_ShouldRemoveAccount()
    {
        var context = new BankingDbContext();
        var repository = new AccountRepository(context);
        var account = CreateSampleAccount();
        context.Accounts.Add(account.AccountId, account);

        repository.Remove(account);

        Assert.Empty(context.Accounts);
    }

    [Fact]
    public void GetByAccountNumber_ShouldReturnAccount_WhenExists()
    {
        var context = new BankingDbContext();
        var repository = new AccountRepository(context);
        var account = CreateSampleAccount("123456");

        context.Accounts.Add(account.AccountId, account);

        var result = repository.GetByAccountNumber("123456");

        Assert.NotNull(result);
        Assert.Equal(account, result);
    }

    [Fact]
    public void GetByAccountNumber_ShouldReturnNull_WhenNotExists()
    {
        var context = new BankingDbContext();
        var repository = new AccountRepository(context);

        var result = repository.GetByAccountNumber("nonexistent");

        Assert.Null(result);
    }
}

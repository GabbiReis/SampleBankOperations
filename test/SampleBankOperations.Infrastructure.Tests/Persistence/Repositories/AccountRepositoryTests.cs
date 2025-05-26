using System;
using System.Linq;
using FluentAssertions;
using SampleBankOperations.Core.Entities;
using SampleBankOperations.Core.Enums;
using SampleBankOperations.Infrastructure.Persistence;
using SampleBankOperations.Infrastructure.Persistence.Repositories;
using Xunit;

namespace SampleBankOperations.Tests.Infrastructure
{
    public class AccountRepositoryTests
    {
        private readonly BankingDbContext _context;
        private readonly AccountRepository _repository;

        public AccountRepositoryTests()
        {
            _context = new BankingDbContext();
            _repository = new AccountRepository(_context);
        }

        [Fact]
        public void Add_ShouldStoreAccount()
        {
            var account = new Account("001", 100, AccountType.Checking);

            _repository.Add(account);

            _context.Accounts.Should().ContainKey(account.AccountId);
        }

        [Fact]
        public void GetById_ShouldReturnCorrectAccount()
        {
            var account = new Account("002", 200, AccountType.Savings);
            _context.Accounts[account.AccountId] = account;

            var result = _repository.GetById(account.AccountId);

            result.Should().Be(account);
        }

        [Fact]
        public void GetAll_ShouldReturnAllAccounts()
        {
            var acc1 = new Account("a1", 100, AccountType.Savings);
            var acc2 = new Account("a2", 200, AccountType.Checking);

            _context.Accounts[acc1.AccountId] = acc1;
            _context.Accounts[acc2.AccountId] = acc2;

            var result = _repository.GetAll().ToList();

            result.Should().Contain(acc1);
            result.Should().Contain(acc2);
        }

        [Fact]
        public void Update_ShouldOverwriteAccount()
        {
            // Arrange
            var account = new Account("003", 100, AccountType.Checking);
            _repository.Add(account);

            // Act
            account.Deposit(999); // ou algum outro método que mude o estado
            _repository.Update(account);

            // Assert
            var updated = _repository.GetById(account.AccountId);
            updated.Balance.Should().Be(1099); // 100 original + 999 depositado
        }

        [Fact]
        public void Remove_ShouldDeleteAccount()
        {
            var account = new Account("004", 300, AccountType.Savings);
            _repository.Add(account);

            _repository.Remove(account);

            _context.Accounts.Should().NotContainKey(account.AccountId);
        }

        [Fact]
        public void GetByAccountNumber_ShouldReturnCorrectAccount()
        {
            var account = new Account("abc123", 400, AccountType.Savings);
            _repository.Add(account);

            var result = _repository.GetByAccountNumber("abc123");

            result.Should().Be(account);
        }

        [Fact]
        public void GetByAccountNumber_ShouldReturnNull_WhenNotFound()
        {
            var result = _repository.GetByAccountNumber("notfound");

            result.Should().BeNull();
        }
    }
}

using System;
using FluentAssertions;
using SampleBankOperations.Core.Entities;
using SampleBankOperations.Core.Enums;
using SampleBankOperations.Infrastructure.Persistence;
using Xunit;

namespace SampleBankOperations.Tests.Infrastructure
{
    public class BankingDbContextTests
    {
        [Fact]
        public void Accounts_ShouldStoreAndRetrieveCorrectly()
        {
            // Arrange
            var context = new BankingDbContext();
            var account = new Account("123", 100, AccountType.Checking);

            // Act
            context.Accounts[account.AccountId] = account;

            // Assert
            context.Accounts.Should().ContainKey(account.AccountId);
            context.Accounts[account.AccountId].Should().Be(account);
        }
    }
}

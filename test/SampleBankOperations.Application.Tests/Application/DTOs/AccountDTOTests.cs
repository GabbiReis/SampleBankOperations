using FluentAssertions;
using SampleBankOperations.Application.DTOs;
using System;
using Xunit;

public class AccountDTOTests
{
    [Fact]
    public void AccountDTO_Should_Set_And_Get_Properties_Correctly()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var accountNumber = "ACC123456";
        var balance = 1500.75m;

        // Act
        var dto = new AccountDTO
        {
            AccountId = accountId,
            AccountNumber = accountNumber,
            Balance = balance
        };

        // Assert
        dto.AccountId.Should().Be(accountId);
        dto.AccountNumber.Should().Be(accountNumber);
        dto.Balance.Should().Be(balance);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SampleBankOperations.Application.DTOs;
using Xunit;

namespace SampleBankOperations.Application.Tests.DTOs
{
    public class AccountDTOTests
    {
        [Fact]
        public void Should_Create_AccountDTO_With_Valid_Values()
        {
            // Arrange
            var id = Guid.NewGuid();
            var accountNumber = "123456";
            var balance = 1000m;

            // Act
            var dto = new AccountDTO
            {
                AccountId = id,
                AccountNumber = accountNumber,
                Balance = balance
            };

            // Assert
            Assert.Equal(id, dto.AccountId);
            Assert.Equal(accountNumber, dto.AccountNumber);
            Assert.Equal(balance, dto.Balance);
        }
    }
}

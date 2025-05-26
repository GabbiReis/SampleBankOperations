using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using SampleBankOperations.Core.Services.Validations;

namespace SampleBankOperations.Core.Tests.Services.Validations
{
    public class AccountPredicateTests
    {
        [Theory]
        [InlineData(1000, 500, true)]
        [InlineData(500, 500, true)]
        [InlineData(400, 500, false)]
        public void HasSufficientBalance_Should_Return_Correct_Result(decimal balance, decimal amount, bool expected)
        {
            // Act
            var result = AccountPredicate.HasSufficientBalance(balance, amount);

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(1000, 100, true)]
        [InlineData(100, 100, true)]
        [InlineData(50, 100, false)]
        public void IsBalanceAboveMinimum_Should_Return_Correct_Result(decimal balance, decimal minimumBalance, bool expected)
        {
            // Act
            var result = AccountPredicate.IsBalanceAboveMinimum(balance, minimumBalance);

            // Assert
            Assert.Equal(expected, result);
        }
    }
}
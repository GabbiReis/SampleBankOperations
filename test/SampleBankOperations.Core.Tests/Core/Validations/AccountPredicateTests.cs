using SampleBankOperations.Core.Services.Validations;
using Xunit;
using FluentAssertions;

public class AccountPredicateTests
{
    [Theory]
    [InlineData(500, 200, true)]
    [InlineData(200, 200, true)]
    [InlineData(100, 200, false)]
    public void HasSufficientBalance_ShouldReturnExpectedResult(decimal balance, decimal amount, bool expectedResult)
    {
        // Act
        var result = AccountPredicate.HasSufficientBalance(balance, amount);

        // Assert
        result.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData(500, 100, true)]
    [InlineData(100, 100, true)]
    [InlineData(50, 100, false)]
    public void IsBalanceAboveMinimum_ShouldReturnExpectedResult(decimal balance, decimal minimumBalance, bool expectedResult)
    {
        // Act
        var result = AccountPredicate.IsBalanceAboveMinimum(balance, minimumBalance);

        // Assert
        result.Should().Be(expectedResult);
    }
}

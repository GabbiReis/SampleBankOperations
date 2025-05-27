using FluentAssertions;
using SampleBankOperations.Application.Validations;
using Xunit;

public class AccountValidatorTests
{
    [Theory]
    [InlineData(100, 50, false)]   // 50 < 100 -> false
    [InlineData(100, 100, true)]   // 100 >= 100 -> true
    [InlineData(100, 150, true)]   // 150 > 100 -> true
    public void MinimumBalanceValidator_ShouldValidateCorrectly(decimal minimumBalance, decimal currentBalance, bool expectedResult)
    {
        // Arrange
        var validator = AccountValidator.MinimumBalanceValidator(minimumBalance);

        // Act
        bool result = validator(currentBalance);

        // Assert
        result.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData(150, 200, false)]  // 150 < 200 -> false
    [InlineData(200, 200, true)]   // 200 >= 200 -> true
    [InlineData(250, 200, true)]   // 250 >= 200 -> true
    public void RequestedAmountValidator_ShouldValidateCorrectly(decimal balance, decimal requestedAmount, bool expectedResult)
    {
        // Arrange
        var validator = AccountValidator.RequestedAmountValidator(requestedAmount);

        // Act
        bool result = validator(balance);

        // Assert
        result.Should().Be(expectedResult);
    }
}

namespace SampleBankOperations.Application.Validations;

public static class AccountValidator
{
    public static Predicate<decimal> MinimumBalanceValidator(decimal minimumBalance)
    {
        return balance => balance >= minimumBalance;
    }

    public static Predicate<decimal> RequestedAmountValidator(decimal requestedAmount)
    {
        return balance => balance >= requestedAmount;
    }
}

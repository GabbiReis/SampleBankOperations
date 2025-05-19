namespace SampleBankOperations.Core.Services.Validations;

public static class AccountPredicate
{
    public static bool HasSufficientBalance(decimal balance, decimal amount)
    {
        return balance >= amount;
    }

    public static bool IsBalanceAboveMinimum(decimal balance, decimal minimumBalance)
    {
        return balance >= minimumBalance;
    }
}

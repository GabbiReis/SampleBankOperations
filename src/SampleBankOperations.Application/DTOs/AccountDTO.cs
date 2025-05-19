namespace SampleBankOperations.Application.DTOs;

public class AccountDTO
{
    public Guid AccountId { get; set; }
    public required string AccountNumber { get; set; }
    public decimal Balance { get; set; }
}

namespace River.API.DTOs.Wallet;


public class WalletDto
{
    public required string AccountNumber { get; set; }
    public required decimal Balance { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string PhoneNumber { get; set; }
}
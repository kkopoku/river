using System.ComponentModel.DataAnnotations;


namespace River.API.DTOs.Transfer;

public class ReverseTransferDto {

    [Required(ErrorMessage = "Transaction Id is required")]
    public required string TransactionId { get; set; }

    public bool? IsReversed { get; set; }

}
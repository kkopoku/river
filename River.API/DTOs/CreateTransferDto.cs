using System.ComponentModel.DataAnnotations;


namespace River.API.DTOs;

public class CreateTransferDto {

    [Required(ErrorMessage = "From Account Number is required")]
    public required string FromAccountNumber { get; set; }


    [Required(ErrorMessage = "To Account Number is required")]
    public required string ToAccountNumber { get; set; }


    [Required(ErrorMessage = "Amount is required")]
    public required decimal Amount { get; set; }

}
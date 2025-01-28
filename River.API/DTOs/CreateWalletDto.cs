using System.ComponentModel.DataAnnotations;


namespace River.API.DTOs
{
    public class CreateWalletDto
    {

        [Required(ErrorMessage = "Account Number is required")]
        public required string AccountNumber { get; set; }


        [Required(ErrorMessage = "First Name is required")]
        public required string FirstName { get; set; }


        [Required(ErrorMessage = "Last Name is required")]
        public required string LastName { get; set; }


        public string? Email { get; set; }


        public string? PhoneNumber { get; set; }

    }
}

using System.ComponentModel.DataAnnotations;

namespace DatingApp.API.Dtos
{
    //Data transfer object - send data from Client JSON object to API
    public class UserForRegisterDto
    {
        [Required]
        public string Username { get; set; }
        [Required]
        [StringLength(16, MinimumLength = 6, ErrorMessage = "You must specify password between 6 and 16 characters.")]
        public string Password { get; set; }
    }
}
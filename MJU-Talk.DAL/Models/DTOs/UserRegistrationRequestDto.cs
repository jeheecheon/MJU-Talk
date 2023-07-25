
using System.ComponentModel.DataAnnotations;

namespace MJU_Talk.DAL.Models.DTOs;

public class UserRegistrationRequestDto
{
    [Required]
    public string Name { get; set; }    

    [Required]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }

}

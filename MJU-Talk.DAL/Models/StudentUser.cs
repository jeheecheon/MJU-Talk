using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace MJU_Talk.DAL.Models;

public class StudentUser : IdentityUser
{
    [Required]
    [DataType(DataType.Text)]
    [StringLength(20, ErrorMessage = "Major name cannot exceed 20 characters...")]
    [PersonalData]
    public string Major { get; set; } = string.Empty;

    // true represents male, false represents female. Null value for those who don't want to reveal their real genders
    // public bool? Gender { get; set; } = default(bool?);

    [Required]
    [PersonalData]
    public short AdmissionYear { get; set; }
}

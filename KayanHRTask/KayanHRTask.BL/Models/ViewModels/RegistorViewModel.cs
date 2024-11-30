using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KayanHRTask.BL.Models.ViewModels
{
    public class RegistorViewModel
    {
        [DataType(DataType.Text)]
        [Display(Name = "First Name")]
        public string? FirstName { get; set; }
        [DataType(DataType.Text)]
        [Display(Name = "Last Name")]
        public string? LastName { get; set; }
        [Required(ErrorMessage = "Please Enter Email")]
        [MaxLength(40, ErrorMessage = "max length 40 char")]
        [EmailAddress(ErrorMessage = "Example: user@gmail.com")]
        public string? Email { set; get; }
        [Required(ErrorMessage = "Please enter password")]
        [DataType(DataType.Password)]
        public string? Password { set; get; }
        [Required(ErrorMessage = "Please confirm password")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Miss Match")]
        public string? ConfirmPassword { set; get; }
        public IFormFile Image { get; set; }
    }
}

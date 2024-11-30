using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KayanHRTask.BL.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Please Enter Email")]
        [MaxLength(40, ErrorMessage = "max length 40 char")]
        public string? Email { set; get; }
        [Required(ErrorMessage = "Please enter password")]
        [DataType(DataType.Password)]
        public string? Password { set; get; }
        public bool RememberMe { get; set; }
    }
}

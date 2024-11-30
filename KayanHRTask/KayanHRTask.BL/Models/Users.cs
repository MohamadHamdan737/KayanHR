using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KayanHRTask.BL.Models
{
    public class Users
    {
        public int UsersId { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set;}
        public string? Password { get; set;}
        public string? SaltPassword { get; set;}
        public string? ImageName { get; set; }
        public bool IsDelete { get; set; }

    }
}

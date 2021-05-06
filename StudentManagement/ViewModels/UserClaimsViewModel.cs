using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StudentManagement.Models;

namespace StudentManagement.ViewModels
{
    public class UserClaimsViewModel
    {
        public string UserId { get; set; }
        public List<UserClaim> Cliams { get; set; }
        public UserClaimsViewModel()
        {
            Cliams = new List<UserClaim>();
        }

    }
}

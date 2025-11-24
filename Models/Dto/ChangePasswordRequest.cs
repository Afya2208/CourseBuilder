using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Models.Dto
{
    public class ChangePasswordRequest
    {
        public long UserId { get; set; }
        public string Password { get; set; }
    }
}
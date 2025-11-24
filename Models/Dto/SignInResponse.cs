using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Models.Dto
{
    public class SignInResponse : BaseResponse
    {
        public string Token { get; set; }
        public string? LastName { get; set; }
        public string? MiddleName { get; set; }
        public string? FirstName { get; set; }
        public string? Position { get; set; }
        public int RoleId{ get; set; }
        public string RoleName{ get; set; }
        public long UserId { get; set; }
    }
}
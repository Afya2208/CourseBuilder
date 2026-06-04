using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Models.Dto
{
    public class SignInResponse : BaseResponse
    {
        public string Token { get; set; }
        public UserDto User { get; set; }
    }
}
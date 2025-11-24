using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Models.Dto
{
    public class AddUserRequest
    {
        public string Email {get; set;}
        public string Password {get; set;}
        public int RoleId {get; set;}

        public string? LastName {get; set;}
        public string? MiddleName {get; set;}
        public string? FirstName {get; set;}
        public string? Phone {get; set;}
        public string? Position {get; set;}
    }
}
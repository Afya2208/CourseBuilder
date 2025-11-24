using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Models.Dto;

namespace API.Interface
{
    public interface IAuthService
    {
        Task<SignInResponse> SignInAsync(SignInRequest request);
    }
}
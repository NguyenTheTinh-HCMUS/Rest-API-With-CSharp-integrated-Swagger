using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAplicationAPI1.Contracts.V1.Responses
{
    public class AuthSuccessResopnse
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
}

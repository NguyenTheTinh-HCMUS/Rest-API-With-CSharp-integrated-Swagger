using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAplicationAPI1.Filters;

namespace WebAplicationAPI1.Controllers.V1
{
    [ApiKeyAuth]
    public class SecretController: ControllerBase
    {
        [HttpGet("secret")]
        public IActionResult Getsecret()
        {
            return Ok("I have no secrets");
        }
    }
}

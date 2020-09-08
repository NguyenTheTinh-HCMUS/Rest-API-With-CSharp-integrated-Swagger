using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAplicationAPI1.Contracts.V1.Requests
{
    public class CreatePostRequest
    {
        public String   Name{ get; set; }
        public string[] Tags { get; set; }
    }
}

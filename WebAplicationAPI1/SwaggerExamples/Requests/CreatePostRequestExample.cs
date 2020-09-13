using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAplicationAPI1.Contracts.V1.Requests;

namespace WebAplicationAPI1.SwaggerExamples.Requests
{
    public class CreatePostRequestExample : IExamplesProvider<CreatePostRequest>
    {
        public CreatePostRequest GetExamples()
        {
            return new CreatePostRequest { Name = "Post Example" ,
                Tags=new string[]{"Tag1","Tag2"}
            };
        }
    }
}

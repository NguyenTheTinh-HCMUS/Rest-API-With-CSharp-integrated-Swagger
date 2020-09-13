using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAplicationAPI1.Contracts.V1.Requests;

namespace WebAplicationAPI1.Validators
{
    public class CreateTagRequestValidator: AbstractValidator<CreatePostRequest>
    {
        public CreateTagRequestValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
        }
    }
}

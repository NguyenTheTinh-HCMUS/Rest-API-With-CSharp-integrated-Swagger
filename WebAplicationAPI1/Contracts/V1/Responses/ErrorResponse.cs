﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAplicationAPI1.Contracts.V1.Responses
{
    public class ErrorResponse
    {
        public List<ErrorModel> Errors { get; set; }
       
    }
}

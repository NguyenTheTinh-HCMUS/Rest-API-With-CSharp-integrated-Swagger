using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TweetBooks.Contracts.Contracts.V1.Requests.querries;
using WebAplicationAPI1.Domain;

namespace WebAplicationAPI1.MappingProfiles
{
    public class RequestToDoaminProfile:Profile
    {
        public RequestToDoaminProfile()
        {
            CreateMap<PaginationQuerry, PaginationFilter>();

        }
    }
}

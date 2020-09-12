using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAplicationAPI1.Contracts.V1.Responses;
using WebAplicationAPI1.Domain;

namespace WebAplicationAPI1.MappingProfiles
{
    public class DomainToResponseProfile: Profile
    {
        public DomainToResponseProfile()
        {
            CreateMap<Post, PostResponse>()
                    .ForMember(dest => dest.Tags,opt=> {
                        opt.MapFrom(src => src.Tags.Select(x => new TagResponse { Name = x.TagName }));
                    });
        }
    }
}

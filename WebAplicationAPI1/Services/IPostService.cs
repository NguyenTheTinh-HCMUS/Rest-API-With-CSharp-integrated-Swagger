using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAplicationAPI1.Domain;

namespace WebAplicationAPI1.Services
{
     public interface IPostService
    {
        List<Post> GetAll();
        Post Get(Guid postId);
        bool UpdatePost(Post updatePost);
        bool DeletePost(Guid postId);
    }
}

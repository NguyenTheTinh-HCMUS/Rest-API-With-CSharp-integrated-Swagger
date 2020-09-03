using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAplicationAPI1.Domain;

namespace WebAplicationAPI1.Services
{
    public class PostService : IPostService
    {
        private readonly List<Post> _posts;
        public PostService()
        {
            _posts = new List<Post>();
            for (int i = 0; i < 5; i++)
            {
                _posts.Add(new Post { Id = Guid.NewGuid(), Name = $"Post {i}" });

            }
        }

        public bool DeletePost(Guid postId)
        {
            var post = Get(postId);
            if (post == null)
            {
                return false;
            }
            _posts.Remove(post);
            return true;
        }

        public Post Get(Guid postId)
        {
            return _posts.SingleOrDefault(x => x.Id == postId);
        }

        public List<Post> GetAll()
        {
            return _posts;
        }

        public bool UpdatePost(Post updatePost)
        {
            var exists=Get(updatePost.Id)!=null;
            if (!exists)
            {
                return false;
            }
            var index = _posts.FindIndex(x => x.Id == updatePost.Id);
            _posts[index] = updatePost;
            return true;

        }
    }
}

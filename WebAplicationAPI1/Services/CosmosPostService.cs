using Cosmonaut;
using Cosmonaut.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAplicationAPI1.Domain;

namespace WebAplicationAPI1.Services
{
    public class CosmosPostService : IPostService
    {
        private readonly ICosmosStore<CosmosPostDto> _cosmosStore;
        public  CosmosPostService(ICosmosStore<CosmosPostDto> cosmosStore)
        {
            _cosmosStore = cosmosStore;

        }
        public async Task<bool> Create_Async(Post post)
        {
            var cosmosPost = new CosmosPostDto { Id=post.Id.ToString(),Name=post.Name};
            var response= await _cosmosStore.AddAsync(cosmosPost);
            return response.IsSuccess;

        }

        public async Task<bool> DeletePost_Async(Guid postId)
        {
            var cosmoPost = await _cosmosStore.FindAsync(postId.ToString());
            var response = await _cosmosStore.RemoveAsync(cosmoPost);
            return response.IsSuccess;
        }

        public Task<List<Tag>> GetAllTags_Async()
        {
            throw new NotImplementedException();
        }

        public async Task<List<Post>> GetAll_Async()
        {
            var posts = await _cosmosStore.Query().ToListAsync();
            return posts.Select(x => new Post { Id = Guid.Parse(x.Id), Name = x.Name }).ToList();
        }

        public async Task<Post> Get_Async(Guid postId)
        {
            var post = await _cosmosStore.FindAsync(postId.ToString());
            if (post == null)
            {
                return null;
            }
            return new Post { Id=Guid.Parse(post.Id),Name=post.Name};
        }

        public async Task<bool> UpdatePost_Async(Post updatePost)
        {
            var cosmoPost = new CosmosPostDto { Id=updatePost.Id.ToString(),Name=updatePost.Name};
            var response = await _cosmosStore.UpdateAsync(cosmoPost);
            return response.IsSuccess;

        }

        public Task<bool> UserOwnsPos_Async(Guid postId, string UserID)
        {
            throw new NotImplementedException();
        }
    }
}

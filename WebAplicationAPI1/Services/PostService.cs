using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAplicationAPI1.Data;
using WebAplicationAPI1.Domain;

namespace WebAplicationAPI1.Services
{
    public class PostService : IPostService
    {
        private readonly DataContext _dataContext;
        #region Functions
        private async Task AddNewTags(Post post)
        {
            foreach (var tag in post.Tags)
            {
                var existingTag =
                    await _dataContext.Tags.SingleOrDefaultAsync(x =>
                        x.Name == tag.TagName.ToUpper());
                if (existingTag != null)
                    continue;

                await _dataContext.Tags.AddAsync(new Tag
                { Name = tag.TagName.ToUpper(), CreatedOn = DateTime.UtcNow, CreatorId = post.UserId });
            }
        }
        #endregion
        public PostService(DataContext dataContext)
        {
            _dataContext = dataContext;
           
        }

        public async Task<bool> DeletePost_Async(Guid postId)
        {
            var post = await Get_Async(postId);
            _dataContext.Posts.Remove(post);
            return  await _dataContext.SaveChangesAsync()>0;
        }

        public async Task<Post> Get_Async(Guid postId)
        {
            return await _dataContext.Posts.Include(x=>x.Tags).SingleOrDefaultAsync(x => x.Id == postId);
        }

        public async Task<List<Post>> GetAll_Async()
        {
            return await _dataContext.Posts.Include(x=>x.Tags).ToListAsync();
        }

        public async Task<bool> UpdatePost_Async(Post updatePost)
        {
            _dataContext.Posts.Update(updatePost);

            return await _dataContext.SaveChangesAsync()>0;

        }
        public async Task<bool>  Create_Async(Post post)
        {
            await _dataContext.Posts.AddAsync(post);
            await AddNewTags(post);
            return await _dataContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> UserOwnsPos_Async(Guid postId, string UserID)
        {
            var post = await _dataContext.Posts.AsNoTracking().SingleOrDefaultAsync(x => x.Id == postId && x.UserId == UserID);
            if (post != null) { return true; }
            return false;
        }

        public async Task<List<Tag>> GetAllTags_Async()
        {
            return await _dataContext.Tags.ToListAsync(); 
        }
    }
}

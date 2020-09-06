﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAplicationAPI1.Domain;

namespace WebAplicationAPI1.Services
{
     public interface IPostService
    {
        Task<List<Post>> GetAll_Async();
        Task<Post> Get_Async(Guid postId);
        Task<bool> UpdatePost_Async(Post updatePost);
        Task<bool> DeletePost_Async(Guid postId);
        Task<bool> Create_Async(Post post);
        Task<bool> UserOwnsPos_Async(Guid postId,string UserID);
    }
}
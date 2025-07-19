using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Connect.Models;

namespace Connect.Utilities.Service.IService
{
    public interface IPostService
    {
        Task<Post> GetPostById(int postId);
        Task<List<Post>> ProcessPosts(List<Post> posts);
        Task<Post> ProcessPosts(Post post);


    }
}

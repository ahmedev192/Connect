using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Connect.Domain;

namespace Connect.Application.Interfaces
{
    public interface IPostService
    {
        Task<Post> GetPostById(int postId);

        Task CreatePostAsync(Post post);
        Task<ServiceResult> DeletePostAsync(int postId, int userId);
        Task<List<Post>> ProcessPosts(IEnumerable<Post> posts);
        Task<Post> ProcessPosts(Post post);


    }
}

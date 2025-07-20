using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Connect.Models;

namespace Connect.Utilities.Service.IService
{
    public interface IUsersService
    {
        Task<User> GetUser(int loggedInUserId);
        Task<List<Post>> GetPostsByUserId(int userId = 0 );


    }
}

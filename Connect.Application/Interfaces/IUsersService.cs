using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Connect.Domain.Entities;
using Connect.Domain.Entities;

namespace Connect.Application.Interfaces
{
    public interface IUsersService
    {
        Task<User> GetUser(int loggedInUserId);
        Task<List<Post>> GetPostsByUserId(int userId = 0 );


    }
}

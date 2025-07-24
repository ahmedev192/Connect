using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Connect.Infrastructure.Migrations;

namespace Connect.Application.Dtos
{
    public class UserProfileDto
    {
        public UserDto User { get; set; }
        public List<PostDto> Posts { get; set; }
    }
}

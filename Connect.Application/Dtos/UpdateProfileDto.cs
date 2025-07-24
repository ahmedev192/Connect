using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connect.Application.Dtos
{
    public class UpdateProfileDto
    {
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string Bio { get; set; }
    }
}

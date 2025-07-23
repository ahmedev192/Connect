using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connect.Application.Interfaces
{
    public  interface IHashtagService
    {
        List<string> ExtractHashtags(string postText);
    }
}

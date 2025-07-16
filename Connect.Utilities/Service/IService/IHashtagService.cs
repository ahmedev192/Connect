using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connect.Utilities.Service.IService
{
    public  interface IHashtagService
    {
        List<string> ExtractHashtags(string postText);
    }
}

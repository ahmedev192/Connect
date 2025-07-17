using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connect.Utilities.StaticDetails
{
    public class ApplicationRoles
    {
        public const string Admin = "Admin";
        public const string User = "User";

        public static readonly IReadOnlyList<string> All = new[] { Admin, User };
    }
}

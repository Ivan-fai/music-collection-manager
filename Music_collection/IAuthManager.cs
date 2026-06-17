using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Music_collection
{
    public interface IAuthManager
    {
        User? SignIn(string login, string password);

        bool SignUp(string login, string password);
    }
}

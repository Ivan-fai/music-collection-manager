using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Music_collection
{
    public class User
    {
        public int UserId { get; private set; }
        public string UserName { get; private set; }
        public bool IsAdmin{ get; private set; }
        public User(int _userId, string _userName, bool _isAdmin)
        {
            UserId = _userId;
            UserName = _userName;
            IsAdmin = _isAdmin;
        }
    }
}

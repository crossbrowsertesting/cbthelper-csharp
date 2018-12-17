using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cbthelper
{

    public class Globals
    {

        public static string username;
        public static string authkey;

        public Globals(string UserName, string AuthKey)
        {
            username = UserName;
            authkey = AuthKey;
        }
    }
}

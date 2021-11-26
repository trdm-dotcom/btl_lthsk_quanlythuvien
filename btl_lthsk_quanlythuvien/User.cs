using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace btl_lthsk_quanlythuvien
{
    public class User
    {
        private string name;
        private string iduser;
        private int type;
        public User()
        {

        }
        public User(string name, string iduser, int type)
        {
            this.name = name;
            this.iduser = iduser;
            this.type = type;
        }

        public int Type { get => type; set => type = value; }
        public string Iduser { get => iduser; set => iduser = value; }
        public string Name { get => name; set => name = value; }
    }
}

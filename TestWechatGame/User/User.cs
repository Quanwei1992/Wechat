using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestWechatGame
{
    public class User
    {
        public string Name { get; private set; }
        public Int64 ID { get; private set; }

        public User(string name,Int64 id)
        {
            this.Name = name;
            this.ID = id;
        }
    }
}

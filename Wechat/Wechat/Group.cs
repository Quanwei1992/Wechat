using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wechat
{
    public class Group : Contact
    {
        public Contact[] Members { get; set; }
    }
}

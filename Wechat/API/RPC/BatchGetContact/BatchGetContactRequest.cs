using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wechat.API.RPC
{

    public class BatchGetContactRequest
    {
        public BaseRequest BaseRequest;
        public int Count;
        public User[] List;
    }
}

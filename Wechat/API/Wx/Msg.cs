using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wechat.API
{
    public class Msg
    {
        public long ClientMsgId;
        public long LocalID;
        public string Content;
        public string FromUserName;
        public string ToUserName;
        public int Type;
    }
}

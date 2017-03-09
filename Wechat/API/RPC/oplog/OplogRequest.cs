using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wechat.API.RPC
{
    public class OplogRequest
    {
        public BaseRequest BaseRequest;
        public int CmdId;
        public int OP;
        public string UserName;
        public string RemarkName;// 设置备注名，CmdId == 2时可用
    }
}

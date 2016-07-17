using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wechat.API.RPC
{
    public class InitResponse
    {
        public BaseResponse BaseResponse;
        public int Count;
        public User[] ContactList;
        public SyncKey SyncKey;
        public User User;
        public string ChatSet;
        public string SKey;
        public long ClientVersion;
        public long SystemTime;
        public int GrayScale;
        public int InviteStartCount;
        public int MPSubscribeMsgCount;
        //public MPSubscribeMsg[] MPSubscribeMsgList;
        public int ClickReportInterval;

    }
}

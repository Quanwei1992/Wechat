using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wechat.API.RPC
{
    public class SyncResponse
    {
        public BaseResponse BaseResponse;
        public int AddMsgCount;
        public Msg[] AddMsgList;
        public int ContinueFlag;
        public int DelContactCount;
        //public Contanct[] DelContactList;
        public int ModChatRoomMemberCount;
        //public ChatRoomMember[] ModChatRoomMemberList;
        public int ModContactCount;
        //public Contact[] ModContactList;
        public string Skey;
        public SyncKey SyncKey;
    }
}

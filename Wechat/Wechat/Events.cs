using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wechat
{

    public class WeChatClientEvent : EventArgs { }


    public class GetQRCodeImageEvent : WeChatClientEvent
    {
        public Image QRImage;
    }

    public class UserScanQRCodeEvent : WeChatClientEvent
    {
        public Image UserAvatarImage;
    }

    public class LoginSucessEvent : WeChatClientEvent
    {

    }

    public class InitedEvent : WeChatClientEvent
    {

    }

    public class AddMessageEvent:WeChatClientEvent
    {
        public Message Msg;
    }

    public class StatusChangedEvent:WeChatClientEvent
    {
        public ClientStatusType FromStatus;
        public ClientStatusType ToStatus;
    }
}

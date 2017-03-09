using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestWechatGame.Frameworks
{

    public class GetQRCodeImageEvent : EventArgs
    {
        public Image QRImage;
    }

    public class UserScanQRCodeEvent : EventArgs
    {
        public Image UserAvatarImage;
    }

    public class LoginSucessEvent : EventArgs
    {

    }

    public class InitComplateEvent : EventArgs
    {

    }
}

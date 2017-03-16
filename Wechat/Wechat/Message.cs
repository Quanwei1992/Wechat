using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wechat
{
   public class Message
   {
        public string FromContactID;
        public string ToContactD;
   }


    public class TextMessage : Message
    {
        public string Content;
    }

    public class DefaultMessage: Message
    {
        public int MsgType;
        public string Content;
    }

    public class MessageFactory
    {
        public static Message CreateMessage(Wechat.API.AddMsg msg)
        {
            //MsgType
            //1   文本消息
            //3   图片消息
            //34  语音消息
            //37  VERIFYMSG
            //40  POSSIBLEFRIEND_MSG
            //42  共享名片
            //43  视频通话消息
            //47  动画表情
            //48  位置消息
            //49  分享链接
            //50  VOIPMSG
            //51  微信初始化消息
            //52  VOIPNOTIFY
            //53  VOIPINVITE
            //62  小视频
            //9999    SYSNOTICE
            //10000   系统消息
            //10002   撤回消息

            Message ret = null;

            if (msg.MsgType == 1) {
                ret = new TextMessage() {
                    Content = msg.Content
                };
                
            }

            if (ret == null)
            {
                ret = new DefaultMessage()
                {
                    MsgType = msg.MsgType,
                    Content = msg.Content
                };
            }
            ret.FromContactID = msg.FromUserName;
            ret.ToContactD = msg.ToUserName;
            return ret;
        }
    }
}

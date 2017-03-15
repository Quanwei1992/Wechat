using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wechat;
namespace TestWechatGame
{
    public class Robot
    {
        private WeChatClient mClient;
        private string mGroupID;
        private bool IsRunning = false;

        public Robot(Wechat.WeChatClient client)
        {
            mClient = client;
             
        }


        public void Run(string group)
        {
            mGroupID = group;
            mClient.OnEvent += OnWechatEvent;
            IsRunning = true;
        }

        public void Stop()
        {
            mClient.OnEvent -= OnWechatEvent;
            IsRunning = false;
        }

        private void OnWechatEvent(WeChatClient sender, WeChatClientEvent e)
        {
            if (!IsRunning) return;
            if (e is AddMessageEvent)
            {
                var addMsgEvent = e as AddMessageEvent;
                if (addMsgEvent.Msg is TextMessage)
                {
                    var textMsg = addMsgEvent.Msg as TextMessage;
                    if (textMsg.ToIContactD == mGroupID)
                    {
                        HandleGroupMessage(textMsg);
                    }
                }
            }
        }

        private void HandleGroupMessage(TextMessage msg)
        {
            if (!IsRunning) return;
            if (msg.Content.Contains("注册用户"))
            {
                var user = UserManager.CreateUser("1111");
                if (user != null)
                {
                    mClient.SendMsg(mGroupID, "注册成功，您的ID是:" + user.ID);
                }
            }
        }
    }
}

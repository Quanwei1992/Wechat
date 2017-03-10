using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestWechatGame.Frameworks;
using Wechat.API;

namespace TestWechatGame.PKTEN
{
    public class PKTENApp : WeChatApp
    {
        private bool IsRunning = false;
        private User mGroup = null;
        private Dictionary<string, Wechat.API.User> mGroups = new Dictionary<string, Wechat.API.User>();
        public PKTENApp(Action<WeChatApp, EventArgs> eventCallback) : base(eventCallback)
        {
        }


        
        public User[] Groups
        {
            get {
                return mGroups.Values.ToArray();
            }
        }

        


        public void RunGame(string groupUserName)
        {
            mGroup = mGroups[groupUserName];
            IsRunning = true;
        }

        public void StopGame()
        {
            mGroup = null;
            IsRunning = false;
        }

        float lastUpdateTime = 0.0f;
        int i = 0;
        protected override void Update()
        {
            base.Update();
            if (IsRunning && mGroup!= null) {
                if (Time - lastUpdateTime > 5.0f) {
                    lastUpdateTime = Time;
                    //Console.WriteLine(":" + i++);
                    //Client.SendMsg(mGroup.UserName, (i++).ToString());
                }
            }
        }

        protected override void WeChatInitComplate()
        {
            base.WeChatInitComplate();
            foreach (var username in Client.ContactList) {
                if (username.StartsWith("@@")) {
                    var user = Client.GetContact(username);
                    mGroups[user.UserName] = user;
                }
            }
        }

        protected override void HandleMessage(AddMsg msg)
        {
            base.HandleMessage(msg);
            if (IsRunning && mGroup != null)
            {
                if (msg.ToUserName == mGroup.UserName) {
                    if (msg.MsgType == 1) { //文本消息
                        HandleGroupMessage(msg.FromUserName, msg.Content);
                    }
                }
            }
        }

        private void HandleGroupMessage(string userName, string msg)
        {
            Console.WriteLine(string.Format("UserName:{0} Msg:{1}", userName,msg));
            if (!msg.StartsWith("Simsimi:")) {
                var rep = WechatTest.Simsimi.say(msg);
                Client.SendMsg(mGroup.UserName, "Simsimi:" + rep.respSentence);
            }

        }


    }
}

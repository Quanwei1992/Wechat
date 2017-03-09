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

        protected override void HandleMessage(AddMsg msg)
        {
            base.HandleMessage(msg);
        }

        
        public User[] Groups
        {
            get {
                return mGroups.Values.ToArray();
            }
        }

        


        public void RunGame(User group)
        {
            mGroup = group;
            IsRunning = true;
        }

        protected override void Update()
        {
            base.Update();
            if (IsRunning && mGroup!= null) {

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
    }
}

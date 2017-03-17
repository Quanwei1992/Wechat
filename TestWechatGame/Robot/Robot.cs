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
        private const long DEBUGUID = 80000000;

        public Robot(Wechat.WeChatClient client)
        {
            mClient = client;
             
        }


        public void Run(string group)
        {
            mGroupID = group;
            mClient.OnEvent += OnWechatEvent;
            IsRunning = true;
            mClient.SendMsg(mGroupID, "_________start robot");
        }

        public void Stop()
        {
            mClient.OnEvent -= OnWechatEvent;
            IsRunning = false;
            mClient.SendMsg(mGroupID, "_________stop robot");
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
                    if (textMsg.FromContactID == mClient.Self.ID)
                    {
                        if (textMsg.ToContactD == mGroupID) {
                            HandleGroupMessage(textMsg);
                        }
                    }
                    else if (textMsg.FromContactID == mGroupID && textMsg.ToContactD == mClient.Self.ID)
                    {
                        //@c9951cbb490a69bc575061677c724382:<br/>烙饼配酸奶
                        string[] param = textMsg.Content.Split(':');
                        if (param.Length == 2) {
                            textMsg.FromContactID = param[0];
                            textMsg.ToContactD = mGroupID;
                            textMsg.Content = param[1].Remove(0, 5);
                        }
                        HandleGroupMessage(textMsg);
                    }
                }
            }
        }


        private long ReadMemeberID(Contact contact)
        {
            if (contact.ID == mClient.Self.ID) return DEBUGUID;
            if (contact.RemarkName != null && contact.RemarkName.Contains("(#") && contact.RemarkName.EndsWith(") ")){
                int startIndex = contact.RemarkName.LastIndexOf("(#");
                string id = contact.RemarkName.Substring(startIndex + 2, contact.RemarkName.Length - startIndex-2);
                long uid = 1;
                long.TryParse(id, out uid);
                return uid;
            }
            return -1;
        }

        private bool SetMemberID(Contact contact,long id)
        {
            string idstr = string.Format("{0}(#{1}) ",contact.NickName,id.ToString());
            if (mClient.SetRemarkName(contact.ID, idstr)) {
                contact.RemarkName = idstr;
                return true;
            }
            return false;
        }


        private void HandleGroupMessage(TextMessage msg)
        {
            if (!IsRunning) return;
            var group = mClient.GetGroup(mGroupID);
            var member = group.GetMember(msg.FromContactID);
            var cmd = CommandFactory.Parse(msg.Content,member);
            if (cmd != null) {
                cmd.Execute(this);
            }

        }


        public User GetUser(Contact contact)
        {
            long uid = ReadMemeberID(contact);
            if (uid < 0)
            {
                var user = UserManager.CreateUser(contact.NickName);
                if (user != null && SetMemberID(contact, user.ID))
                {
                    return user;
                }
                else {
                    return null;
                }
            }
            return UserManager.GetUser(uid);
        }

        public void SendMessageToGroup(string message)
        {
            mClient.SendMsg(mGroupID, message);
        }
    }
}

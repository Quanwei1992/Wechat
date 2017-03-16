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
            mClient.SendMsg(mGroupID, "开始游戏");
        }

        public void Stop()
        {
            mClient.OnEvent -= OnWechatEvent;
            IsRunning = false;
            mClient.SendMsg(mGroupID, "停止游戏");
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

        private void HandleGroupMessage(TextMessage msg)
        {
            if (!IsRunning) return;
            if (msg.Content.Contains("注册用户"))
            {
                var group = mClient.GetGroup(mGroupID);
                var member = group.GetMember(msg.FromContactID);

                if (member.RemarkName == null || !member.RemarkName.StartsWith("UID:"))
                {
                    var user = UserManager.CreateUser(member.NickName);
                    if (user != null)
                    {
                        string remark = "UID:" + user.ID.ToString();
                        if (mClient.SetRemarkName(msg.FromContactID, remark))
                        {
                            member.RemarkName = remark;
                            mClient.SendMsg(mGroupID, "注册成功，您的ID是:" + user.ID);
                        }
                    }
                }
                else
                {
                    mClient.SendMsg(mGroupID, "您已经注册过了，" + member.RemarkName);
                }
            }
            else {
                mClient.SendMsg(mGroupID, msg.Content);
            }
        }
    }
}

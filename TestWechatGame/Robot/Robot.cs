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
        public Lottery.LotteryCenter Lottery;
        private string mGroupID;
        private bool IsRunning = false;
        private const long DEBUGUID = 80000000;

        public Robot(Wechat.WeChatClient client,Lottery.LotteryCenter lottery)
        {
            mClient = client;
            Lottery = lottery;
        }


        public void Run(string group)
        {
            mGroupID = group;
            mClient.OnEvent += OnWechatEvent;
            Lottery.OnEvent += OnLotteryEvent;
            IsRunning = true;
            mClient.SendMsg(mGroupID, "_________start robot");
        }

        public void Stop()
        {
            mClient.OnEvent -= OnWechatEvent;
            Lottery.OnEvent -= OnLotteryEvent;
            IsRunning = false;
            mClient.SendMsg(mGroupID, "_________stop robot");
        }


        private void OnLotteryEvent(Lottery.LotteryCenter sender,Lottery.LottyEvent e)
        {
            if (!IsRunning) return;
            if (e is Lottery.StatusChanedEvent)
            {
                var statusChanedEvent = e as Lottery.StatusChanedEvent;
                if (statusChanedEvent.To == TestWechatGame.Lottery.LotteryStatus.Waitting)
                {
                    SendMessageToGroup("停止下注了，正在等待开奖.");
                }
                if (statusChanedEvent.To == TestWechatGame.Lottery.LotteryStatus.Order)
                {
                    SendMessageToGroup("开始下注了");
                }
            }

            if (e is Lottery.AwardEvent)
            {
                var awardEvent = e as Lottery.AwardEvent;
                string awardNumberStr = "";
                var nums = awardEvent.Award.AwardNumbers;
                for (int i =0;i<nums.Length;i++) {
                    awardNumberStr += nums[i].ToString();
                    if (i != nums.Length - 1) awardNumberStr += ",";
                }
                string msg = "==============<br>";
                msg += "期号:" + awardEvent.Award.No.ToString() + "<br>";
                msg += "开奖时间:" + awardEvent.Award.date.ToString() + "<br>";
                msg += "开奖号码:" + awardNumberStr + "<br>";
                msg += "==============<br>";
                SendMessageToGroup(msg);
            }
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
            if (contact.RemarkName != null && contact.RemarkName.Contains("(#") && contact.RemarkName.EndsWith(")")){
                int startIndex = contact.RemarkName.LastIndexOf("(#");
                string id = contact.RemarkName.Substring(startIndex + 2, contact.RemarkName.Length - startIndex-3);
                long uid = 1;
                long.TryParse(id, out uid);
                return uid;
            }
            return -1;
        }

        private bool SetMemberID(Contact contact,long id)
        {
            string idstr = string.Format("{0}(#{1})", Utils.ClearHtml(contact.NickName),id.ToString());
            if (mClient.SetRemarkName(contact.ID, idstr)) {
                return true;
            }
            return false;
        }


        private void HandleGroupMessage(TextMessage msg)
        {
            if (!IsRunning) return;
            var group = mClient.GetGroup(mGroupID);
            var member = group.GetMember(msg.FromContactID);
            var cmd = CommandFactory.Parse(msg.Content);
            if (cmd != null) {
                var contact = mClient.GetContact(msg.FromContactID);
                if (contact != null)
                {
                    cmd.Member = contact;
                    cmd.Execute(this);
                }
                else {
                    SendMessageToGroup("@" + member.NickName + " 请加我好友.");
                }
                
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

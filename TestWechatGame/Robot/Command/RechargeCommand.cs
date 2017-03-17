using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wechat;

namespace TestWechatGame.Command
{
    public class RechargeCommand : BaseCommand
    {

        private double mScore = 0.0f;

        public RechargeCommand(Contact member,double score) : base(member)
        {
            mScore = score;
        }

        public override bool Execute(Robot robot)
        {
            var user = robot.GetUser(Member);
            if (user == null)
            {
                robot.SendMessageToGroup("查找用户失败:" + Member.NickName);
                return false;
            }

            double score = (double)UserManager.GetUserData(user.ID, "Score");
            bool ret = UserManager.SetUserData(user.ID, "Score", (double)(score + mScore));
            if (ret)
            {
                robot.SendMessageToGroup("@" + Member.NickName + " 充值成功，您当前剩余积分为：" + (double)(score + mScore));
                return true;
            }
            else {
                robot.SendMessageToGroup("@" + Member.NickName + " 充值失败，您当前剩余积分为：" + (double)(score));
            }
            return false;
        }
    }
}

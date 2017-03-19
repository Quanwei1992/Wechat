using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wechat;

namespace TestWechatGame.Command
{
    public class QueryScoreCommand : BaseCommand
    {


        public override bool Execute(Robot robot)
        {
            var user = robot.GetUser(Member);
            if (user == null)
            {
                robot.SendMessageToGroup("查找用户失败:" + Member.NickName);
                return false;
            }
            double score = (double)UserManager.GetUserData(user.ID, "Score");
            robot.SendMessageToGroup("@" + Utils.ClearHtml(Member.NickName) + string.Format(" 您的剩余积分为:{0:F2}",score));
            return true;
        }
    }
}

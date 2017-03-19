using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestWechatGame.Command
{
    public class QueryNextAwardNextTime : BaseCommand
    {
        public override bool Execute(Robot robot)
        {
            var nextAwardTime = robot.Lottery.GetNextAwardTime();
            robot.SendMessageToGroup(string.Format("下次开奖时间为：{0},剩余时间还有:{1}",nextAwardTime,(nextAwardTime - DateTime.Now).ToString()));
            return true;
        }
    }
}

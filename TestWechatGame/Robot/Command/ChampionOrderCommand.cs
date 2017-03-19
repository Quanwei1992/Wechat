using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestWechatGame.Command
{
    public class ChampionOrderCommand : BaseCommand
    {
        public double Score;
        public int[] Nums;
        public ChampionOrderCommand(double score,int[] nums)
        {
            Score = score;
            Nums = nums;
        }

        public override bool Execute(Robot robot)
        {
            var user = robot.GetUser(Member);
            if(user != null){
                Lottery.ChampionOrder order = new Lottery.ChampionOrder(user,Nums,Score);
                if (robot.Lottery.SubmitOrder(order))
                {
                    robot.SendMessageToGroup("@" + Utils.ClearHtml(Member.NickName) + " 下注成功!");
                }
                else {
                    robot.SendMessageToGroup("@" + Utils.ClearHtml(Member.NickName) + " 下注失败!");
                }
            }
            return false;
        }

    }
}

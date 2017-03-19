using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TestWechatGame.Command;
namespace TestWechatGame
{
    public class CommandFactory
    {
        public static BaseCommand Parse(string msgContent)
        {
            string ChampionOrderPattern = @"^(?<NUMS>[0-9]+)/(?<Score>[0-9\.]+)$";
            Regex ChampionOrderRegex = new Regex(ChampionOrderPattern);


            BaseCommand cmd = null;
            string msg = msgContent.Trim();
            if (msg == "查积分")
            {
                cmd = new QueryScoreCommand();
            }
            else if (msg.StartsWith("充值"))
            {
                var param = msg.Split('值');
                if (param.Length == 2)
                {
                    double score = 0;
                    if (double.TryParse(param[1], out score))
                    {
                        cmd = new RechargeCommand(score);
                    }
                }
            }
            else if (ChampionOrderRegex.IsMatch(msg))
            {
                var match = ChampionOrderRegex.Match(msg);
                var numsStr = match.Groups["NUMS"].Value;
                var scoreStr = match.Groups["Score"].Value;
                int[] nums = new int[numsStr.Length];
                for (int i = 0; i < nums.Length; i++)
                {
                    int num = int.Parse(numsStr[i].ToString());
                    if (num == 0) num = 10;
                    nums[i] = num;
                }
                double score = 0.0f;
                if (double.TryParse(scoreStr, out score))
                {
                    cmd = new ChampionOrderCommand(score, nums);
                }

            }
            else if (msg == "开奖时间") {
                cmd = new QueryNextAwardNextTime();
            }

           
            return cmd;
        }
    }
}

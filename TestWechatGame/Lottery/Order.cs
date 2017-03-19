using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestWechatGame.Lottery
{
    public class Order
    {

        public User Onwer { get; private set; }

        public Order(User user)
        {
            Onwer = user;
        }


        public virtual bool PreProcess()
        {
            return false;
        }

        public virtual double Process(PKTENAward award)
        {
            return 0;
        }

    }


    public class ChampionOrder : Order
    {
        private const double ODDS = 9.8f;
        public int[] Nums { get; private set; }
        public double Score { get; private set; }
        public ChampionOrder(User user,int[] nums,double score) : base(user)
        {
            List<int> numList = new List<int>(nums);      
            Nums = numList.Distinct().ToArray();
            Score = score;
        }


        public override bool PreProcess()
        {
            if (Nums.Length <= 0 || Score<=0) return false;
            double needSubScore = Nums.Length * Score;
            double userScore = (double)UserManager.GetUserData(Onwer.ID, "Score");
            if (userScore < needSubScore) return false;
            return UserManager.SetUserData(Onwer.ID, "Score", (double)(userScore-needSubScore));
        }

        public override double Process(PKTENAward award)
        {
            int firstNum = award.AwardNumbers[0];
            double addScore = 0.0f;
            foreach (var num in Nums)
            {
                if (firstNum == num) {
                    addScore += Score * ODDS;
                    break;
                }
            }
            if (addScore > 0)
            {
                double userScore = (double)UserManager.GetUserData(Onwer.ID, "Score");
                UserManager.SetUserData(Onwer.ID, "Score", (double)(addScore + userScore));
            }
           
            return addScore;
        }
    }


}

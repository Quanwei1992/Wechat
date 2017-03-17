using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestWechatGame.Command;
namespace TestWechatGame
{
    public class CommandFactory
    {
        public static IContactCommand Parse(string msgContent,Wechat.Contact contact)
        {
            IContactCommand cmd = null;
            string msg = msgContent.Trim();
            if (msg == "查积分")
            {
                cmd = new QueryScoreCommand(contact);
            }
            else if(msg.StartsWith("充值")) {
                var param = msg.Split('值');
                if (param.Length == 2){
                    double score = 0;
                    if (double.TryParse(param[1], out score)) {
                        cmd = new RechargeCommand(contact,score);
                    }
                }
            }
            return cmd;
        }
    }
}

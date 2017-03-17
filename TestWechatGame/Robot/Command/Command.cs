using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wechat;

namespace TestWechatGame
{
    public interface IContactCommand
    {
        bool Execute(Robot robot);
    }

    public class BaseCommand : IContactCommand
    {
        protected Contact Member;
        public BaseCommand(Contact member)
        {
            Member = member;
        }

        public virtual bool Execute(Robot robot)
        {
            return false;
        }
    }


}

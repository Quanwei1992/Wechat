using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestWechatGame.PKTEN
{
    public class PKTENAward
    {
        /// 期号
        /// </summary>
        public int No;
        /// <summary>
        /// 开奖号码
        /// </summary>
        public int[] AwardNumbers = new int[10];
        /// <summary>
        /// 开奖时间
        /// </summary>
        public DateTime date;
    }
}

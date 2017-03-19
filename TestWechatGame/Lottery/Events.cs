using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestWechatGame.Lottery
{
    public class LottyEvent : EventArgs
    {
    }

    public class StatusChanedEvent : LottyEvent
    {
        public LotteryStatus From;
        public LotteryStatus To;
        public StatusChanedEvent(LotteryStatus from,LotteryStatus to)
        {
            From = from;
            To = to;
        }
    }

    public class AwardEvent : LottyEvent
    {
        public PKTENAward Award;
        public AwardEvent(PKTENAward award)
        {
            Award = award;
        }
    }

    public class OrderProcessedEvent : LottyEvent
    {
        public double Result;
        public Order UserOrder;

        public OrderProcessedEvent(double result, Order order)
        {
            Result = result;
            UserOrder = order;
        }
    }
}

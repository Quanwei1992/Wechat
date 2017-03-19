using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using HtmlAgilityPack;
using System.Threading;

namespace TestWechatGame.Lottery
{


    public enum LotteryStatus
    {
        None,
        Order,//接受下注
        Waitting,//等待开奖
        Award,//开奖结算
        End,//结束开奖 开奖时间是09:00~ 24:00
    }

    public class LotteryCenter
    {


        private Thread mMainLoopThread = null;
        private LotteryStatus mStatus = LotteryStatus.None;
        private PKTENAward lastAward = null;
        private List<Order> mOrderList = new List<Order>();

        public Action<LotteryCenter, Lottery.LottyEvent> OnEvent;

        public LotteryStatus Status
        {
            get {
                return mStatus;
            }
            private set
            {
                if (mStatus != value) {
                    var statusChangedEvent = new StatusChanedEvent(mStatus, value);
                    mStatus = value;
                    OnEvent?.Invoke(this, statusChangedEvent);
                }
            }
        }


        public void Run()
        {
            if (mMainLoopThread != null) return;
            Status = LotteryStatus.Order;
            mMainLoopThread = new Thread(MainLoop);
            mMainLoopThread.Start();
        }

        public void Stop()
        {
            if (mMainLoopThread == null || !mMainLoopThread.IsAlive) return;
            mMainLoopThread.Abort();
            mMainLoopThread = null;
        }


        public bool SubmitOrder(Order order)
        {
            if (Status != LotteryStatus.Order) return false;
            if (order == null) return false;
            if (order.PreProcess()) {
                mOrderList.Add(order);
                return true;
            }
            return false;
        }




        /// <summary>
        /// 获得下次开奖时间
        /// </summary>
        /// <returns></returns>
        public DateTime GetNextAwardTime()
        {
            if (lastAward == null) {
                lastAward = getLatestAward();
            }
            DateTime nextTime = lastAward.date + new TimeSpan(0, 5, 0);
            if (nextTime.Hour >= 0 && nextTime.Hour < 9) {
                nextTime = lastAward.date + new TimeSpan(9, 10, 0);
            }
            return nextTime;
        }


        private static PKTENAward getLatestAward()
        {
            string url = "http://www.bwlc.net/bulletin/trax.html";
            HtmlWeb htmlWeb = new HtmlWeb();
            HtmlDocument document = htmlWeb.Load(url);
            var lottery_tabs = document.GetElementbyId("lottery_tabs");
            if (lottery_tabs != null) {
                var lott_cont = lottery_tabs.Element("div");
                if (lott_cont != null && lott_cont.GetAttributeValue("class", null) == "lott_cont") {
                    var tb = lott_cont.Element("table");
                    if (tb != null) {
                        var trs = tb.Elements("tr");
                        foreach (var tr in trs){
                            var tds = tr.Elements("td").ToArray();
                            if (tds.Length == 3) {
                                PKTENAward award = new PKTENAward();
                                // 解析开奖期号
                                award.No = int.Parse(tds[0].InnerText);
                                // 解析开奖号码
                                string[] numstrs = tds[1].InnerText.Split(',');
                                for (int i = 0; i < 10; i++)
                                {
                                    int num = int.Parse(numstrs[i]);
                                    award.AwardNumbers[i] = num;
                                }
                                // 解析开奖时间
                                award.date = DateTime.Parse(tds[2].InnerText);

                                return award;
                            }
                        }
                    }
                }
            }


            return null;
        }




        void MainLoop()
        {
            while (true)
            {
                switch (Status)
                {
                    case LotteryStatus.Order:
                        HandleOrder();
                        break;
                    case LotteryStatus.Waitting:
                        HandleWaitting();
                        break;
                    case LotteryStatus.Award:
                        HandleAward();
                        break;
                }
            }
        }



        void HandleOrder()
        {
            //获得距离下次开奖时间，如果小于1分30秒则进入等待开奖状态
            var nextTime = GetNextAwardTime();
            var remainingTime = nextTime - DateTime.Now;
            if (remainingTime.TotalSeconds < 90)
            {
                Status = LotteryStatus.Waitting;
            }
        }


        void HandleWaitting()
        {
            if (lastAward == null) lastAward = getLatestAward();
            var newAward = getLatestAward();
            if (newAward.No > lastAward.No)
            {
                lastAward = newAward;
                Status = LotteryStatus.Award;
            }
            else {
                Thread.Sleep(3000);
            }
        }

        void HandleAward()
        {
            if (lastAward == null) lastAward = getLatestAward();
            OnEvent?.Invoke(this,new AwardEvent(lastAward));
            var orders = mOrderList.ToArray();
            mOrderList.Clear();
            foreach (var order in orders)
            {
                var result = order.Process(lastAward);
                OnEvent?.Invoke(this,new OrderProcessedEvent(result,order));
            }

            Status = LotteryStatus.Order;
        }



    }

    public class PKTENAward
    {
        public int No;
        public DateTime date;
        public int[] AwardNumbers = new int[10];
    }
}

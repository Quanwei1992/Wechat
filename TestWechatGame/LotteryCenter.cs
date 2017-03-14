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

namespace TestWechatGame.PKTEN
{
    public class LotteryCenter
    {


        #region Callbacks
        public Action<PKTENAward> OnNewAward;
        #endregion

        private PKTENAward mLastAward = null;



        public void Update()
        {
            updateAward();
        }


        private void updateAward()
        {
            var award = getLatestAward();
            if (mLastAward == null) {
                mLastAward = award;
                OnNewAward?.Invoke(award);
            }
            if (award.No > mLastAward.No) {
                mLastAward = award;
                OnNewAward?.Invoke(award);
            }
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



    }

    public class PKTENAward
    {
        public int No;
        public DateTime date;
        public int[] AwardNumbers;
    }
}

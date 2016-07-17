using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wechat.API.Http;
using Wechat.API.RPC;
using System.Drawing;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace Wechat.API
{
    public class WechatAPIService
    {
        HttpClient http;
        public WechatAPIService(HttpClient httpClient) {
            http = httpClient;
        }

        /// <summary>
        /// 获得二维码登录SessionID,使用此ID可以获得登录二维码
        /// </summary>
        /// <returns>Session</returns>
        public string GetNewQRLoginSessionID()
        {
            //respone like this => window.QRLogin.code = 200; window.QRLogin.uuid = "Qa_GBH_IqA==";
            string url = "https://login.weixin.qq.com/jslogin?appid=wx782c26e4c19acffb";
            byte[] bytes = http.GET(url);
            string str = Encoding.UTF8.GetString(bytes);
            string sessionID = str.Split(new string[] { "\"" }, StringSplitOptions.None)[1];
            return sessionID;
        }

        /// <summary>
        /// 获得登录二维码URL
        /// </summary>
        /// <param name="QRLoginSessionID"></param>
        /// <returns></returns>
        public string GetQRCodeUrl(string QRLoginSessionID) {
            string url = "https://login.weixin.qq.com/qrcode/" + QRLoginSessionID;
            return url;
        }

        /// <summary>
        /// 获得登录二维码图片
        /// </summary>
        /// <param name="QRLoginSessionID"></param>
        /// <returns></returns>
        public Image GetQRCodeImage(string QRLoginSessionID)
        {
            string url = GetQRCodeUrl(QRLoginSessionID);
            var bytes = http.GET(url);
            return Image.FromStream(new MemoryStream(bytes));
        }

        /// <summary>
        /// 登录检查
        /// </summary>
        /// <param name="QRLoginSessionID"></param>
        /// <returns></returns>
        public LoginResult Login(string QRLoginSessionID)
        {
            string url = "https://login.weixin.qq.com/cgi-bin/mmwebwx-bin/login?loginicon=true&uuid=" + QRLoginSessionID;
            byte[] bytes = http.GET(url);
            string login_result = Encoding.UTF8.GetString(bytes);
            LoginResult result = new LoginResult();
            result.code = 408;
            if (login_result.Contains("window.code=201")) //已扫描 未登录
            {
                string base64_image = login_result.Split(new string[] { "\'" }, StringSplitOptions.None)[1].Split(',')[1];
                result.code = 201;
                result.UserAvatar = base64_image;
            } else if (login_result.Contains("window.code=200"))  //已扫描 已登录
              {
                string login_redirect_url = login_result.Split(new string[] { "\"" }, StringSplitOptions.None)[1];
                result.code = 200;
                result.redirect_uri = login_redirect_url;
            }

            return result;
        }

        public LoginRedirectResult LoginRedirect(string redirect_uri)
        {
            string url = redirect_uri + "&fun=new&version=v2&lang=zh_CN";
            byte[] bytes = http.GET(url);
            string rep = Encoding.UTF8.GetString(bytes);
            LoginRedirectResult result = new LoginRedirectResult();
            result.pass_ticket = rep.Split(new string[] { "pass_ticket" }, StringSplitOptions.None)[1].TrimStart('>').TrimEnd('<', '/');
            result.skey = rep.Split(new string[] { "skey" }, StringSplitOptions.None)[1].TrimStart('>').TrimEnd('<', '/');
            result.wxsid = rep.Split(new string[] { "wxsid" }, StringSplitOptions.None)[1].TrimStart('>').TrimEnd('<', '/');
            result.wxuin = rep.Split(new string[] { "wxuin" }, StringSplitOptions.None)[1].TrimStart('>').TrimEnd('<', '/');
            result.isgrayscale = rep.Split(new string[] { "isgrayscale" }, StringSplitOptions.None)[1].TrimStart('>').TrimEnd('<', '/');
            return result;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="pass_ticket"></param>
        /// <param name="uin"></param>
        /// <param name="sid"></param>
        /// <param name="skey"></param>
        /// <param name="deviceID"></param>
        /// <returns></returns>
        public InitResponse Init(string pass_ticket,string uin,string sid,string skey,string deviceID= "e096782367064097")
        {
            string url = "https://wx.qq.com/cgi-bin/mmwebwx-bin/webwxinit?r=151280129&pass_ticket=" + pass_ticket;
            InitRequest initReq = new InitRequest();
            initReq.BaseRequest = new BaseRequest();
            initReq.BaseRequest.DeviceID = deviceID;
            initReq.BaseRequest.Sid = sid;
            initReq.BaseRequest.Uin = uin;
            initReq.BaseRequest.Skey = skey;
            string requestJson = JsonConvert.SerializeObject(initReq);
            string repJsonStr = http.POST_UTF8String(url,requestJson);
            var rep = JsonConvert.DeserializeObject<InitResponse>(repJsonStr);
            return rep;
        }

        /// <summary>
        /// 获得联系人列表
        /// </summary>
        /// <param name="pass_ticket"></param>
        /// <param name="skey"></param>
        /// <returns></returns>

        public GetContactResponse GetContact(string pass_ticket,string skey)
        {
            string url = "https://wx.qq.com/cgi-bin/mmwebwx-bin/webwxgetcontact?pass_ticket={0}&r=1468727560705&seq=0&skey={1}";
            url = string.Format(url, pass_ticket, skey);
            string json = http.GET_UTF8String(url);
            var rep = JsonConvert.DeserializeObject<GetContactResponse>(json);
            return rep;
        }

        /// <summary>
        /// 批量获取联系人详细信息
        /// </summary>
        /// <param name="requestContacts"></param>
        /// <param name="pass_ticket"></param>
        /// <param name="uin"></param>
        /// <param name="sid"></param>
        /// <param name="skey"></param>
        /// <param name="deviceID"></param>
        /// <returns></returns>
        public BatchGetContactResponse BatchGetContact(User[] requestContacts,string pass_ticket, string uin, string sid, string skey, string deviceID = "e096782367064097")
        {
            string url = "https://wx.qq.com/cgi-bin/mmwebwx-bin/webwxbatchgetcontact?type=ex&r=1468738266264&lang=zh_CN&pass_ticket=" + pass_ticket;
            BatchGetContactRequest req = new BatchGetContactRequest();
            req.BaseRequest = new BaseRequest();
            req.BaseRequest.DeviceID = deviceID;
            req.BaseRequest.Sid = sid;
            req.BaseRequest.Uin = uin;
            req.BaseRequest.Skey = skey;
            req.Count = requestContacts.Length;
            req.List = requestContacts;
            string requestJson = JsonConvert.SerializeObject(req);
            string repJsonStr = http.POST_UTF8String(url, requestJson);
            var rep = JsonConvert.DeserializeObject<BatchGetContactResponse>(repJsonStr);
            return rep;
        }

        public SyncCheckResponse SyncCheck(SyncItem[] syncItems,string uin, string sid, string skey, string deviceID = "e096782367064097")
        {
            string synckey = "";
            for (int i = 0; i < syncItems.Length; i++) {
                if (i != 0) {
                    synckey += "|";
                }
                synckey += syncItems[i].Key + "_" + syncItems[i].Val;
            }
            string url = "https://webpush.wx.qq.com/cgi-bin/mmwebwx-bin/synccheck?r=1468742341659&skey={0}&sid={1}&uin={2}&deviceid={3}&synckey={4}&t={5}";
            url = string.Format(url,skey,sid,uin,deviceID,synckey,DateTime.Now.Ticks);
            string repStr = http.GET_UTF8String(url);
            SyncCheckResponse rep = new SyncCheckResponse();
            if (repStr.StartsWith("window.synccheck="))
            {
                repStr = repStr.Substring("window.synccheck=".Length);
                rep = JsonConvert.DeserializeObject<SyncCheckResponse>(repStr);
            }
            
            return rep;
        }
    }
}

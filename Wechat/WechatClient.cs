using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wechat.API;
using System.Drawing;
using System.Diagnostics;
namespace Wechat
{
    public class WechatClient
    {

        public WechatClient()
        {
            api = new WechatAPIService(new API.Http.HttpClient());
        }


        private WechatAPIService api = null;
        private string mSkey;
        private string mWxsid;
        private string mWxuin;
        private string mPass_ticket;
        private string mDeviceID;
        public bool IsLogin { get; private set;}

        //---------------------------------------------------------
        // 所有缓存的用户信息
        private Dictionary<string, User> mCachedUsers = new Dictionary<string, User>();
        
        // 联系人列表
        private List<string> mContactList = new List<string>();

        // 最近联系人
        private List<string> mRecentContacts = new List<string>();

        // 当前用户
        private User mCurrentUser;



        //callback
        public Action<Image> OnGetQRCodeImage;
        public Action<Image> OnUserScanQRCode;
        public Action OnLoginSucess;


        /// <summary>
        /// 运行微信Client主逻辑,推荐放在独立的线程中执行这个方法
        /// </summary>
        public void Run()
        {
            string session = api.GetNewQRLoginSessionID();
            var QRImg = api.GetQRCodeImage(session);
            OnGetQRCodeImage?.Invoke(QRImg);
            //login check
            while (true) {
                var loginResult = api.Login(session);
                if (loginResult.code == 200) {
                    // 登录成功
                    var redirectResult = api.LoginRedirect(loginResult.redirect_uri);
                    mSkey = redirectResult.skey;
                    mWxsid = redirectResult.wxsid;
                    mWxuin = redirectResult.wxuin;
                    mPass_ticket = redirectResult.pass_ticket;
                    Random random = new Random();
                    IsLogin = true;
                    break;
                } else if (loginResult.code == 201){
                    // 已扫描,但是未确认登录
                    // convert base64 to image
                    byte[] base64_image_bytes = Convert.FromBase64String(loginResult.UserAvatar);
                    MemoryStream memoryStream = new MemoryStream(base64_image_bytes, 0, base64_image_bytes.Length);
                    memoryStream.Write(base64_image_bytes, 0, base64_image_bytes.Length);
                    var image = Image.FromStream(memoryStream);
                    OnUserScanQRCode?.Invoke(image);
                }
            }

            if (!IsLogin) return;

            Random ran = new Random();
            //e474383088963323
            int rand1 = ran.Next(10000,99999);
            int rand2 = ran.Next(10000, 99999);
            int rand3 = ran.Next(10000, 99999);
            mDeviceID = string.Format("e{0}{1}{2}",rand1,rand2,rand3);
            // 初始化
            var initResult = api.Init(mPass_ticket, mWxuin, mWxsid, mSkey,mDeviceID);
            if (initResult.BaseResponse.ret != 0) {
                System.Diagnostics.Debug.WriteLine(initResult.BaseResponse.ErrMsg);
                return;
            }

            mCurrentUser = initResult.User;

            // 开启状态通知

            var statusNotifyRep = api.Statusnotify(mCurrentUser.UserName,mCurrentUser.UserName,mPass_ticket,mWxuin,mWxsid,mSkey,mDeviceID);


            // 获得联系人列表
            var getContactResult = api.GetContact(mPass_ticket,mSkey);
            if (getContactResult.BaseResponse.ret != 0) {
                System.Diagnostics.Debug.WriteLine(getContactResult.BaseResponse.ErrMsg);
                return;
            }

            foreach (var user in getContactResult.MemberList) {
                mCachedUsers[user.UserName] = user;
            }

            List<string> waitingToCacheUserList = new List<string>();

            
            mRecentContacts.Clear();
            if (initResult.ContactList != null) {
                foreach (var user in initResult.ContactList){
                    mCachedUsers[user.UserName] = user;
                    Debug.WriteLine("Added User:" + user.NickName + " id:" + user.UserName);
                }
            }

            var chatsets = initResult.ChatSet.Split(',');
            foreach (var username in chatsets) {
                if (!username.StartsWith("@")) continue;
                if (!mCachedUsers.ContainsKey(username) && !waitingToCacheUserList.Contains(username)) {
                    waitingToCacheUserList.Add(username);
                }
            }


            OnLoginSucess?.Invoke();
            SyncKey syncKey = initResult.SyncKey;
            // main loop
            while (true){
                if (syncKey.Count > 0) {
                    var syncCheckResult = api.SyncCheck(syncKey.List, mWxuin, mWxsid, mSkey, mDeviceID);
                    
                    if (syncCheckResult.retcode == "0" && syncCheckResult.selector != "0") {
                        Debug.WriteLine("syncheck:" + syncCheckResult.retcode+","  + syncCheckResult.selector);
                        var syncResult = api.Sync(syncKey,mWxuin,mWxsid,mSkey,mPass_ticket, mDeviceID);
                        syncKey = syncResult.SyncKey;
                        Debug.WriteLine("AddMsgCount:" + syncResult.AddMsgCount);
                        // addmsg
                        if (syncResult.AddMsgCount > 0) {
                            foreach (var msg in syncResult.AddMsgList) {
                                Debug.WriteLine("Msg:" + msg.Content+"," + msg.StatusNotifyUserName);
                                var notifyUserNames = msg.StatusNotifyUserName.Split(',');
                                foreach (var username in notifyUserNames) {
                                    if (!username.StartsWith("@")) continue;
                                    if (!mCachedUsers.ContainsKey(username) && !waitingToCacheUserList.Contains(username)) {
                                        waitingToCacheUserList.Add(username);
                                    }
                                }
                            }
                        }
                    }
                }

                // get user
                var batchResult = api.BatchGetContact(waitingToCacheUserList.ToArray(), mPass_ticket, mWxuin, mWxsid, mSkey,mDeviceID);
                if (batchResult.ContactList != null) {
                    foreach (var user in batchResult.ContactList) {
                        mCachedUsers[user.UserName] = user;
                        Debug.WriteLine("Added User:" + user.NickName + " id:" + user.UserName);
                    }
                }
                waitingToCacheUserList.Clear();
            }


        }




    }
}

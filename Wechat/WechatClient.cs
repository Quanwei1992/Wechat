using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wechat.API;
using System.Drawing;
using System.Diagnostics;
using System.Drawing.Imaging;
using Wechat.API.RPC;
namespace Wechat
{
    public class WechatClient
    {

        public WechatClient()
        {
            api = new WechatAPIService(new API.Http.HttpClient());
        }


        private WechatAPIService api = null;
        private string mPass_ticket;
        private BaseRequest mBaseReq;
        public bool IsLogin { get; private set;}
        
        //---------------------------------------------------------
        // 所有缓存的用户信息
        private Dictionary<string, User> mCachedUsers = new Dictionary<string, User>();

        // 联系人列表
        private List<string> mContactList = new List<string>();
        // 群聊列表
        private List<string> mGroupList = new List<string>();
        public string[] ContactList
        {
            get
            {
                return mContactList.ToArray();
            }
        }

        public string[] GroupList
        {
            get
            {
                return mGroupList.ToArray();
            }
        }

        // 最近联系人
        private List<string> mRecentContacts = new List<string>();

        // 当前用户
        public User CurrentUser { get;private set; }



        //callback
        public Action<Image> OnGetQRCodeImage;
        public Action<Image> OnUserScanQRCode;
        public Action OnLoginSucess;
        public Action OnInitComplate;
        public Action<User> OnAddUser;
        public Action<User> OnUpdateUser;
        public Action<AddMsg> OnRecvMsg;

        private void CacheUser(User user)
        {

            if (user.UserName.StartsWith("@@")) { //群聊
                if (!mGroupList.Contains(user.UserName)) {
                    mGroupList.Add(user.UserName);
                }
            }

            if (!mCachedUsers.ContainsKey(user.UserName)) {
                mCachedUsers[user.UserName] = user;
                OnAddUser?.Invoke(user);
            } else {
                mCachedUsers[user.UserName] = user;
                OnUpdateUser?.Invoke(user);
            }
        }

        public User GetContact(string userName)
        {
            if (mCachedUsers.ContainsKey(userName)) {
                return mCachedUsers[userName];
            }
            return null;
        }

        //首次初始化已完成(收到MsgType=51的消息,且获取完Contact)
        private bool firstInited = false;

        /// <summary>
        /// 运行微信Client主逻辑,推荐放在独立的线程中执行这个方法
        /// </summary>
        public void Run()
        {
            // 启动流程
            // 1.登陆
            // 2.初始化
            // 3.开启系统通知
            // 4.获得联系人列表
            // 5.进入同步主循环


            // ----------1.登陆

            do {
                Debug.Write("[*] 正在获取Session ....");
                string session = api.GetNewQRLoginSessionID();
                if (!string.IsNullOrWhiteSpace(session)) {
                    Debug.Write("成功\n");
                } else {
                    continue;
                }
                Debug.Write("[*] 正在生成二维码 ....");
                var QRImg = api.GetQRCodeImage(session);
                if (QRImg != null) {
                    Debug.Write("成功\n");
                } else {
                    continue;
                }
                Debug.Write("[*] 正在等待扫码 ....");
                OnGetQRCodeImage?.Invoke(QRImg);          
                //login check
                while (true) {
                    var loginResult = api.Login(session);
                    if (loginResult.code == 200) {
                        // 登录成功
                        var redirectResult = api.LoginRedirect(loginResult.redirect_uri);
                        mBaseReq = new BaseRequest();
                        mBaseReq.Skey = redirectResult.skey;
                        mBaseReq.Sid = redirectResult.wxsid;
                        mBaseReq.Uin = redirectResult.wxuin;
                        // 生成DeviceID
                        Random ran = new Random();
                        int rand1 = ran.Next(10000, 99999);
                        int rand2 = ran.Next(10000, 99999);
                        int rand3 = ran.Next(10000, 99999);
                        mBaseReq.DeviceID = string.Format("e{0}{1}{2}", rand1, rand2, rand3);
                        mPass_ticket = redirectResult.pass_ticket;
                        IsLogin = true;
                        Debug.Write("已确认\n");
                        break;
                    } else if (loginResult.code == 201) {
                        // 已扫描,但是未确认登录
                        // convert base64 to image
                        byte[] base64_image_bytes = Convert.FromBase64String(loginResult.UserAvatar);
                        MemoryStream memoryStream = new MemoryStream(base64_image_bytes, 0, base64_image_bytes.Length);
                        memoryStream.Write(base64_image_bytes, 0, base64_image_bytes.Length);
                        var image = Image.FromStream(memoryStream);
                        OnUserScanQRCode?.Invoke(image);
                        Debug.Write("已扫码\n");
                        Debug.Write("[*] 正在等待确认 ....");
                    } else {
                        // 超时
                        Debug.Write("超时\n");
                        break;
                    }
                }
            } while (!IsLogin);


            // ----------2.初始化
            Debug.Write("[*] 正在初始化 ....");
            var initResult = api.Init(mPass_ticket, mBaseReq);
            if (initResult.BaseResponse.ret == 0) {
                Debug.Write("成功\n");
            } else {
                Debug.Write("失败.错误码:" + initResult.BaseResponse.ret);
                return;
            }

            CurrentUser = initResult.User;


            // 最近联系人
            mRecentContacts.Clear();
            if (initResult.ContactList != null) {
                foreach (var user in initResult.ContactList) {
                    CacheUser(user);
                    mRecentContacts.Add(user.UserName);
                }
            }

            // chatsets 里有需要获取详细信息的联系人.
            List<string> waitingToCacheUserList = new List<string>();
            var chatsets = initResult.ChatSet.Split(',');
            foreach (var username in chatsets) {
                if (!username.StartsWith("@")) continue;
                if (!mCachedUsers.ContainsKey(username) && !waitingToCacheUserList.Contains(username)) {
                    waitingToCacheUserList.Add(username);
                }
            }



            // ----------3.开启状态通知
            Debug.Write("[*] 正在开启系统通知 ....");
            var statusNotifyRep = api.Statusnotify(CurrentUser.UserName, CurrentUser.UserName, mPass_ticket, mBaseReq);
            if (statusNotifyRep != null && statusNotifyRep.BaseResponse != null && statusNotifyRep.BaseResponse.ret == 0){
                Debug.Write("成功\n");
            } else {
                Debug.Write("失败.错误码:" + statusNotifyRep.BaseResponse.ret);
                return;
            }


            // ----------4.获得联系人列表
            Debug.Write("[*] 正在获取联系人列表 ....");
            var getContactResult = api.GetContact(mPass_ticket, mBaseReq.Skey);
            if (getContactResult!=null && getContactResult.BaseResponse!=null && getContactResult.BaseResponse.ret == 0) {
                Debug.Write("成功\n");
                Debug.WriteLine("[*] 共有 " + getContactResult.MemberCount + " 个联系人.");
            } else {
                Debug.Write("失败. 错误码:" + getContactResult.BaseResponse.ret);
                return;
            }
            foreach (var user in getContactResult.MemberList) {
                CacheUser(user);
                mContactList.Add(user.UserName);
            }

            Debug.Write("[*] 正在请求群聊成员详细信息 ....\n");
            //-----------5.批量获取群组详细信息
            foreach (var user in mCachedUsers.Values) {
                if (user.UserName.StartsWith("@@")) {
                    RefreshGroupMemberInfo(user.UserName);
                }
            }

            OnLoginSucess?.Invoke();
            SyncKey syncKey = initResult.SyncKey;
            // ----------6.同步主循环
            Debug.WriteLine("[*] 进入同步循环 ....");
            while (true){
                bool hasInitMsg = false;
                // 同步
                if (syncKey.Count > 0) {                  
                    var syncCheckResult = api.SyncCheck(syncKey.List, mBaseReq);
                    if (syncCheckResult == null) continue;
                    if (syncCheckResult.retcode != "0") {
                        Debug.WriteLine("[*] 登陆已失效,请重新登陆 ....");
                        IsLogin = false;
                        mCachedUsers.Clear();
                        mRecentContacts.Clear();
                        mContactList.Clear();
                        return;
                    }
                    if (syncCheckResult.retcode == "0" && syncCheckResult.selector != "0") {
                        Debug.WriteLine(string.Format("[*] 同步检查 RetCode:{0} Selector:{1}",syncCheckResult.retcode, syncCheckResult.selector));
                        var syncResult = api.Sync(syncKey,mPass_ticket, mBaseReq);
                        syncKey = syncResult.SyncKey;
                        Debug.WriteLine(string.Format("[*] 同步结果 AddMsgCount:{0} ModContactCount:{1}", syncResult.AddMsgCount, syncResult.ModContactCount));
                        // addmsg
                        if (syncResult.AddMsgCount > 0) {
                            foreach (var msg in syncResult.AddMsgList) {
                                // 过滤系统信息
                                if (msg.MsgType != 51) {
                                    OnRecvMsg?.Invoke(msg);
                                } else {
                                    hasInitMsg = true;
                                }
                                var notifyUserNames = msg.StatusNotifyUserName.Split(',');
                                foreach (var username in notifyUserNames) {
                                    if (!username.StartsWith("@")) continue;
                                    if (!mCachedUsers.ContainsKey(username) && !waitingToCacheUserList.Contains(username)) {
                                        waitingToCacheUserList.Add(username);
                                    }
                                }
                            }
                        }

                        // modify contact
                        if (syncResult.ModContactList != null) {
                            foreach (var modContact in syncResult.ModContactList) {
                                CacheUser(modContact);
                            }
                        }
                    }
                }
                if (waitingToCacheUserList.Count > 0) {
                    // 获得群详细信息
                    Debug.WriteLine("[*] 正在获取联系人详细信息 ....");

                    foreach (var userName in waitingToCacheUserList) {
                        if (userName.StartsWith("@@")) {
                            RefreshGroupMemberInfo(userName);
                        }
                    }

                    var batchResult = api.BatchGetContact(waitingToCacheUserList.ToArray(), mPass_ticket, mBaseReq);
                    if (batchResult != null && batchResult.ContactList != null) {
                        foreach (var user in batchResult.ContactList) {
                            CacheUser(user);
                        }
                        Debug.WriteLine("[*] 获取到联系人详细信息 "+batchResult.Count+"个");
                    }
                    waitingToCacheUserList.Clear();
                }

                
                // 初始化完成回调
                if (hasInitMsg && !firstInited) {
                    firstInited = true;
                    OnInitComplate?.Invoke();
                }
            }
        }

        /// <summary>
        /// 刷新群聊成员信息(Sync的时候可以返回群聊成员的Uin)
        /// </summary>
        /// <param name="groupUserName"></param>
        public void RefreshGroupMemberInfo(string groupUserName)
        {
            api.Oplog(groupUserName, 3, 0, mPass_ticket, mBaseReq);
        }

        public bool SendMsg(string toUserName,string content)
        {
            Msg msg = new Msg();
            msg.FromUserName = CurrentUser.UserName;
            msg.ToUserName = toUserName;
            msg.Content = content;
            msg.ClientMsgId = DateTime.Now.Millisecond;
            msg.LocalID = DateTime.Now.Millisecond;
            msg.Type = 1;//type 1 文本消息
            var response = api.SendMsg(msg,mPass_ticket, mBaseReq);
            if (response != null && response.BaseResponse != null && response.BaseResponse.ret == 0) {
                return true;
            } else {
                return false;
            }
        }

        int upLoadMediaCount = 0;

        public bool SendMsg(string toUserName, Image img, ImageFormat format = null, string imageName = null)
        {
            if (img == null) return false;
            string fileName = imageName != null ? imageName : "img_" + upLoadMediaCount;
            var imgFormat = format != null ? format : ImageFormat.Png;

            fileName += "." + imgFormat.ToString().ToLower();

            MemoryStream ms = new MemoryStream();
            img.Save(ms, imgFormat);
            ms.Seek(0, SeekOrigin.Begin);
            byte[] data = new byte[ms.Length];
            int readCount = ms.Read(data,0,data.Length);
            if (readCount != data.Length) return false;

            string mimetype = "image/" + imgFormat.ToString().ToLower();
            var response = api.Uploadmedia(CurrentUser.UserName, toUserName, "WU_FILE_" + upLoadMediaCount, mimetype, 2, 4, data, fileName, mPass_ticket, mBaseReq);
            if (response != null && response.BaseResponse != null && response.BaseResponse.ret == 0) {
                upLoadMediaCount++;
                string mediaId = response.MediaId;
                ImgMsg msg = new ImgMsg();
                msg.FromUserName = CurrentUser.UserName;
                msg.ToUserName = toUserName;
                msg.MediaId = mediaId;
                msg.ClientMsgId = DateTime.Now.Millisecond;
                msg.LocalID = DateTime.Now.Millisecond;
                msg.Type = 3;
                var sendImgRep = api.SendMsgImg(msg, mPass_ticket, mBaseReq);
                if (sendImgRep != null && sendImgRep.BaseResponse != null && sendImgRep.BaseResponse.ret == 0) {
                    return true;
                }
                return false;
            } else {
                return false;
            }
        }

    }
}

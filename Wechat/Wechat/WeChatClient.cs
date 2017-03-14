using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Wechat.API;
using Wechat.API.RPC;
using System.Collections.Generic;

namespace Wechat
{

    public enum ClientStatusType
    {
        GetUUID,
        GetQRCode,
        Login,
        WeixinInit,
        SyncCheck,
        WeixinSync,
        None,
    }


    public class WeChatClient
    {



        public Action<WeChatClient, WeChatClientEvent> OnEvent;


        public Contact Self { get; private set; }

        public bool IsLogin { get; private set; }

        public ClientStatusType CurrentStatus
        {
            get
            {
                return mStatus;
            }
            private set
            {
                if (mStatus != value)
                {
                    var changedEvent = new StatusChangedEvent()
                    {
                        FromStatus = mStatus,
                        ToStatus = value
                    };
                    mStatus = value;
                    OnEvent?.Invoke(this, changedEvent);
                }
            }
        }



        private List<Contact> mContacts;
        
        public Contact[] Contacts
        {
            get{
                return mContacts.ToArray();
            }
            private set {
                mContacts = new List<Contact>();
                mContacts.AddRange(value);
            }
        }

        private List<Group> mGroups;
        public Group[] Groups
        {
            get {
                return mGroups.ToArray();
            }
            private set
            {
                mGroups = new List<Group>();
                mGroups.AddRange(value);
            }
        }


        /*
         * Web Weixin Pipeline
         +--------------+     +---------------+   +---------------+
               |              |     |               |   |               |
               |   Get UUID   |     |  Get Contact  |   | Status Notify |
               |              |     |               |   |               |
               +-------+------+     +-------^-------+   +-------^-------+
                       |                    |                   |
                       |                    +-------+  +--------+
                       |                            |  |
               +-------v------+               +-----+--+------+      +--------------+
               |              |               |               |      |              |
               |  Get QRCode  |               |  Weixin Init  +------>  Sync Check<----+
               |              |               |               |      |              |    |
               +-------+------+               +-------^-------+      +-------+------+    |
                       |                              |                      |           |
                       |                              |                      +-----------+
                       |                              |                      |
               +-------v------+               +-------+--------+     +-------v-------+
               |              | Confirm Login |                |     |               |
        +------>    Login     +---------------> New Login Page |     |  Weixin Sync  |
        |      |              |               |                |     |               |
        |      +------+-------+               +----------------+     +---------------+
        |             |
        |QRCode Scaned|
        +-------------+
        */

        private System.Threading.Thread mMainLoopThread;
        private API.Http.HttpClient mHttpClient;
        private ClientStatusType mStatus = ClientStatusType.None;
        public void Run()
        {
            Quit();
            mIsQuit = false;
            IsLogin = false;
            CurrentStatus = ClientStatusType.GetUUID;
            mHttpClient = new API.Http.HttpClient();
            mAPIService = new WechatAPIService(mHttpClient);
            mMainLoopThread = new System.Threading.Thread(MainLoop);
            mMainLoopThread.Start();
        }

        public void Quit(bool force = false)
        {
            mIsQuit = true;
            if (force) {
                if (mMainLoopThread != null && mMainLoopThread.IsAlive) {
                    mMainLoopThread.Abort();
                }       
            }
        }



        private bool mIsQuit = false;
        private WechatAPIService mAPIService = null;
        private string mPass_ticket;
        private BaseRequest mBaseReq;
        private string mLoginSession;
        private void MainLoop()
        {
            while (!mIsQuit)
            {
                switch (CurrentStatus)
                {
                    case ClientStatusType.GetUUID:
                        HandleGetLoginSession();
                        break;
                    case ClientStatusType.GetQRCode:
                        HandleGetQRCode();
                        break;
                    case ClientStatusType.Login:
                        HandleLogin();
                        break;
                    case ClientStatusType.WeixinInit:
                        HandleInit();
                        break;
                    case ClientStatusType.WeixinSync:
                        HandleSync();
                        break;
                }
            }
        }



        #region StatusHandle
        private void HandleGetLoginSession()
        {
            IsLogin = false;
            mHttpClient.ClearCookie();
            mLoginSession = mAPIService.GetNewQRLoginSessionID();
            if (!string.IsNullOrWhiteSpace(mLoginSession))
            {
                CurrentStatus = ClientStatusType.GetQRCode;
            }
        }

        private void HandleGetQRCode()
        {
            var QRCodeImg = mAPIService.GetQRCodeImage(mLoginSession);
            if (QRCodeImg != null)
            {
                CurrentStatus = ClientStatusType.Login;
                var wce = new GetQRCodeImageEvent()
                {
                    QRImage = QRCodeImg
                };
                OnEvent?.Invoke(this, wce);
            }
        }


        private void HandleLogin()
        {
            var loginResult = mAPIService.Login(mLoginSession);
            if (loginResult.code == 200)
            {
                // 登录成功
                var redirectResult = mAPIService.LoginRedirect(loginResult.redirect_uri);
                mBaseReq = new BaseRequest();
                mBaseReq.Skey = redirectResult.skey;
                mBaseReq.Sid = redirectResult.wxsid;
                mBaseReq.Uin = redirectResult.wxuin;
                mBaseReq.DeviceID = CreateNewDeviceID();
                mPass_ticket = redirectResult.pass_ticket;
                CurrentStatus = ClientStatusType.WeixinInit;
                OnEvent?.Invoke(this, new LoginSucessEvent());

            }
            else if (loginResult.code == 201)
            {
                // 已扫描,但是未确认登录
                // convert base64 to image
                byte[] base64_image_bytes = Convert.FromBase64String(loginResult.UserAvatar);
                MemoryStream memoryStream = new MemoryStream(base64_image_bytes, 0, base64_image_bytes.Length);
                memoryStream.Write(base64_image_bytes, 0, base64_image_bytes.Length);
                var image = Image.FromStream(memoryStream);
                OnEvent?.Invoke(this, new UserScanQRCodeEvent()
                {
                    UserAvatarImage = image
                });
            }
            else
            {
                CurrentStatus = ClientStatusType.GetUUID;
            }
        }

        private void HandleInit()
        {
            var initResult = mAPIService.Init(mPass_ticket, mBaseReq);
            if (initResult.BaseResponse.ret == 0)
            {
                Self = CreateContact(initResult.User);
                mSyncKey = initResult.SyncKey;
                // 开启系统通知
                var statusNotifyRep = mAPIService.Statusnotify(Self.ID, Self.ID, mPass_ticket, mBaseReq);
                if (statusNotifyRep != null && statusNotifyRep.BaseResponse != null && statusNotifyRep.BaseResponse.ret == 0)
                {
                    IsLogin = true;
                    CurrentStatus = ClientStatusType.WeixinSync;
                }
                else {
                    CurrentStatus = ClientStatusType.GetUUID;
                    return;
                }              
            }
            else
            {
                CurrentStatus = ClientStatusType.GetUUID;
                return;
            }

            InitContactAndGroups();

            OnEvent?.Invoke(this, new InitedEvent());

        }


        private void InitContactAndGroups()
        {
            mContacts = new List<Contact>();
            mGroups = new List<Group>();

            var contactResult = mAPIService.GetContact(mPass_ticket, mBaseReq.Skey);
            if (contactResult == null || contactResult.BaseResponse == null || contactResult.BaseResponse.ret != 0){
                CurrentStatus = ClientStatusType.GetUUID;
                return;
            }

            List<string> groupIDs = new List<string>();
            foreach (var user in contactResult.MemberList)
            {
                if (user.UserName.StartsWith("@@")){
                    groupIDs.Add(user.UserName);
                }
                else {
                    var contact = CreateContact(user);
                    mContacts.Add(contact);
                }
            }

            if (groupIDs.Count <= 0) return;
            // 批量获得群成员详细信息
            var batchResult = mAPIService.BatchGetContact(groupIDs.ToArray(), mPass_ticket, mBaseReq);
            if (batchResult == null || batchResult.BaseResponse.ret != 0) return;

            foreach (var user in batchResult.ContactList)
            {
                if (!user.UserName.StartsWith("@@") || user.MemberCount <= 0) continue;
                Group group = new Group();
                group.ID = user.UserName;
                group.NickName = user.NickName;
                group.RemarkName = user.RemarkName;
                List<Contact> groupMembers = new List<Contact>();
                foreach (var member in user.MemberList)
                {
                    groupMembers.Add(CreateContact(member));
                }
                group.Members = groupMembers.ToArray();
                mGroups.Add(group);
            }


        }


        private SyncKey mSyncKey;
        private void HandleSync()
        {
            if (mSyncKey == null) {
                CurrentStatus = ClientStatusType.GetUUID;
                return;
            }
            if (mSyncKey.Count <= 0) return;

            var checkResult = mAPIService.SyncCheck(mSyncKey.List, mBaseReq);
            if (checkResult.retcode != "0") {
                CurrentStatus = ClientStatusType.GetUUID;
                return;
            }
            if (checkResult.selector == "0") return;
            var syncResult = mAPIService.Sync(mSyncKey, mPass_ticket, mBaseReq);
            mSyncKey = syncResult.SyncKey;

            // 处理同步
            ProcessSyncResult(syncResult);

        }

        private void ProcessSyncResult(SyncResponse result)
        {
            // 处理消息
            if (result.AddMsgCount > 0) {
                foreach (var msg in result.AddMsgList)
                {
                    var message = MessageFactory.CreateMessage(msg);
                    OnEvent?.Invoke(this, new AddMessageEvent() {
                        Msg = message
                    });
                }
            }
        }

        #endregion


        private static string CreateNewDeviceID()
        {
            Random ran = new Random();
            int rand1 = ran.Next(10000, 99999);
            int rand2 = ran.Next(10000, 99999);
            int rand3 = ran.Next(10000, 99999);
            return string.Format("e{0}{1}{2}", rand1, rand2, rand3);
        }


        public Contact CreateContact(Wechat.API.User user)
        {

            Contact contact = new Contact();
            contact.ID = user.UserName;
            contact.NickName = user.NickName;
            contact.RemarkName = user.RemarkName;
            return contact;
        }

        public Contact[] GetGroupMembers(string groupID)
        {

            //获取群聊成员
            var batchResult = mAPIService.BatchGetContact(new string[] { groupID}, mPass_ticket, mBaseReq);
            if (batchResult == null || batchResult.BaseResponse.ret != 0) return null;

            List<Contact> members = new List<Contact>();
            foreach(var contact in batchResult.ContactList)
            {
                if (contact.UserName.StartsWith("@@")) continue;
                members.Add(CreateContact(contact));
            }

            return members.ToArray();

        }
    }
}

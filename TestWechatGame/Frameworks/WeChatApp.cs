using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Wechat;
namespace TestWechatGame.Frameworks
{
    public class WeChatApp
    {
        public Action<WeChatApp, EventArgs> mAppEventCallback;
        private WechatClient mClient = new Wechat.WechatClient();
        private System.Diagnostics.Stopwatch mStopWatch = new System.Diagnostics.Stopwatch();
        private Thread mWechatThread = null;
        private bool IsRunning = true;
        private Thread mMainLoopThread = null;


        protected WechatClient Client
        {
            get
            {
                return mClient;
            }
        }

        public float Time
        {
            get {
                return mStopWatch.ElapsedMilliseconds / 1000.0f;
            }
        }





        public WeChatApp(Action<WeChatApp, EventArgs> eventCallback)
        {
            mAppEventCallback = eventCallback;
        }




        public virtual void Run()
        {
            mStopWatch.Start();
            Init();
            mClient.OnGetQRCodeImage = (QRImage) => {
                if (mAppEventCallback != null) {
                    GetQRCodeImageEvent eve = new GetQRCodeImageEvent();
                    eve.QRImage = QRImage;
                    mAppEventCallback.Invoke(this, eve);
                }
            };
            mClient.OnUserScanQRCode = (AvataImage) => {
                if (mAppEventCallback != null) {
                    UserScanQRCodeEvent eve = new UserScanQRCodeEvent();
                    eve.UserAvatarImage = AvataImage;
                    mAppEventCallback.Invoke(this, eve);
                }
            };
            mClient.OnLoginSucess = () => {
                if (mAppEventCallback != null) {
                    LoginSucessEvent eve = new LoginSucessEvent();
                    mAppEventCallback.Invoke(this, eve);
                }
            };
            mClient.OnInitComplate = () => {
                if (mAppEventCallback != null) {
                    InitComplateEvent eve = new InitComplateEvent();
                    mAppEventCallback.Invoke(this, eve);
                    WeChatInitComplate();
                }
            };

            mClient.OnRecvMsg = HandleMessage;

            mWechatThread = new Thread(mClient.Run);
            mWechatThread.Start();
            mMainLoopThread = new Thread(MainLoop);
            mMainLoopThread.Start();
        }

        private void MainLoop()
        {
            while (IsRunning) {
                Update();
            }
        }


        protected virtual void Init()
        {

        }

        protected virtual void WeChatInitComplate()
        {

        }

        protected virtual void HandleMessage(Wechat.API.AddMsg msg)
        {
        }


        protected virtual void Update()
        {

        }

    }
}

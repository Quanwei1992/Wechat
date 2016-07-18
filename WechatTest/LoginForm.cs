using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Wechat.API.Http;
using Wechat.API;
using Wechat;
using System.IO;
namespace WechatTest
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            WechatClient wc = new WechatClient();
            wc.OnGetQRCodeImage = (image) => {
                RunInMainthread(()=> {
                    pictureBox_qr.Image = image;
                });
            };

            wc.OnUserScanQRCode = (image) => {
                RunInMainthread(() => {
                    pictureBox_qr.Image = image;
                });
            };

            wc.OnLoginSucess = () => {
                RunInMainthread(()=> {
                   // MessageBox.Show("login sucess");
                });
            };

            wc.OnRecvMsg = (msg) => {
                if (msg.ToUserName == wc.CurrentUser.UserName && !msg.FromUserName.StartsWith("@@")) {
                    System.Diagnostics.Debug.WriteLine("RecvMsg:" + msg.Content + " from " + msg.FromUserName);
                    string rep = "";
                    if (msg.Content == "1") {
                        rep = "你";
                    }
                    if (msg.Content == "2") {
                        rep = "你好";
                    }
                    if (msg.Content == "3") {
                        rep = "你好吗";
                    }
                    //var rep = Simsimi.say(msg.Content);
                    wc.SendMsg(msg.FromUserName, rep);
                    System.Diagnostics.Debug.WriteLine("SendMsg:" + rep + " to " + msg.FromUserName);
                }
            };

            wc.OnAddUser = (user) => {
                RunInMainthread(() => {
                    if (user.UserName.StartsWith("@@")) {
                        string nickName = user.NickName;
                        if (string.IsNullOrWhiteSpace(nickName)) {
                            if (user.MemberList != null) {
                                foreach (var member in user.MemberList) {
                                    nickName += member.NickName + ",";
                                }
                            }
                            if (nickName.EndsWith(",")) {
                                nickName = nickName.Remove(nickName.Length - 1);
                            }
                        }
                        comboBox_users.Items.Add(nickName);
                        if (user.MemberCount <= 0) {
                            System.Diagnostics.Debug.WriteLine("memcount<=0:" + nickName);
                        }
                    }
                });
            };


            RunAsync(()=>{
                wc.Run();
            });
        }



        void RunAsync(Action action) {
            ((Action)(delegate () {
                action?.Invoke();
            })).BeginInvoke(null, null);
        }

        void RunInMainthread(Action action) {
            this.BeginInvoke((Action)(delegate () {
                action?.Invoke();
            }));
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestWechatGame
{
    public partial class LoginForm : Form
    {
        Wechat.WechatClient mWechatClient;
        public LoginForm(Wechat.WechatClient wc)
        {
            mWechatClient = wc;
            InitializeComponent();
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            var wc = mWechatClient;
            wc.OnGetQRCodeImage = (image) => {
                RunInMainthread(() => {
                    pictureBox_qr.Image = image;
                });
            };

            wc.OnUserScanQRCode = (image) => {
                RunInMainthread(() => {

                    pictureBox_qr.Image = image;
                    label_tips.Text = "扫描成功\n请在手机上确认登陆";
                });
            };

            wc.OnLoginSucess = () => {
                RunInMainthread(() => {
                    label_tips.Text = "已确认,正在登陆....";
                });
            };

            wc.OnInitComplate += () => {
                RunInMainthread(() => {
                    this.Visible = false;
                });
            };
            RunAsync(() => {
                wc.Run();
            });
        }


        void RunAsync(Action action)
        {
            ((Action)(delegate () {
                action?.Invoke();
            })).BeginInvoke(null, null);
        }

        void RunInMainthread(Action action)
        {
            this.BeginInvoke((Action)(delegate () {
                action?.Invoke();
            }));
        }

    }
}

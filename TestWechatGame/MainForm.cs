using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using TestWechatGame.PKTEN;
using TestWechatGame.Frameworks;

namespace TestWechatGame
{
    public partial class MainForm : Form
    {
        PKTENApp app;
        
        

        public MainForm()
        {
            InitializeComponent();
        }


        private void MainForm_Load(object sender, EventArgs e)
        {
            app = new PKTENApp(OnWechatAppEvent);
            app.Run();

        }


        private void OnWechatAppEvent(WeChatApp sender, EventArgs e)
        {
            if (e is GetQRCodeImageEvent) {
                RunInMainthread(() => {
                    pictureBox_wechat.Image = (e as GetQRCodeImageEvent).QRImage;
                });
            } else if (e is UserScanQRCodeEvent) {
                RunInMainthread(() => {
                    pictureBox_wechat.Image = (e as UserScanQRCodeEvent).UserAvatarImage;
                });
            } else if (e is InitComplateEvent) {
                RunInMainthread(()=> {
                    ShowGroups();
                });
            }
        }


        private void ShowGroups()
        {

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

        private void button_run_Click(object sender, EventArgs e)
        {


        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {

        }
    }
}

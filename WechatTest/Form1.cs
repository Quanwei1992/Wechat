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
    public partial class Form1 : Form
    {
        public Form1()
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

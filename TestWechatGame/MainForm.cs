using System;
using System.Windows.Forms;
using TestWechatGame.PKTEN;
using TestWechatGame.Frameworks;
using System.Data;

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
            comboBox_group.Enabled = false;
            button_run.Enabled = false;
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
            var groups = app.Groups;
            DataTable dt = new DataTable();
            dt.Columns.Add("Name");
            dt.Columns.Add("Group");
            foreach (var group in groups)
            {
                var row = dt.NewRow();
                row["Name"] = Utils.ClearHtml(group.NickName);
                row["Group"] = group.UserName;
                dt.Rows.Add(row);
            }

            comboBox_group.DataSource = dt;
            comboBox_group.DisplayMember = "Name";
            comboBox_group.ValueMember = "Group";
            comboBox_group.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox_group.Enabled = true;
            button_run.Enabled = true;
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
            if (comboBox_group.SelectedValue != null)
            {
                if (button_run.Text == "启动")
                {
                    button_run.Text = "停止";
                    app.RunGame(comboBox_group.SelectedValue as String);
                }
                else
                {
                    button_run.Text = "启动";
                    app.StopGame();
                }
            }

        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {

        }
    }
}

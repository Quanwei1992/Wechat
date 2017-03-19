using System;
using System.Windows.Forms;
using System.Data;
using System.Text.RegularExpressions;

namespace TestWechatGame
{
    public partial class MainForm : Form
    {


        public MainForm()
        {
            InitializeComponent();

            

        }


        private Wechat.WeChatClient wechat = new Wechat.WeChatClient();
        private Robot mRobot = null;
        private Lottery.LotteryCenter lottery = new Lottery.LotteryCenter();
        private void MainForm_Load(object sender, EventArgs e)
        {

            comboBox_contact.Enabled = false;
            comboBox_group.Enabled = false;
            button_run.Enabled = false;
            button_test.Enabled = false;
            wechat.OnEvent += OnWechatAppEvent;
            wechat.Run();
            lottery.Run();
            mRobot = new Robot(wechat,lottery);

        }


        private void OnWechatAppEvent(Wechat.WeChatClient sender, Wechat.WeChatClientEvent e)
        {

            RunInMainthread(() => {
                if (e is Wechat.GetQRCodeImageEvent)
                {
                    pictureBox_wechat.Image = (e as Wechat.GetQRCodeImageEvent).QRImage;
                }
                if (e is Wechat.UserScanQRCodeEvent)
                {
                    pictureBox_wechat.Image = (e as Wechat.UserScanQRCodeEvent).UserAvatarImage;
                }
                if (e is Wechat.LoginSucessEvent)
                {
                    Console.WriteLine("登陆成功！");
                }
                if (e is Wechat.AddMessageEvent)
                {
                    var message = (e as Wechat.AddMessageEvent).Msg;
                    Console.WriteLine("AddMsg:" + message.GetType());
                }
                if (e is Wechat.StatusChangedEvent)
                {
                    var changedEvent = (e as Wechat.StatusChangedEvent);
                    if (changedEvent.ToStatus != Wechat.ClientStatusType.WeixinSync)
                    {
                        comboBox_contact.Enabled = false;
                        comboBox_group.Enabled = false;
                        button_run.Enabled = false;
                        button_test.Enabled  = false;
                    }
                    Console.WriteLine(string.Format( "StatusChanged {0} -> {1}",changedEvent.FromStatus,changedEvent.ToStatus));
                }
                if (e is Wechat.InitedEvent) {
                    ShowGroups();
                    ShowContacts();
                }
            });
        }


        private void ShowGroups()
        {
            var groups = wechat.Groups;
            DataTable dt = new DataTable();
            dt.Columns.Add("Name");
            dt.Columns.Add("ID");
            foreach (var group in groups)
            {
                var row = dt.NewRow();
                row["Name"] = Utils.ClearHtml(group.NickName);
                row["ID"] = group.ID;
                dt.Rows.Add(row);
            }

            comboBox_group.DataSource = dt;
            comboBox_group.DisplayMember = "Name";
            comboBox_group.ValueMember = "ID";
            comboBox_group.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox_group.Enabled = true;
            button_run.Enabled = true;
        }


        private void ShowContacts()
        {
            var contacts = wechat.Contacts;
            DataTable dt = new DataTable();
            dt.Columns.Add("Name");
            dt.Columns.Add("ID");
            foreach (var contact in contacts)
            {
                var row = dt.NewRow();
                row["Name"] = Utils.ClearHtml(contact.NickName);
                row["ID"] = contact.ID;
                dt.Rows.Add(row);
            }

            comboBox_contact.DataSource = dt;
            comboBox_contact.DisplayMember = "Name";
            comboBox_contact.ValueMember = "ID";
            comboBox_contact.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox_contact.Enabled = true;
            button_test.Enabled = true;
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
                    mRobot.Run(comboBox_group.SelectedValue as String);
                }
                else
                {
                    button_run.Text = "启动";
                    mRobot.Stop();
                }
            }

        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            lottery.Stop();
            wechat.Quit(true);
        }

        private void button_test_Click(object sender, EventArgs e)
        {
            string uid = comboBox_contact.SelectedValue as string;
            wechat.SetRemarkName(uid,"梦(#123456789)");
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
    }
}

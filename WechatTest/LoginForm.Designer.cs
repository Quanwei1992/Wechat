namespace WechatTest
{
    partial class LoginForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.pictureBox_qr = new System.Windows.Forms.PictureBox();
            this.comboBox_users = new System.Windows.Forms.ComboBox();
            this.button_send_msg_txt = new System.Windows.Forms.Button();
            this.textBox_msg_text = new System.Windows.Forms.TextBox();
            this.button_send_msg_image = new System.Windows.Forms.Button();
            this.label_status = new System.Windows.Forms.Label();
            this.button_refreshGroupMemberInfo = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_qr)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox_qr
            // 
            this.pictureBox_qr.Location = new System.Drawing.Point(77, 50);
            this.pictureBox_qr.Name = "pictureBox_qr";
            this.pictureBox_qr.Size = new System.Drawing.Size(228, 202);
            this.pictureBox_qr.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox_qr.TabIndex = 0;
            this.pictureBox_qr.TabStop = false;
            // 
            // comboBox_users
            // 
            this.comboBox_users.FormattingEnabled = true;
            this.comboBox_users.Location = new System.Drawing.Point(12, 312);
            this.comboBox_users.Name = "comboBox_users";
            this.comboBox_users.Size = new System.Drawing.Size(365, 20);
            this.comboBox_users.TabIndex = 1;
            // 
            // button_send_msg_txt
            // 
            this.button_send_msg_txt.Location = new System.Drawing.Point(153, 352);
            this.button_send_msg_txt.Name = "button_send_msg_txt";
            this.button_send_msg_txt.Size = new System.Drawing.Size(99, 23);
            this.button_send_msg_txt.TabIndex = 2;
            this.button_send_msg_txt.Text = "发送文字消息";
            this.button_send_msg_txt.UseVisualStyleBackColor = true;
            this.button_send_msg_txt.Click += new System.EventHandler(this.button_send_msg_txt_Click);
            // 
            // textBox_msg_text
            // 
            this.textBox_msg_text.Location = new System.Drawing.Point(24, 353);
            this.textBox_msg_text.Name = "textBox_msg_text";
            this.textBox_msg_text.Size = new System.Drawing.Size(123, 21);
            this.textBox_msg_text.TabIndex = 3;
            // 
            // button_send_msg_image
            // 
            this.button_send_msg_image.Location = new System.Drawing.Point(258, 352);
            this.button_send_msg_image.Name = "button_send_msg_image";
            this.button_send_msg_image.Size = new System.Drawing.Size(119, 23);
            this.button_send_msg_image.TabIndex = 4;
            this.button_send_msg_image.Text = "发送图片消息";
            this.button_send_msg_image.UseVisualStyleBackColor = true;
            this.button_send_msg_image.Click += new System.EventHandler(this.button_send_msg_image_Click);
            // 
            // label_status
            // 
            this.label_status.Location = new System.Drawing.Point(12, 256);
            this.label_status.Name = "label_status";
            this.label_status.Size = new System.Drawing.Size(350, 53);
            this.label_status.TabIndex = 5;
            this.label_status.Text = "扫描二维码登陆微信";
            this.label_status.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // button_refreshGroupMemberInfo
            // 
            this.button_refreshGroupMemberInfo.Location = new System.Drawing.Point(24, 381);
            this.button_refreshGroupMemberInfo.Name = "button_refreshGroupMemberInfo";
            this.button_refreshGroupMemberInfo.Size = new System.Drawing.Size(107, 23);
            this.button_refreshGroupMemberInfo.TabIndex = 6;
            this.button_refreshGroupMemberInfo.Text = "刷新群聊成员";
            this.button_refreshGroupMemberInfo.UseVisualStyleBackColor = true;
            this.button_refreshGroupMemberInfo.Click += new System.EventHandler(this.button_refreshGroupMemberInfo_Click);
            // 
            // LoginForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(411, 461);
            this.Controls.Add(this.button_refreshGroupMemberInfo);
            this.Controls.Add(this.label_status);
            this.Controls.Add(this.button_send_msg_image);
            this.Controls.Add(this.textBox_msg_text);
            this.Controls.Add(this.button_send_msg_txt);
            this.Controls.Add(this.comboBox_users);
            this.Controls.Add(this.pictureBox_qr);
            this.Name = "LoginForm";
            this.Text = "WeChat";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_qr)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox_qr;
        private System.Windows.Forms.ComboBox comboBox_users;
        private System.Windows.Forms.Button button_send_msg_txt;
        private System.Windows.Forms.TextBox textBox_msg_text;
        private System.Windows.Forms.Button button_send_msg_image;
        private System.Windows.Forms.Label label_status;
        private System.Windows.Forms.Button button_refreshGroupMemberInfo;
    }
}


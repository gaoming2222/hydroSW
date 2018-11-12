namespace Hydrology.Forms
{
    partial class CBeidouNewMgrForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CBeidouNewMgrForm));
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.textBox_Communicate = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox_SendAddress = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.panel7 = new System.Windows.Forms.Panel();
            this.btnSetParam = new System.Windows.Forms.Button();
            this.cmbReceiptType = new System.Windows.Forms.ComboBox();
            this.label11 = new System.Windows.Forms.Label();
            this.cmbRespBeam = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.cmbBaudRate = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.panel5 = new System.Windows.Forms.Panel();
            this.panel6 = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.listView1 = new System.Windows.Forms.ListView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.lblAdjustBeam = new System.Windows.Forms.Label();
            this.panelStatusInfo = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.comboBox_Baurate = new System.Windows.Forms.ComboBox();
            this.label_Baurate = new System.Windows.Forms.Label();
            this.btnNormalState = new System.Windows.Forms.Button();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.btnStop = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.btnStart = new System.Windows.Forms.Button();
            this.panel14 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.label10 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.panel7.SuspendLayout();
            this.panel5.SuspendLayout();
            this.panel6.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel14.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(276, 121);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(134, 32);
            this.button2.TabIndex = 14;
            this.button2.Text = "授时申请";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(54, 121);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(132, 32);
            this.button1.TabIndex = 13;
            this.button1.Text = "本地通信";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // textBox_Communicate
            // 
            this.textBox_Communicate.Location = new System.Drawing.Point(127, 66);
            this.textBox_Communicate.Name = "textBox_Communicate";
            this.textBox_Communicate.Size = new System.Drawing.Size(337, 21);
            this.textBox_Communicate.TabIndex = 12;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(60, 75);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 11;
            this.label2.Text = "通讯报文：";
            // 
            // textBox_SendAddress
            // 
            this.textBox_SendAddress.Location = new System.Drawing.Point(127, 11);
            this.textBox_SendAddress.Name = "textBox_SendAddress";
            this.textBox_SendAddress.Size = new System.Drawing.Size(337, 21);
            this.textBox_SendAddress.TabIndex = 10;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(60, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 9;
            this.label1.Text = "发送地址：";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(12, 27);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(537, 568);
            this.tabControl1.TabIndex = 17;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.panel7);
            this.tabPage1.Controls.Add(this.panel5);
            this.tabPage1.Controls.Add(this.panel3);
            this.tabPage1.Controls.Add(this.panel1);
            this.tabPage1.Controls.Add(this.panel14);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(529, 542);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "查询";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // panel7
            // 
            this.panel7.Controls.Add(this.btnSetParam);
            this.panel7.Controls.Add(this.cmbReceiptType);
            this.panel7.Controls.Add(this.label11);
            this.panel7.Controls.Add(this.cmbRespBeam);
            this.panel7.Controls.Add(this.label8);
            this.panel7.Controls.Add(this.cmbBaudRate);
            this.panel7.Controls.Add(this.label9);
            this.panel7.Location = new System.Drawing.Point(6, 427);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(517, 99);
            this.panel7.TabIndex = 36;
            // 
            // btnSetParam
            // 
            this.btnSetParam.Location = new System.Drawing.Point(339, 27);
            this.btnSetParam.Name = "btnSetParam";
            this.btnSetParam.Size = new System.Drawing.Size(88, 32);
            this.btnSetParam.TabIndex = 22;
            this.btnSetParam.Text = "设置";
            this.btnSetParam.UseVisualStyleBackColor = true;
            this.btnSetParam.Click += new System.EventHandler(this.btnSetParam_Click);
            // 
            // cmbReceiptType
            // 
            this.cmbReceiptType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbReceiptType.FormattingEnabled = true;
            this.cmbReceiptType.Location = new System.Drawing.Point(102, 68);
            this.cmbReceiptType.Name = "cmbReceiptType";
            this.cmbReceiptType.Size = new System.Drawing.Size(200, 20);
            this.cmbReceiptType.TabIndex = 21;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(13, 71);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(77, 12);
            this.label11.TabIndex = 20;
            this.label11.Text = "回执类型  ：";
            // 
            // cmbRespBeam
            // 
            this.cmbRespBeam.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbRespBeam.FormattingEnabled = true;
            this.cmbRespBeam.Location = new System.Drawing.Point(102, 34);
            this.cmbRespBeam.Name = "cmbRespBeam";
            this.cmbRespBeam.Size = new System.Drawing.Size(200, 20);
            this.cmbRespBeam.TabIndex = 19;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(13, 42);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(77, 12);
            this.label8.TabIndex = 18;
            this.label8.Text = "响应波束  ：";
            // 
            // cmbBaudRate
            // 
            this.cmbBaudRate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBaudRate.FormattingEnabled = true;
            this.cmbBaudRate.Location = new System.Drawing.Point(102, 7);
            this.cmbBaudRate.Name = "cmbBaudRate";
            this.cmbBaudRate.Size = new System.Drawing.Size(200, 20);
            this.cmbBaudRate.TabIndex = 17;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(13, 10);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(77, 12);
            this.label9.TabIndex = 16;
            this.label9.Text = "串口波特率：";
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.panel6);
            this.panel5.Location = new System.Drawing.Point(6, 394);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(517, 26);
            this.panel5.TabIndex = 35;
            // 
            // panel6
            // 
            this.panel6.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.panel6.Controls.Add(this.label4);
            this.panel6.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel6.Location = new System.Drawing.Point(0, 0);
            this.panel6.Margin = new System.Windows.Forms.Padding(0);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(517, 21);
            this.panel6.TabIndex = 10;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 4);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 12);
            this.label4.TabIndex = 0;
            this.label4.Text = "参数设置";
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.listView1);
            this.panel3.Location = new System.Drawing.Point(3, 140);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(520, 248);
            this.panel3.TabIndex = 34;
            // 
            // listView1
            // 
            this.listView1.Location = new System.Drawing.Point(3, 3);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(505, 229);
            this.listView1.TabIndex = 0;
            this.listView1.UseCompatibleStateImageBehavior = false;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.panel4);
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Location = new System.Drawing.Point(3, 25);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(520, 109);
            this.panel1.TabIndex = 17;
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.panel4.Controls.Add(this.lblAdjustBeam);
            this.panel4.Controls.Add(this.panelStatusInfo);
            this.panel4.Controls.Add(this.label5);
            this.panel4.Location = new System.Drawing.Point(0, 77);
            this.panel4.Margin = new System.Windows.Forms.Padding(0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(517, 22);
            this.panel4.TabIndex = 33;
            // 
            // lblAdjustBeam
            // 
            this.lblAdjustBeam.AutoSize = true;
            this.lblAdjustBeam.ForeColor = System.Drawing.Color.Red;
            this.lblAdjustBeam.Location = new System.Drawing.Point(340, 4);
            this.lblAdjustBeam.Name = "lblAdjustBeam";
            this.lblAdjustBeam.Size = new System.Drawing.Size(149, 12);
            this.lblAdjustBeam.TabIndex = 35;
            this.lblAdjustBeam.Text = "波束1~6的波束强度均小于3";
            // 
            // panelStatusInfo
            // 
            this.panelStatusInfo.Location = new System.Drawing.Point(3, 24);
            this.panelStatusInfo.Name = "panelStatusInfo";
            this.panelStatusInfo.Size = new System.Drawing.Size(502, 102);
            this.panelStatusInfo.TabIndex = 18;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(13, 4);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 12);
            this.label5.TabIndex = 0;
            this.label5.Text = "状态信息";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.comboBox_Baurate);
            this.panel2.Controls.Add(this.label_Baurate);
            this.panel2.Controls.Add(this.btnNormalState);
            this.panel2.Controls.Add(this.comboBox1);
            this.panel2.Controls.Add(this.btnStop);
            this.panel2.Controls.Add(this.label6);
            this.panel2.Controls.Add(this.btnStart);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(520, 68);
            this.panel2.TabIndex = 7;
            // 
            // comboBox_Baurate
            // 
            this.comboBox_Baurate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_Baurate.FormattingEnabled = true;
            this.comboBox_Baurate.Location = new System.Drawing.Point(105, 62);
            this.comboBox_Baurate.Name = "comboBox_Baurate";
            this.comboBox_Baurate.Size = new System.Drawing.Size(200, 20);
            this.comboBox_Baurate.TabIndex = 14;
            // 
            // label_Baurate
            // 
            this.label_Baurate.AutoSize = true;
            this.label_Baurate.Location = new System.Drawing.Point(16, 65);
            this.label_Baurate.Name = "label_Baurate";
            this.label_Baurate.Size = new System.Drawing.Size(53, 12);
            this.label_Baurate.TabIndex = 13;
            this.label_Baurate.Text = "波特率：";
            // 
            // btnNormalState
            // 
            this.btnNormalState.Location = new System.Drawing.Point(342, 11);
            this.btnNormalState.Name = "btnNormalState";
            this.btnNormalState.Size = new System.Drawing.Size(97, 26);
            this.btnNormalState.TabIndex = 12;
            this.btnNormalState.Text = "查询终端状态";
            this.btnNormalState.UseVisualStyleBackColor = true;
            this.btnNormalState.Click += new System.EventHandler(this.btnNormalState_Click);
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(105, 15);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(200, 20);
            this.comboBox1.TabIndex = 11;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(342, 70);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(88, 24);
            this.btnStop.TabIndex = 10;
            this.btnStop.Text = "关闭串口";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Visible = false;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(16, 18);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(65, 12);
            this.label6.TabIndex = 9;
            this.label6.Text = "侦听串口：";
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(342, 43);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(88, 21);
            this.btnStart.TabIndex = 9;
            this.btnStart.Text = "打开串口";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Visible = false;
            // 
            // panel14
            // 
            this.panel14.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.panel14.Controls.Add(this.label3);
            this.panel14.Location = new System.Drawing.Point(3, 3);
            this.panel14.Margin = new System.Windows.Forms.Padding(0);
            this.panel14.Name = "panel14";
            this.panel14.Size = new System.Drawing.Size(520, 19);
            this.panel14.TabIndex = 16;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 4);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 12);
            this.label3.TabIndex = 0;
            this.label3.Text = "配置";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.textBox_Communicate);
            this.tabPage2.Controls.Add(this.label1);
            this.tabPage2.Controls.Add(this.textBox_SendAddress);
            this.tabPage2.Controls.Add(this.button2);
            this.tabPage2.Controls.Add(this.label2);
            this.tabPage2.Controls.Add(this.button1);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(529, 542);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "通讯";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(13, 4);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(53, 12);
            this.label10.TabIndex = 0;
            this.label10.Text = "参数设置";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(13, 4);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(29, 12);
            this.label7.TabIndex = 0;
            this.label7.Text = "配置";
            // 
            // CBeidouNewMgrForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(553, 601);
            this.Controls.Add(this.tabControl1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "CBeidouNewMgrForm";
            this.Text = "北斗卫星终端（普通）";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.panel7.ResumeLayout(false);
            this.panel7.PerformLayout();
            this.panel5.ResumeLayout(false);
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel14.ResumeLayout(false);
            this.panel14.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox textBox_Communicate;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox_SendAddress;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Panel panel14;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.ComboBox comboBox_Baurate;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Label lblAdjustBeam;
        private System.Windows.Forms.Panel panelStatusInfo;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label_Baurate;
        private System.Windows.Forms.Button btnNormalState;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.Button btnSetParam;
        private System.Windows.Forms.ComboBox cmbReceiptType;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.ComboBox cmbRespBeam;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox cmbBaudRate;
        private System.Windows.Forms.Label label9;
    }
}
namespace Hydrology.Forms
{
    partial class CDatabaseConfigForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CDatabaseConfigForm));
            this.panel1 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.chkChangeDBConfig = new System.Windows.Forms.CheckBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.chb_RembmerPassword = new System.Windows.Forms.CheckBox();
            this.lbl_Password = new System.Windows.Forms.Label();
            this.lbl_UserName = new System.Windows.Forms.Label();
            this.lbl_DBName = new System.Windows.Forms.Label();
            this.lbl_DatasourceName = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.btn_Advance = new System.Windows.Forms.Button();
            this.btn_Cancel = new System.Windows.Forms.Button();
            this.btn_Okay = new System.Windows.Forms.Button();
            this.btn_ConnTest = new System.Windows.Forms.Button();
            this.textBox_DataSource = new System.Windows.Forms.TextBox();
            this.textBox_Password = new System.Windows.Forms.TextBox();
            this.textBox_UserName = new System.Windows.Forms.TextBox();
            this.cmb_DatabaseType = new System.Windows.Forms.ComboBox();
            this.textBox_DataBaseName = new System.Windows.Forms.TextBox();
            this.textBox_ConnectionString = new System.Windows.Forms.TextBox();
            this.ConnectString = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.tableLayoutPanel1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(410, 335);
            this.panel1.TabIndex = 0;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.panel2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel3, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(410, 335);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.ConnectString);
            this.panel2.Controls.Add(this.textBox_ConnectionString);
            this.panel2.Controls.Add(this.textBox_DataBaseName);
            this.panel2.Controls.Add(this.cmb_DatabaseType);
            this.panel2.Controls.Add(this.textBox_Password);
            this.panel2.Controls.Add(this.textBox_UserName);
            this.panel2.Controls.Add(this.textBox_DataSource);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.chkChangeDBConfig);
            this.panel2.Controls.Add(this.pictureBox1);
            this.panel2.Controls.Add(this.chb_RembmerPassword);
            this.panel2.Controls.Add(this.lbl_Password);
            this.panel2.Controls.Add(this.lbl_UserName);
            this.panel2.Controls.Add(this.lbl_DBName);
            this.panel2.Controls.Add(this.lbl_DatasourceName);
            this.panel2.Controls.Add(this.groupBox1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Margin = new System.Windows.Forms.Padding(0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(410, 300);
            this.panel2.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(72, 95);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 12);
            this.label1.TabIndex = 14;
            this.label1.Text = "数据库类型:";
            // 
            // chkChangeDBConfig
            // 
            this.chkChangeDBConfig.AutoSize = true;
            this.chkChangeDBConfig.Location = new System.Drawing.Point(272, 258);
            this.chkChangeDBConfig.Name = "chkChangeDBConfig";
            this.chkChangeDBConfig.Size = new System.Drawing.Size(108, 16);
            this.chkChangeDBConfig.TabIndex = 13;
            this.chkChangeDBConfig.Text = "修改数据库配置";
            this.chkChangeDBConfig.UseVisualStyleBackColor = true;
            this.chkChangeDBConfig.CheckedChanged += new System.EventHandler(this.chkChangeDBConfig_CheckedChanged);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.pictureBox1.Image = global::Hydrology.Properties.Resources.数据库配置;
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(410, 81);
            this.pictureBox1.TabIndex = 10;
            this.pictureBox1.TabStop = false;
            // 
            // chb_RembmerPassword
            // 
            this.chb_RembmerPassword.AutoSize = true;
            this.chb_RembmerPassword.Location = new System.Drawing.Point(162, 258);
            this.chb_RembmerPassword.Name = "chb_RembmerPassword";
            this.chb_RembmerPassword.Size = new System.Drawing.Size(72, 16);
            this.chb_RembmerPassword.TabIndex = 9;
            this.chb_RembmerPassword.Text = "记住密码";
            this.chb_RembmerPassword.UseVisualStyleBackColor = true;
            // 
            // lbl_Password
            // 
            this.lbl_Password.AutoSize = true;
            this.lbl_Password.Location = new System.Drawing.Point(72, 225);
            this.lbl_Password.Name = "lbl_Password";
            this.lbl_Password.Size = new System.Drawing.Size(41, 12);
            this.lbl_Password.TabIndex = 7;
            this.lbl_Password.Text = "密码：";
            // 
            // lbl_UserName
            // 
            this.lbl_UserName.AutoSize = true;
            this.lbl_UserName.Location = new System.Drawing.Point(72, 191);
            this.lbl_UserName.Name = "lbl_UserName";
            this.lbl_UserName.Size = new System.Drawing.Size(53, 12);
            this.lbl_UserName.TabIndex = 5;
            this.lbl_UserName.Text = "用户名：";
            // 
            // lbl_DBName
            // 
            this.lbl_DBName.AutoSize = true;
            this.lbl_DBName.Location = new System.Drawing.Point(72, 157);
            this.lbl_DBName.Name = "lbl_DBName";
            this.lbl_DBName.Size = new System.Drawing.Size(59, 12);
            this.lbl_DBName.TabIndex = 3;
            this.lbl_DBName.Text = "数据库名:";
            // 
            // lbl_DatasourceName
            // 
            this.lbl_DatasourceName.AutoSize = true;
            this.lbl_DatasourceName.Location = new System.Drawing.Point(72, 123);
            this.lbl_DatasourceName.Name = "lbl_DatasourceName";
            this.lbl_DatasourceName.Size = new System.Drawing.Size(59, 12);
            this.lbl_DatasourceName.TabIndex = 1;
            this.lbl_DatasourceName.Text = "数据源名:";
            // 
            // groupBox1
            // 
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.groupBox1.Location = new System.Drawing.Point(0, 290);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(410, 10);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.btn_Advance);
            this.panel3.Controls.Add(this.btn_Cancel);
            this.panel3.Controls.Add(this.btn_Okay);
            this.panel3.Controls.Add(this.btn_ConnTest);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(3, 303);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(404, 29);
            this.panel3.TabIndex = 1;
            // 
            // btn_Advance
            // 
            this.btn_Advance.Location = new System.Drawing.Point(318, 1);
            this.btn_Advance.Name = "btn_Advance";
            this.btn_Advance.Size = new System.Drawing.Size(75, 26);
            this.btn_Advance.TabIndex = 3;
            this.btn_Advance.Text = "高级 >>";
            this.btn_Advance.UseVisualStyleBackColor = true;
            this.btn_Advance.Click += new System.EventHandler(this.btn_Advance_Click);
            // 
            // btn_Cancel
            // 
            this.btn_Cancel.Location = new System.Drawing.Point(237, 1);
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.Size = new System.Drawing.Size(75, 26);
            this.btn_Cancel.TabIndex = 2;
            this.btn_Cancel.Text = "取消";
            this.btn_Cancel.UseVisualStyleBackColor = true;
            this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
            // 
            // btn_Okay
            // 
            this.btn_Okay.Location = new System.Drawing.Point(147, 1);
            this.btn_Okay.Name = "btn_Okay";
            this.btn_Okay.Size = new System.Drawing.Size(75, 26);
            this.btn_Okay.TabIndex = 1;
            this.btn_Okay.Text = "确定";
            this.btn_Okay.UseVisualStyleBackColor = true;
            this.btn_Okay.Click += new System.EventHandler(this.btn_Okay_Click);
            // 
            // btn_ConnTest
            // 
            this.btn_ConnTest.Location = new System.Drawing.Point(9, 1);
            this.btn_ConnTest.Name = "btn_ConnTest";
            this.btn_ConnTest.Size = new System.Drawing.Size(85, 26);
            this.btn_ConnTest.TabIndex = 0;
            this.btn_ConnTest.Text = "测试连接...";
            this.btn_ConnTest.UseVisualStyleBackColor = true;
            this.btn_ConnTest.Click += new System.EventHandler(this.btn_ConnTest_Click);
            // 
            // textBox_DataSource
            // 
            this.textBox_DataSource.Location = new System.Drawing.Point(143, 123);
            this.textBox_DataSource.Name = "textBox_DataSource";
            this.textBox_DataSource.Size = new System.Drawing.Size(172, 21);
            this.textBox_DataSource.TabIndex = 16;
            // 
            // textBox_Password
            // 
            this.textBox_Password.Location = new System.Drawing.Point(143, 222);
            this.textBox_Password.Name = "textBox_Password";
            this.textBox_Password.PasswordChar = '*';
            this.textBox_Password.Size = new System.Drawing.Size(172, 21);
            this.textBox_Password.TabIndex = 19;
            // 
            // textBox_UserName
            // 
            this.textBox_UserName.Location = new System.Drawing.Point(143, 187);
            this.textBox_UserName.Name = "textBox_UserName";
            this.textBox_UserName.Size = new System.Drawing.Size(172, 21);
            this.textBox_UserName.TabIndex = 18;
            // 
            // cmb_DatabaseType
            // 
            this.cmb_DatabaseType.FormattingEnabled = true;
            this.cmb_DatabaseType.Location = new System.Drawing.Point(143, 95);
            this.cmb_DatabaseType.Name = "cmb_DatabaseType";
            this.cmb_DatabaseType.Size = new System.Drawing.Size(172, 20);
            this.cmb_DatabaseType.TabIndex = 20;
            // 
            // textBox_DataBaseName
            // 
            this.textBox_DataBaseName.Location = new System.Drawing.Point(143, 154);
            this.textBox_DataBaseName.Name = "textBox_DataBaseName";
            this.textBox_DataBaseName.Size = new System.Drawing.Size(172, 21);
            this.textBox_DataBaseName.TabIndex = 21;
            // 
            // textBox_ConnectionString
            // 
            this.textBox_ConnectionString.Location = new System.Drawing.Point(143, 120);
            this.textBox_ConnectionString.Multiline = true;
            this.textBox_ConnectionString.Name = "textBox_ConnectionString";
            this.textBox_ConnectionString.Size = new System.Drawing.Size(172, 91);
            this.textBox_ConnectionString.TabIndex = 22;
            // 
            // ConnectString
            // 
            this.ConnectString.AutoSize = true;
            this.ConnectString.Location = new System.Drawing.Point(72, 145);
            this.ConnectString.Name = "ConnectString";
            this.ConnectString.Size = new System.Drawing.Size(77, 12);
            this.ConnectString.TabIndex = 23;
            this.ConnectString.Text = "连接字符串：";
            // 
            // CDatabaseConfigForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(410, 335);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CDatabaseConfigForm";
            this.Text = "数据库配置";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CDatabaseConfigForm_FormClosing);
            this.panel1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panel3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lbl_DatasourceName;
        private System.Windows.Forms.Label lbl_Password;
        private System.Windows.Forms.Label lbl_UserName;
        private System.Windows.Forms.Label lbl_DBName;
        private System.Windows.Forms.Button btn_Advance;
        private System.Windows.Forms.Button btn_Cancel;
        private System.Windows.Forms.Button btn_Okay;
        private System.Windows.Forms.Button btn_ConnTest;
        private System.Windows.Forms.CheckBox chb_RembmerPassword;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.CheckBox chkChangeDBConfig;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox_Password;
        private System.Windows.Forms.TextBox textBox_UserName;
        private System.Windows.Forms.TextBox textBox_DataSource;
        private System.Windows.Forms.ComboBox cmb_DatabaseType;
        private System.Windows.Forms.TextBox textBox_DataBaseName;
        private System.Windows.Forms.Label ConnectString;
        private System.Windows.Forms.TextBox textBox_ConnectionString;

    }
}
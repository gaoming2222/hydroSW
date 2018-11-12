namespace Hydrology.Forms
{
    partial class CStationDataAddForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CStationDataAddForm));
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.btn_Apply = new System.Windows.Forms.Button();
            this.btn_Cancel = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label19 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.chk_Water = new System.Windows.Forms.CheckBox();
            this.chk_Rain = new System.Windows.Forms.CheckBox();
            this.chk_Voltage = new System.Windows.Forms.CheckBox();
            this.cmb_AddDataType = new System.Windows.Forms.ComboBox();
            this.label16 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.number_Voltage = new System.Windows.Forms.NumericUpDown();
            this.label15 = new System.Windows.Forms.Label();
            this.number_WaterFlow = new System.Windows.Forms.NumericUpDown();
            this.label14 = new System.Windows.Forms.Label();
            this.number_WaterStage = new System.Windows.Forms.NumericUpDown();
            this.label13 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.number_TotalRain = new System.Windows.Forms.NumericUpDown();
            this.number_DayRain = new System.Windows.Forms.NumericUpDown();
            this.number_PeriodRain = new System.Windows.Forms.NumericUpDown();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.dtp_TimeReceived = new System.Windows.Forms.DateTimePicker();
            this.dtp_CollectTime = new System.Windows.Forms.DateTimePicker();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.cmb_DataType = new System.Windows.Forms.ComboBox();
            this.cmb_ChannelType = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBox_StationType = new System.Windows.Forms.TextBox();
            this.cmb_StationId = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.number_Voltage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.number_WaterFlow)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.number_WaterStage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.number_TotalRain)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.number_DayRain)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.number_PeriodRain)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.groupBox4);
            this.panel1.Controls.Add(this.btn_Apply);
            this.panel1.Controls.Add(this.btn_Cancel);
            this.panel1.Controls.Add(this.groupBox3);
            this.panel1.Controls.Add(this.groupBox2);
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(583, 463);
            this.panel1.TabIndex = 1;
            // 
            // groupBox4
            // 
            this.groupBox4.Location = new System.Drawing.Point(0, 399);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(591, 10);
            this.groupBox4.TabIndex = 11;
            this.groupBox4.TabStop = false;
            // 
            // btn_Apply
            // 
            this.btn_Apply.Location = new System.Drawing.Point(456, 415);
            this.btn_Apply.Name = "btn_Apply";
            this.btn_Apply.Size = new System.Drawing.Size(117, 36);
            this.btn_Apply.TabIndex = 10;
            this.btn_Apply.Text = "完成添加";
            this.btn_Apply.UseVisualStyleBackColor = true;
            this.btn_Apply.Click += new System.EventHandler(this.btn_Apply_Click);
            // 
            // btn_Cancel
            // 
            this.btn_Cancel.Location = new System.Drawing.Point(308, 415);
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.Size = new System.Drawing.Size(117, 36);
            this.btn_Cancel.TabIndex = 9;
            this.btn_Cancel.Text = "取消添加";
            this.btn_Cancel.UseVisualStyleBackColor = true;
            this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label19);
            this.groupBox3.Controls.Add(this.label18);
            this.groupBox3.Controls.Add(this.chk_Water);
            this.groupBox3.Controls.Add(this.chk_Rain);
            this.groupBox3.Controls.Add(this.chk_Voltage);
            this.groupBox3.Controls.Add(this.cmb_AddDataType);
            this.groupBox3.Controls.Add(this.label16);
            this.groupBox3.Controls.Add(this.label17);
            this.groupBox3.Controls.Add(this.number_Voltage);
            this.groupBox3.Controls.Add(this.label15);
            this.groupBox3.Controls.Add(this.number_WaterFlow);
            this.groupBox3.Controls.Add(this.label14);
            this.groupBox3.Controls.Add(this.number_WaterStage);
            this.groupBox3.Controls.Add(this.label13);
            this.groupBox3.Controls.Add(this.label12);
            this.groupBox3.Controls.Add(this.label11);
            this.groupBox3.Controls.Add(this.label10);
            this.groupBox3.Controls.Add(this.number_TotalRain);
            this.groupBox3.Controls.Add(this.number_DayRain);
            this.groupBox3.Controls.Add(this.number_PeriodRain);
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Location = new System.Drawing.Point(3, 184);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(576, 176);
            this.groupBox3.TabIndex = 6;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "数据参数";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(362, 130);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(29, 12);
            this.label19.TabIndex = 25;
            this.label19.Text = "m³/s";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(178, 130);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(11, 12);
            this.label18.TabIndex = 24;
            this.label18.Text = "m";
            // 
            // chk_Water
            // 
            this.chk_Water.AutoSize = true;
            this.chk_Water.Location = new System.Drawing.Point(344, 25);
            this.chk_Water.Name = "chk_Water";
            this.chk_Water.Size = new System.Drawing.Size(48, 16);
            this.chk_Water.TabIndex = 23;
            this.chk_Water.Text = "水位";
            this.chk_Water.UseVisualStyleBackColor = true;
            // 
            // chk_Rain
            // 
            this.chk_Rain.AutoSize = true;
            this.chk_Rain.Location = new System.Drawing.Point(213, 25);
            this.chk_Rain.Name = "chk_Rain";
            this.chk_Rain.Size = new System.Drawing.Size(48, 16);
            this.chk_Rain.TabIndex = 22;
            this.chk_Rain.Text = "雨量";
            this.chk_Rain.UseVisualStyleBackColor = true;
            // 
            // chk_Voltage
            // 
            this.chk_Voltage.AutoSize = true;
            this.chk_Voltage.Location = new System.Drawing.Point(91, 25);
            this.chk_Voltage.Name = "chk_Voltage";
            this.chk_Voltage.Size = new System.Drawing.Size(48, 16);
            this.chk_Voltage.TabIndex = 21;
            this.chk_Voltage.Text = "电压";
            this.chk_Voltage.UseVisualStyleBackColor = true;
            // 
            // cmb_AddDataType
            // 
            this.cmb_AddDataType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_AddDataType.FormattingEnabled = true;
            this.cmb_AddDataType.Location = new System.Drawing.Point(88, 23);
            this.cmb_AddDataType.Name = "cmb_AddDataType";
            this.cmb_AddDataType.Size = new System.Drawing.Size(143, 20);
            this.cmb_AddDataType.TabIndex = 5;
            this.cmb_AddDataType.SelectedIndexChanged += new System.EventHandler(this.cmb_AddDataType_SelectedIndexChanged);
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(178, 58);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(11, 12);
            this.label16.TabIndex = 20;
            this.label16.Text = "v";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(17, 26);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(65, 12);
            this.label17.TabIndex = 4;
            this.label17.Text = "选择数据：";
            // 
            // number_Voltage
            // 
            this.number_Voltage.DecimalPlaces = 2;
            this.number_Voltage.Location = new System.Drawing.Point(88, 56);
            this.number_Voltage.Maximum = new decimal(new int[] {
            -1530494977,
            232830,
            0,
            0});
            this.number_Voltage.Name = "number_Voltage";
            this.number_Voltage.Size = new System.Drawing.Size(81, 21);
            this.number_Voltage.TabIndex = 19;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(17, 62);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(41, 12);
            this.label15.TabIndex = 18;
            this.label15.Text = "电压：";
            // 
            // number_WaterFlow
            // 
            this.number_WaterFlow.DecimalPlaces = 2;
            this.number_WaterFlow.Location = new System.Drawing.Point(270, 128);
            this.number_WaterFlow.Maximum = new decimal(new int[] {
            -1530494977,
            232830,
            0,
            0});
            this.number_WaterFlow.Name = "number_WaterFlow";
            this.number_WaterFlow.Size = new System.Drawing.Size(81, 21);
            this.number_WaterFlow.TabIndex = 17;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(211, 134);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(41, 12);
            this.label14.TabIndex = 16;
            this.label14.Text = "流量：";
            // 
            // number_WaterStage
            // 
            this.number_WaterStage.DecimalPlaces = 2;
            this.number_WaterStage.Location = new System.Drawing.Point(88, 128);
            this.number_WaterStage.Maximum = new decimal(new int[] {
            -1530494977,
            232830,
            0,
            0});
            this.number_WaterStage.Name = "number_WaterStage";
            this.number_WaterStage.Size = new System.Drawing.Size(81, 21);
            this.number_WaterStage.TabIndex = 15;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(17, 134);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(41, 12);
            this.label13.TabIndex = 14;
            this.label13.Text = "水位：";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(549, 95);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(17, 12);
            this.label12.TabIndex = 13;
            this.label12.Text = "mm";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(362, 95);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(17, 12);
            this.label11.TabIndex = 12;
            this.label11.Text = "mm";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(178, 95);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(17, 12);
            this.label10.TabIndex = 11;
            this.label10.Text = "mm";
            // 
            // number_TotalRain
            // 
            this.number_TotalRain.DecimalPlaces = 1;
            this.number_TotalRain.Location = new System.Drawing.Point(456, 90);
            this.number_TotalRain.Maximum = new decimal(new int[] {
            -1530494977,
            232830,
            0,
            0});
            this.number_TotalRain.Name = "number_TotalRain";
            this.number_TotalRain.Size = new System.Drawing.Size(81, 21);
            this.number_TotalRain.TabIndex = 10;
            // 
            // number_DayRain
            // 
            this.number_DayRain.DecimalPlaces = 1;
            this.number_DayRain.Enabled = false;
            this.number_DayRain.Location = new System.Drawing.Point(270, 90);
            this.number_DayRain.Maximum = new decimal(new int[] {
            -1530494977,
            232830,
            0,
            0});
            this.number_DayRain.Name = "number_DayRain";
            this.number_DayRain.Size = new System.Drawing.Size(81, 21);
            this.number_DayRain.TabIndex = 9;
            // 
            // number_PeriodRain
            // 
            this.number_PeriodRain.DecimalPlaces = 1;
            this.number_PeriodRain.Enabled = false;
            this.number_PeriodRain.Location = new System.Drawing.Point(88, 92);
            this.number_PeriodRain.Maximum = new decimal(new int[] {
            -1530494977,
            232830,
            0,
            0});
            this.number_PeriodRain.Name = "number_PeriodRain";
            this.number_PeriodRain.Size = new System.Drawing.Size(81, 21);
            this.number_PeriodRain.TabIndex = 8;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(385, 94);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(65, 12);
            this.label9.TabIndex = 7;
            this.label9.Text = "累计雨量：";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(211, 95);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(53, 12);
            this.label8.TabIndex = 6;
            this.label8.Text = "日雨量：";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(17, 95);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(65, 12);
            this.label7.TabIndex = 5;
            this.label7.Text = "时段雨量：";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.dtp_TimeReceived);
            this.groupBox2.Controls.Add(this.dtp_CollectTime);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.cmb_DataType);
            this.groupBox2.Controls.Add(this.cmb_ChannelType);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Location = new System.Drawing.Point(3, 74);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(576, 99);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "基本参数";
            // 
            // dtp_TimeReceived
            // 
            this.dtp_TimeReceived.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.dtp_TimeReceived.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtp_TimeReceived.Location = new System.Drawing.Point(362, 28);
            this.dtp_TimeReceived.Name = "dtp_TimeReceived";
            this.dtp_TimeReceived.Size = new System.Drawing.Size(153, 21);
            this.dtp_TimeReceived.TabIndex = 7;
            // 
            // dtp_CollectTime
            // 
            this.dtp_CollectTime.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.dtp_CollectTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtp_CollectTime.Location = new System.Drawing.Point(88, 26);
            this.dtp_CollectTime.Name = "dtp_CollectTime";
            this.dtp_CollectTime.Size = new System.Drawing.Size(164, 21);
            this.dtp_CollectTime.TabIndex = 6;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(293, 68);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(65, 12);
            this.label6.TabIndex = 5;
            this.label6.Text = "信道类型：";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(17, 28);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(65, 12);
            this.label5.TabIndex = 4;
            this.label5.Text = "采集时间：";
            // 
            // cmb_DataType
            // 
            this.cmb_DataType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_DataType.FormattingEnabled = true;
            this.cmb_DataType.Location = new System.Drawing.Point(88, 62);
            this.cmb_DataType.Name = "cmb_DataType";
            this.cmb_DataType.Size = new System.Drawing.Size(164, 20);
            this.cmb_DataType.TabIndex = 1;
            // 
            // cmb_ChannelType
            // 
            this.cmb_ChannelType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_ChannelType.FormattingEnabled = true;
            this.cmb_ChannelType.Location = new System.Drawing.Point(363, 65);
            this.cmb_ChannelType.Name = "cmb_ChannelType";
            this.cmb_ChannelType.Size = new System.Drawing.Size(152, 20);
            this.cmb_ChannelType.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(293, 32);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 12);
            this.label3.TabIndex = 2;
            this.label3.Text = "接收时间：";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(17, 65);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 12);
            this.label4.TabIndex = 0;
            this.label4.Text = "报文类型：";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textBox_StationType);
            this.groupBox1.Controls.Add(this.cmb_StationId);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(3, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(576, 56);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "选择站点";
            // 
            // textBox_StationType
            // 
            this.textBox_StationType.Location = new System.Drawing.Point(362, 19);
            this.textBox_StationType.Name = "textBox_StationType";
            this.textBox_StationType.ReadOnly = true;
            this.textBox_StationType.Size = new System.Drawing.Size(139, 21);
            this.textBox_StationType.TabIndex = 3;
            // 
            // cmb_StationId
            // 
            this.cmb_StationId.FormattingEnabled = true;
            this.cmb_StationId.Location = new System.Drawing.Point(88, 20);
            this.cmb_StationId.Name = "cmb_StationId";
            this.cmb_StationId.Size = new System.Drawing.Size(148, 20);
            this.cmb_StationId.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(294, 23);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "站类：";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "选择站点：";
            // 
            // CStationDataAddForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(583, 463);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CStationDataAddForm";
            this.Text = "添加站点数据";
            this.panel1.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.number_Voltage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.number_WaterFlow)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.number_WaterStage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.number_TotalRain)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.number_DayRain)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.number_PeriodRain)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmb_StationId;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ComboBox cmb_DataType;
        private System.Windows.Forms.ComboBox cmb_ChannelType;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.DateTimePicker dtp_CollectTime;
        private System.Windows.Forms.DateTimePicker dtp_TimeReceived;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button btn_Apply;
        private System.Windows.Forms.Button btn_Cancel;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.NumericUpDown number_PeriodRain;
        private System.Windows.Forms.NumericUpDown number_DayRain;
        private System.Windows.Forms.NumericUpDown number_TotalRain;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.NumericUpDown number_WaterFlow;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.NumericUpDown number_WaterStage;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.NumericUpDown number_Voltage;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.ComboBox cmb_AddDataType;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TextBox textBox_StationType;
        private System.Windows.Forms.CheckBox chk_Water;
        private System.Windows.Forms.CheckBox chk_Rain;
        private System.Windows.Forms.CheckBox chk_Voltage;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label18;
    }
}
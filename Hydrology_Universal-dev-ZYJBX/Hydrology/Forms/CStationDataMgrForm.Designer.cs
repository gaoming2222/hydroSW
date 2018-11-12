namespace Hydrology.Forms
{
    partial class CStationDataMgrForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CStationDataMgrForm));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.m_statusLable = new System.Windows.Forms.ToolStripStatusLabel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.panelLeft = new System.Windows.Forms.Panel();
            this.cmb_TimeSelect = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.cmb_SubCenter = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.cmb_RainShape = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.cmb_ViewStyle = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.btnNewRecord = new System.Windows.Forms.Button();
            this.btnApply = new System.Windows.Forms.Button();
            this.labelInfoSelect = new System.Windows.Forms.Label();
            this.btnExit = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btnQuery = new System.Windows.Forms.Button();
            this.dptTimeEnd = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.dtpTimeStart = new System.Windows.Forms.DateTimePicker();
            this.cmbQueryInfo = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.panelRight = new System.Windows.Forms.Panel();
            this.tLayoutRight = new System.Windows.Forms.TableLayoutPanel();
            this.panelBottom = new System.Windows.Forms.Panel();
            this.m_lableRowCount = new System.Windows.Forms.Label();
            this.m_lablePageIndex = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tableLayoutPanel.SuspendLayout();
            this.panelLeft.SuspendLayout();
            this.panelRight.SuspendLayout();
            this.tLayoutRight.SuspendLayout();
            this.panelBottom.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.BackColor = System.Drawing.Color.Transparent;
            this.tableLayoutPanel1.BackgroundImage = global::Hydrology.Properties.Resources.WhiteSpace;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.statusStrip1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1147, 624);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // statusStrip1
            // 
            this.statusStrip1.BackgroundImage = global::Hydrology.Properties.Resources.状态栏;
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_statusLable});
            this.statusStrip1.Location = new System.Drawing.Point(0, 602);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1147, 22);
            this.statusStrip1.TabIndex = 0;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // m_statusLable
            // 
            this.m_statusLable.ForeColor = System.Drawing.Color.White;
            this.m_statusLable.Name = "m_statusLable";
            this.m_statusLable.Size = new System.Drawing.Size(56, 17);
            this.m_statusLable.Text = "准备就绪";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.tableLayoutPanel);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1147, 599);
            this.panel1.TabIndex = 1;
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.tableLayoutPanel.BackgroundImage = global::Hydrology.Properties.Resources.WhiteSpace;
            this.tableLayoutPanel.ColumnCount = 2;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 200F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.Controls.Add(this.panelLeft, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.panelRight, 1, 0);
            this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 1;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 648F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 648F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 648F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 648F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 648F));
            this.tableLayoutPanel.Size = new System.Drawing.Size(1147, 599);
            this.tableLayoutPanel.TabIndex = 2;
            // 
            // panelLeft
            // 
            this.panelLeft.BackColor = System.Drawing.SystemColors.HighlightText;
            this.panelLeft.Controls.Add(this.cmb_TimeSelect);
            this.panelLeft.Controls.Add(this.label9);
            this.panelLeft.Controls.Add(this.cmb_SubCenter);
            this.panelLeft.Controls.Add(this.label7);
            this.panelLeft.Controls.Add(this.cmb_RainShape);
            this.panelLeft.Controls.Add(this.label6);
            this.panelLeft.Controls.Add(this.cmb_ViewStyle);
            this.panelLeft.Controls.Add(this.label5);
            this.panelLeft.Controls.Add(this.btnNewRecord);
            this.panelLeft.Controls.Add(this.btnApply);
            this.panelLeft.Controls.Add(this.labelInfoSelect);
            this.panelLeft.Controls.Add(this.btnExit);
            this.panelLeft.Controls.Add(this.label1);
            this.panelLeft.Controls.Add(this.btnQuery);
            this.panelLeft.Controls.Add(this.dptTimeEnd);
            this.panelLeft.Controls.Add(this.label2);
            this.panelLeft.Controls.Add(this.label4);
            this.panelLeft.Controls.Add(this.dtpTimeStart);
            this.panelLeft.Controls.Add(this.cmbQueryInfo);
            this.panelLeft.Controls.Add(this.label3);
            this.panelLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelLeft.Location = new System.Drawing.Point(0, 0);
            this.panelLeft.Margin = new System.Windows.Forms.Padding(0);
            this.panelLeft.Name = "panelLeft";
            this.panelLeft.Size = new System.Drawing.Size(200, 599);
            this.panelLeft.TabIndex = 10;
            // 
            // cmb_TimeSelect
            // 
            this.cmb_TimeSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_TimeSelect.FormattingEnabled = true;
            this.cmb_TimeSelect.Location = new System.Drawing.Point(80, 327);
            this.cmb_TimeSelect.Name = "cmb_TimeSelect";
            this.cmb_TimeSelect.Size = new System.Drawing.Size(117, 20);
            this.cmb_TimeSelect.TabIndex = 23;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(12, 331);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(65, 12);
            this.label9.TabIndex = 22;
            this.label9.Text = "整点选择：";
            // 
            // cmb_SubCenter
            // 
            this.cmb_SubCenter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_SubCenter.FormattingEnabled = true;
            this.cmb_SubCenter.Location = new System.Drawing.Point(76, 58);
            this.cmb_SubCenter.Name = "cmb_SubCenter";
            this.cmb_SubCenter.Size = new System.Drawing.Size(121, 20);
            this.cmb_SubCenter.TabIndex = 19;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.BackColor = System.Drawing.Color.Transparent;
            this.label7.Location = new System.Drawing.Point(12, 61);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(53, 12);
            this.label7.TabIndex = 18;
            this.label7.Text = "分中心：";
            // 
            // cmb_RainShape
            // 
            this.cmb_RainShape.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_RainShape.FormattingEnabled = true;
            this.cmb_RainShape.Location = new System.Drawing.Point(80, 292);
            this.cmb_RainShape.Name = "cmb_RainShape";
            this.cmb_RainShape.Size = new System.Drawing.Size(117, 20);
            this.cmb_RainShape.TabIndex = 16;
            this.cmb_RainShape.SelectedIndexChanged += new System.EventHandler(this.cmb_RainShape_SelectedIndexChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 296);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(65, 12);
            this.label6.TabIndex = 15;
            this.label6.Text = "雨量图形：";
            // 
            // cmb_ViewStyle
            // 
            this.cmb_ViewStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_ViewStyle.FormattingEnabled = true;
            this.cmb_ViewStyle.Location = new System.Drawing.Point(80, 258);
            this.cmb_ViewStyle.Name = "cmb_ViewStyle";
            this.cmb_ViewStyle.Size = new System.Drawing.Size(117, 20);
            this.cmb_ViewStyle.TabIndex = 14;
            this.cmb_ViewStyle.SelectedIndexChanged += new System.EventHandler(this.cmb_ViewStyle_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 262);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(65, 12);
            this.label5.TabIndex = 13;
            this.label5.Text = "图表选择：";
            // 
            // btnNewRecord
            // 
            this.btnNewRecord.Location = new System.Drawing.Point(115, 400);
            this.btnNewRecord.Name = "btnNewRecord";
            this.btnNewRecord.Size = new System.Drawing.Size(75, 23);
            this.btnNewRecord.TabIndex = 12;
            this.btnNewRecord.Text = "新增...";
            this.btnNewRecord.UseVisualStyleBackColor = true;
            this.btnNewRecord.Click += new System.EventHandler(this.btnNewRecord_Click);
            // 
            // btnApply
            // 
            this.btnApply.Location = new System.Drawing.Point(14, 400);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(75, 23);
            this.btnApply.TabIndex = 11;
            this.btnApply.Text = "保存";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // labelInfoSelect
            // 
            this.labelInfoSelect.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.labelInfoSelect.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelInfoSelect.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelInfoSelect.Location = new System.Drawing.Point(0, 0);
            this.labelInfoSelect.Name = "labelInfoSelect";
            this.labelInfoSelect.Padding = new System.Windows.Forms.Padding(5);
            this.labelInfoSelect.Size = new System.Drawing.Size(200, 25);
            this.labelInfoSelect.TabIndex = 10;
            this.labelInfoSelect.Text = "配置查询信息";
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(115, 362);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 23);
            this.btnExit.TabIndex = 9;
            this.btnExit.Text = "退出";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 104);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "测站：";
            // 
            // btnQuery
            // 
            this.btnQuery.Location = new System.Drawing.Point(14, 362);
            this.btnQuery.Name = "btnQuery";
            this.btnQuery.Size = new System.Drawing.Size(75, 23);
            this.btnQuery.TabIndex = 8;
            this.btnQuery.Text = "查询";
            this.btnQuery.UseVisualStyleBackColor = true;
            this.btnQuery.Click += new System.EventHandler(this.btnQuery_Click);
            // 
            // dptTimeEnd
            // 
            this.dptTimeEnd.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.dptTimeEnd.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dptTimeEnd.Location = new System.Drawing.Point(80, 214);
            this.dptTimeEnd.Name = "dptTimeEnd";
            this.dptTimeEnd.ShowUpDown = true;
            this.dptTimeEnd.Size = new System.Drawing.Size(117, 21);
            this.dptTimeEnd.TabIndex = 7;
            this.dptTimeEnd.Value = new System.DateTime(2011, 1, 1, 23, 59, 0, 0);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 142);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "查询信息：";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 218);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 12);
            this.label4.TabIndex = 5;
            this.label4.Text = "结束时间：";
            // 
            // dtpTimeStart
            // 
            this.dtpTimeStart.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.dtpTimeStart.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpTimeStart.Location = new System.Drawing.Point(80, 174);
            this.dtpTimeStart.Name = "dtpTimeStart";
            this.dtpTimeStart.ShowUpDown = true;
            this.dtpTimeStart.Size = new System.Drawing.Size(117, 21);
            this.dtpTimeStart.TabIndex = 6;
            this.dtpTimeStart.Value = new System.DateTime(2010, 1, 1, 0, 0, 0, 0);
            // 
            // cmbQueryInfo
            // 
            this.cmbQueryInfo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbQueryInfo.FormattingEnabled = true;
            this.cmbQueryInfo.Location = new System.Drawing.Point(80, 135);
            this.cmbQueryInfo.Name = "cmbQueryInfo";
            this.cmbQueryInfo.Size = new System.Drawing.Size(117, 20);
            this.cmbQueryInfo.TabIndex = 3;
            this.cmbQueryInfo.SelectedIndexChanged += new System.EventHandler(this.cmbQueryInfo_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 180);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 12);
            this.label3.TabIndex = 4;
            this.label3.Text = "起始时间：";
            // 
            // panelRight
            // 
            this.panelRight.Controls.Add(this.tLayoutRight);
            this.panelRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelRight.Location = new System.Drawing.Point(200, 0);
            this.panelRight.Margin = new System.Windows.Forms.Padding(0);
            this.panelRight.Name = "panelRight";
            this.panelRight.Size = new System.Drawing.Size(947, 599);
            this.panelRight.TabIndex = 11;
            // 
            // tLayoutRight
            // 
            this.tLayoutRight.ColumnCount = 1;
            this.tLayoutRight.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tLayoutRight.Controls.Add(this.panelBottom, 0, 1);
            this.tLayoutRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tLayoutRight.Location = new System.Drawing.Point(0, 0);
            this.tLayoutRight.Margin = new System.Windows.Forms.Padding(0);
            this.tLayoutRight.Name = "tLayoutRight";
            this.tLayoutRight.RowCount = 3;
            this.tLayoutRight.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tLayoutRight.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tLayoutRight.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tLayoutRight.Size = new System.Drawing.Size(947, 599);
            this.tLayoutRight.TabIndex = 2;
            // 
            // panelBottom
            // 
            this.panelBottom.BackColor = System.Drawing.SystemColors.Info;
            this.panelBottom.Controls.Add(this.m_lableRowCount);
            this.panelBottom.Controls.Add(this.m_lablePageIndex);
            this.panelBottom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelBottom.Location = new System.Drawing.Point(0, 574);
            this.panelBottom.Margin = new System.Windows.Forms.Padding(0);
            this.panelBottom.Name = "panelBottom";
            this.panelBottom.Size = new System.Drawing.Size(947, 25);
            this.panelBottom.TabIndex = 0;
            // 
            // m_lableRowCount
            // 
            this.m_lableRowCount.Dock = System.Windows.Forms.DockStyle.Right;
            this.m_lableRowCount.Location = new System.Drawing.Point(847, 0);
            this.m_lableRowCount.Name = "m_lableRowCount";
            this.m_lableRowCount.Size = new System.Drawing.Size(100, 25);
            this.m_lableRowCount.TabIndex = 1;
            this.m_lableRowCount.Text = "0 行";
            this.m_lableRowCount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // m_lablePageIndex
            // 
            this.m_lablePageIndex.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.m_lablePageIndex.AutoSize = true;
            this.m_lablePageIndex.Location = new System.Drawing.Point(4, 7);
            this.m_lablePageIndex.Name = "m_lablePageIndex";
            this.m_lablePageIndex.Size = new System.Drawing.Size(77, 12);
            this.m_lablePageIndex.TabIndex = 0;
            this.m_lablePageIndex.Text = "页码： 0 / 0";
            // 
            // CStationDataMgrForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1147, 624);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CStationDataMgrForm";
            this.Text = "数据查询";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CStationDataMgrForm_FormClosing);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.tableLayoutPanel.ResumeLayout(false);
            this.panelLeft.ResumeLayout(false);
            this.panelLeft.PerformLayout();
            this.panelRight.ResumeLayout(false);
            this.tLayoutRight.ResumeLayout(false);
            this.panelBottom.ResumeLayout(false);
            this.panelBottom.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.Panel panelLeft;
        private System.Windows.Forms.Label labelInfoSelect;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnQuery;
        //public System.Windows.Forms.ComboBox cmbStation;
        public Hydrology.CControls.CStationComboBox_1 cmbStation;
        private System.Windows.Forms.DateTimePicker dptTimeEnd;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.DateTimePicker dtpTimeStart;
        public System.Windows.Forms.ComboBox cmbQueryInfo;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel panelRight;
        private System.Windows.Forms.ToolStripStatusLabel m_statusLable;
        private System.Windows.Forms.Button btnNewRecord;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cmb_ViewStyle;
        private System.Windows.Forms.TableLayoutPanel tLayoutRight;
        private System.Windows.Forms.Panel panelBottom;
        private System.Windows.Forms.Label m_lableRowCount;
        private System.Windows.Forms.Label m_lablePageIndex;
        private System.Windows.Forms.ComboBox cmb_RainShape;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cmb_SubCenter;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox cmb_TimeSelect;
        private System.Windows.Forms.Label label9;
    }
}
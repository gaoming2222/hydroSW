namespace Hydrology.Forms
{
    partial class DebugInfoSearch
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
            this.m_lablePageIndex = new System.Windows.Forms.Label();
            this.panelBottom = new System.Windows.Forms.Panel();
            this.m_lableRowCount = new System.Windows.Forms.Label();
            this.tLayoutRight = new System.Windows.Forms.TableLayoutPanel();
            this.panelRight = new System.Windows.Forms.Panel();
            this.btnExit = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btnQuery = new System.Windows.Forms.Button();
            this.dptTimeEnd = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.dtpTimeStart = new System.Windows.Forms.DateTimePicker();
            this.cmbQueryInfo = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.m_statusLable = new System.Windows.Forms.ToolStripStatusLabel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.panelLeft = new System.Windows.Forms.Panel();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.panel5 = new System.Windows.Forms.Panel();
            this.ts_subcenter = new System.Windows.Forms.ComboBox();
            this.label14 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.btn_Exit = new System.Windows.Forms.Button();
            this.label10 = new System.Windows.Forms.Label();
            this.btn_Query = new System.Windows.Forms.Button();
            this.dateTimeEnd = new System.Windows.Forms.DateTimePicker();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.dateTimeStart = new System.Windows.Forms.DateTimePicker();
            this.ts_query = new System.Windows.Forms.ComboBox();
            this.label13 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.cmb_SubCenter = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.cmb_RainShape = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.cmb_ViewStyle = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.btnApply = new System.Windows.Forms.Button();
            this.cmb_TimeSelect = new System.Windows.Forms.ComboBox();
            this.label15 = new System.Windows.Forms.Label();
            this.panelBottom.SuspendLayout();
            this.tLayoutRight.SuspendLayout();
            this.panelRight.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tableLayoutPanel.SuspendLayout();
            this.panelLeft.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.panel4.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.panel5.SuspendLayout();
            this.panel3.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
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
            // panelBottom
            // 
            this.panelBottom.BackColor = System.Drawing.SystemColors.Info;
            this.panelBottom.Controls.Add(this.m_lableRowCount);
            this.panelBottom.Controls.Add(this.m_lablePageIndex);
            this.panelBottom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelBottom.Location = new System.Drawing.Point(0, 499);
            this.panelBottom.Margin = new System.Windows.Forms.Padding(0);
            this.panelBottom.Name = "panelBottom";
            this.panelBottom.Size = new System.Drawing.Size(730, 25);
            this.panelBottom.TabIndex = 0;
            // 
            // m_lableRowCount
            // 
            this.m_lableRowCount.Dock = System.Windows.Forms.DockStyle.Right;
            this.m_lableRowCount.Location = new System.Drawing.Point(630, 0);
            this.m_lableRowCount.Name = "m_lableRowCount";
            this.m_lableRowCount.Size = new System.Drawing.Size(100, 25);
            this.m_lableRowCount.TabIndex = 1;
            this.m_lableRowCount.Text = "0 行";
            this.m_lableRowCount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
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
            this.tLayoutRight.Size = new System.Drawing.Size(730, 524);
            this.tLayoutRight.TabIndex = 2;
            // 
            // panelRight
            // 
            this.panelRight.Controls.Add(this.tLayoutRight);
            this.panelRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelRight.Location = new System.Drawing.Point(200, 0);
            this.panelRight.Margin = new System.Windows.Forms.Padding(0);
            this.panelRight.Name = "panelRight";
            this.panelRight.Size = new System.Drawing.Size(730, 524);
            this.panelRight.TabIndex = 11;
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(0, 0);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 23);
            this.btnExit.TabIndex = 27;
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
            this.btnQuery.Location = new System.Drawing.Point(0, 0);
            this.btnQuery.Name = "btnQuery";
            this.btnQuery.Size = new System.Drawing.Size(75, 23);
            this.btnQuery.TabIndex = 28;
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
            this.tableLayoutPanel1.Size = new System.Drawing.Size(930, 549);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // statusStrip1
            // 
            this.statusStrip1.BackgroundImage = global::Hydrology.Properties.Resources.状态栏;
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_statusLable});
            this.statusStrip1.Location = new System.Drawing.Point(0, 527);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(930, 22);
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
            this.panel1.Size = new System.Drawing.Size(930, 524);
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
            this.tableLayoutPanel.Size = new System.Drawing.Size(930, 524);
            this.tableLayoutPanel.TabIndex = 2;
            // 
            // panelLeft
            // 
            this.panelLeft.BackColor = System.Drawing.SystemColors.HighlightText;
            this.panelLeft.Controls.Add(this.tableLayoutPanel3);
            this.panelLeft.Controls.Add(this.cmb_SubCenter);
            this.panelLeft.Controls.Add(this.label7);
            this.panelLeft.Controls.Add(this.cmb_RainShape);
            this.panelLeft.Controls.Add(this.label6);
            this.panelLeft.Controls.Add(this.cmb_ViewStyle);
            this.panelLeft.Controls.Add(this.label5);
            this.panelLeft.Controls.Add(this.btnApply);
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
            this.panelLeft.Size = new System.Drawing.Size(200, 524);
            this.panelLeft.TabIndex = 10;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.BackColor = System.Drawing.Color.Transparent;
            this.tableLayoutPanel3.BackgroundImage = global::Hydrology.Properties.Resources.WhiteSpace;
            this.tableLayoutPanel3.ColumnCount = 1;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Controls.Add(this.panel4, 0, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 2;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(200, 524);
            this.tableLayoutPanel3.TabIndex = 20;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.tableLayoutPanel4);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.Location = new System.Drawing.Point(0, 0);
            this.panel4.Margin = new System.Windows.Forms.Padding(0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(200, 499);
            this.panel4.TabIndex = 1;
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.tableLayoutPanel4.BackgroundImage = global::Hydrology.Properties.Resources.WhiteSpace;
            this.tableLayoutPanel4.ColumnCount = 2;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 200F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.Controls.Add(this.panel5, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.panel3, 1, 0);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 1;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 499F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 499F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 499F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 499F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 499F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(200, 499);
            this.tableLayoutPanel4.TabIndex = 2;
            // 
            // panel5
            // 
            this.panel5.BackColor = System.Drawing.SystemColors.HighlightText;
            this.panel5.Controls.Add(this.cmb_TimeSelect);
            this.panel5.Controls.Add(this.label15);
            this.panel5.Controls.Add(this.ts_subcenter);
            this.panel5.Controls.Add(this.label14);
            this.panel5.Controls.Add(this.label17);
            this.panel5.Controls.Add(this.btn_Exit);
            this.panel5.Controls.Add(this.label10);
            this.panel5.Controls.Add(this.btn_Query);
            this.panel5.Controls.Add(this.dateTimeEnd);
            this.panel5.Controls.Add(this.label11);
            this.panel5.Controls.Add(this.label12);
            this.panel5.Controls.Add(this.dateTimeStart);
            this.panel5.Controls.Add(this.ts_query);
            this.panel5.Controls.Add(this.label13);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel5.Location = new System.Drawing.Point(0, 0);
            this.panel5.Margin = new System.Windows.Forms.Padding(0);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(200, 499);
            this.panel5.TabIndex = 10;
            // 
            // ts_subcenter
            // 
            this.ts_subcenter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ts_subcenter.FormattingEnabled = true;
            this.ts_subcenter.Location = new System.Drawing.Point(80, 61);
            this.ts_subcenter.Name = "ts_subcenter";
            this.ts_subcenter.Size = new System.Drawing.Size(122, 20);
            this.ts_subcenter.TabIndex = 19;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.BackColor = System.Drawing.Color.Transparent;
            this.label14.Location = new System.Drawing.Point(0, 64);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(53, 12);
            this.label14.TabIndex = 18;
            this.label14.Text = "分中心：";
            // 
            // label17
            // 
            this.label17.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.label17.Dock = System.Windows.Forms.DockStyle.Top;
            this.label17.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label17.Location = new System.Drawing.Point(0, 0);
            this.label17.Name = "label17";
            this.label17.Padding = new System.Windows.Forms.Padding(5);
            this.label17.Size = new System.Drawing.Size(200, 25);
            this.label17.TabIndex = 10;
            this.label17.Text = "配置查询信息";
            // 
            // btn_Exit
            // 
            this.btn_Exit.Location = new System.Drawing.Point(117, 305);
            this.btn_Exit.Name = "btn_Exit";
            this.btn_Exit.Size = new System.Drawing.Size(75, 23);
            this.btn_Exit.TabIndex = 9;
            this.btn_Exit.Text = "退出";
            this.btn_Exit.UseVisualStyleBackColor = true;
            this.btn_Exit.Click += new System.EventHandler(this.btn_Exit_Click);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(3, 104);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(41, 12);
            this.label10.TabIndex = 0;
            this.label10.Text = "测站：";
            // 
            // btn_Query
            // 
            this.btn_Query.Location = new System.Drawing.Point(12, 305);
            this.btn_Query.Name = "btn_Query";
            this.btn_Query.Size = new System.Drawing.Size(75, 23);
            this.btn_Query.TabIndex = 8;
            this.btn_Query.Text = "查询";
            this.btn_Query.UseVisualStyleBackColor = true;
            this.btn_Query.Click += new System.EventHandler(this.btn_Query_Click);
            // 
            // dateTimeEnd
            // 
            this.dateTimeEnd.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.dateTimeEnd.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimeEnd.Location = new System.Drawing.Point(81, 230);
            this.dateTimeEnd.Name = "dateTimeEnd";
            this.dateTimeEnd.ShowUpDown = true;
            this.dateTimeEnd.Size = new System.Drawing.Size(117, 21);
            this.dateTimeEnd.TabIndex = 7;
            this.dateTimeEnd.Value = new System.DateTime(2011, 1, 1, 23, 59, 0, 0);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(0, 143);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(65, 12);
            this.label11.TabIndex = 2;
            this.label11.Text = "查询信息：";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(3, 239);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(65, 12);
            this.label12.TabIndex = 5;
            this.label12.Text = "结束时间：";
            // 
            // dateTimeStart
            // 
            this.dateTimeStart.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.dateTimeStart.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimeStart.Location = new System.Drawing.Point(80, 187);
            this.dateTimeStart.Name = "dateTimeStart";
            this.dateTimeStart.ShowUpDown = true;
            this.dateTimeStart.Size = new System.Drawing.Size(117, 21);
            this.dateTimeStart.TabIndex = 6;
            this.dateTimeStart.Value = new System.DateTime(2010, 1, 1, 0, 0, 0, 0);
            // 
            // ts_query
            // 
            this.ts_query.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ts_query.FormattingEnabled = true;
            this.ts_query.Items.AddRange(new object[] {
            "雨量",
            "水位",
            "电压"});
            this.ts_query.Location = new System.Drawing.Point(80, 139);
            this.ts_query.Name = "ts_query";
            this.ts_query.Size = new System.Drawing.Size(117, 20);
            this.ts_query.TabIndex = 3;
            this.ts_query.SelectedIndexChanged += new System.EventHandler(this.ts_query_SelectedIndexChanged);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(0, 192);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(65, 12);
            this.label13.TabIndex = 4;
            this.label13.Text = "起始时间：";
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.tableLayoutPanel2);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(200, 0);
            this.panel3.Margin = new System.Windows.Forms.Padding(0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1, 499);
            this.panel3.TabIndex = 11;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.panel2, 0, 1);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 3;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.Size = new System.Drawing.Size(1, 499);
            this.tableLayoutPanel2.TabIndex = 2;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.SystemColors.Info;
            this.panel2.Controls.Add(this.label9);
            this.panel2.Controls.Add(this.label8);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 474);
            this.panel2.Margin = new System.Windows.Forms.Padding(0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1, 25);
            this.panel2.TabIndex = 0;
            // 
            // label9
            // 
            this.label9.Dock = System.Windows.Forms.DockStyle.Right;
            this.label9.Location = new System.Drawing.Point(-99, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(100, 25);
            this.label9.TabIndex = 1;
            this.label9.Text = "0 行";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label8
            // 
            this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(4, 7);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(77, 12);
            this.label8.TabIndex = 0;
            this.label8.Text = "页码： 0 / 0";
            // 
            // cmb_SubCenter
            // 
            this.cmb_SubCenter.Location = new System.Drawing.Point(0, 0);
            this.cmb_SubCenter.Name = "cmb_SubCenter";
            this.cmb_SubCenter.Size = new System.Drawing.Size(121, 20);
            this.cmb_SubCenter.TabIndex = 21;
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
            this.cmb_RainShape.Location = new System.Drawing.Point(0, 0);
            this.cmb_RainShape.Name = "cmb_RainShape";
            this.cmb_RainShape.Size = new System.Drawing.Size(121, 20);
            this.cmb_RainShape.TabIndex = 22;
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(0, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(100, 23);
            this.label6.TabIndex = 23;
            // 
            // cmb_ViewStyle
            // 
            this.cmb_ViewStyle.Location = new System.Drawing.Point(0, 0);
            this.cmb_ViewStyle.Name = "cmb_ViewStyle";
            this.cmb_ViewStyle.Size = new System.Drawing.Size(121, 20);
            this.cmb_ViewStyle.TabIndex = 24;
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(0, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(100, 23);
            this.label5.TabIndex = 25;
            // 
            // btnApply
            // 
            this.btnApply.Location = new System.Drawing.Point(0, 0);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(75, 23);
            this.btnApply.TabIndex = 26;
            // 
            // cmb_TimeSelect
            // 
            this.cmb_TimeSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_TimeSelect.FormattingEnabled = true;
            this.cmb_TimeSelect.Location = new System.Drawing.Point(80, 267);
            this.cmb_TimeSelect.Name = "cmb_TimeSelect";
            this.cmb_TimeSelect.Size = new System.Drawing.Size(117, 20);
            this.cmb_TimeSelect.TabIndex = 25;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(3, 271);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(65, 12);
            this.label15.TabIndex = 24;
            this.label15.Text = "整点选择：";
            // 
            // DebugInfoSearch
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(930, 549);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "DebugInfoSearch";
            this.Text = "调试信息查询";
            this.panelBottom.ResumeLayout(false);
            this.panelBottom.PerformLayout();
            this.tLayoutRight.ResumeLayout(false);
            this.panelRight.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.tableLayoutPanel.ResumeLayout(false);
            this.panelLeft.ResumeLayout(false);
            this.panelLeft.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.tableLayoutPanel4.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label m_lablePageIndex;
        private System.Windows.Forms.Panel panelBottom;
        private System.Windows.Forms.Label m_lableRowCount;
        private System.Windows.Forms.TableLayoutPanel tLayoutRight;
        private System.Windows.Forms.Panel panelRight;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnQuery;
        private System.Windows.Forms.DateTimePicker dptTimeEnd;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.DateTimePicker dtpTimeStart;
        public System.Windows.Forms.ComboBox cmbQueryInfo;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel m_statusLable;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.Panel panelLeft;
        private System.Windows.Forms.ComboBox cmb_SubCenter;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox cmb_RainShape;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cmb_ViewStyle;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.ComboBox ts_subcenter;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Button btn_Exit;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Button btn_Query;
        private System.Windows.Forms.DateTimePicker dateTimeEnd;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.DateTimePicker dateTimeStart;
        public System.Windows.Forms.ComboBox ts_query;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        public Hydrology.CControls.CStationComboBox_1 cmbStation;
        private System.Windows.Forms.ComboBox cmb_TimeSelect;
        private System.Windows.Forms.Label label15;
    }
}
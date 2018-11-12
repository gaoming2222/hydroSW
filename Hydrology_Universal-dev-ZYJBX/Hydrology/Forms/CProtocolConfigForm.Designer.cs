namespace Hydrology.Forms
{
    partial class CProtocolConfigForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CProtocolConfigForm));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tsButSave = new System.Windows.Forms.ToolStripButton();
            this.tsButRevert = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsButExit = new System.Windows.Forms.ToolStripButton();
            this.btnAddNewProtocol = new System.Windows.Forms.ToolStripButton();
            this.btnDelete = new System.Windows.Forms.ToolStripButton();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel4ChannelAdd = new System.Windows.Forms.Panel();
            this.lbl_ToolTip1 = new System.Windows.Forms.Label();
            this.cmb_ChannelInterfaceNames = new System.Windows.Forms.ComboBox();
            this.label15 = new System.Windows.Forms.Label();
            this.txt_ChannelDllPath = new System.Windows.Forms.TextBox();
            this.txt_ChannelDllFileName = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.btn_ChannelBrowse = new System.Windows.Forms.Button();
            this.label14 = new System.Windows.Forms.Label();
            this.txt_ChannelClassName = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.txt_ChannelProtocolName = new System.Windows.Forms.TextBox();
            this.lbl_ChannelClassName = new System.Windows.Forms.Label();
            this.btnChannelSaveAddNew = new System.Windows.Forms.Button();
            this.btnChannelCancelAddNew = new System.Windows.Forms.Button();
            this.panel4DataAdd = new System.Windows.Forms.Panel();
            this.lbl_ToolTip2 = new System.Windows.Forms.Label();
            this.txt_DataDllPath = new System.Windows.Forms.TextBox();
            this.txt_DataDllFileName = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.btn_DataBrowse = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.txt_DataUp = new System.Windows.Forms.TextBox();
            this.lbl_DataDown = new System.Windows.Forms.Label();
            this.txt_DataDown = new System.Windows.Forms.TextBox();
            this.lbl_DataUp = new System.Windows.Forms.Label();
            this.txt_DataFlash = new System.Windows.Forms.TextBox();
            this.txt_DataUDisk = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txt_DataProtocolName = new System.Windows.Forms.TextBox();
            this.lbl_DataFlash = new System.Windows.Forms.Label();
            this.lbl_DataUDisk = new System.Windows.Forms.Label();
            this.btnDataSaveAddNew = new System.Windows.Forms.Button();
            this.btnDataCancelAddNew = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btn_Cancel = new System.Windows.Forms.Button();
            this.btn_Save = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.textBox_DllPath = new System.Windows.Forms.TextBox();
            this.textBox_DllFileName = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btn_Browse = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtProtocolName = new System.Windows.Forms.TextBox();
            this.cmb_Interfaces = new System.Windows.Forms.ComboBox();
            this.cmb_ClassNames = new System.Windows.Forms.ComboBox();
            this.lbl_Interfaces = new System.Windows.Forms.Label();
            this.lbl_ClassNames = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.listBox_ChannelList = new System.Windows.Forms.ListBox();
            this.labelInfoSelect = new System.Windows.Forms.Label();
            this.m_openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.toolStrip1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel4ChannelAdd.SuspendLayout();
            this.panel4DataAdd.SuspendLayout();
            this.panel2.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.AutoSize = false;
            this.toolStrip1.BackgroundImage = global::Hydrology.Properties.Resources.菜单栏;
            this.toolStrip1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(35, 35);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsButSave,
            this.tsButRevert,
            this.toolStripSeparator1,
            this.tsButExit,
            this.btnAddNewProtocol,
            this.btnDelete});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip1.Size = new System.Drawing.Size(535, 45);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // tsButSave
            // 
            this.tsButSave.AutoSize = false;
            this.tsButSave.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.tsButSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsButSave.Image = ((System.Drawing.Image)(resources.GetObject("tsButSave.Image")));
            this.tsButSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsButSave.Name = "tsButSave";
            this.tsButSave.Size = new System.Drawing.Size(50, 45);
            this.tsButSave.Text = "保存";
            this.tsButSave.Click += new System.EventHandler(this.tsButSave_Click);
            // 
            // tsButRevert
            // 
            this.tsButRevert.AutoSize = false;
            this.tsButRevert.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsButRevert.Image = global::Hydrology.Properties.Resources.undo;
            this.tsButRevert.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsButRevert.Name = "tsButRevert";
            this.tsButRevert.Size = new System.Drawing.Size(50, 45);
            this.tsButRevert.Text = "撤销";
            this.tsButRevert.Click += new System.EventHandler(this.tsButRevert_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 45);
            // 
            // tsButExit
            // 
            this.tsButExit.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.tsButExit.AutoSize = false;
            this.tsButExit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsButExit.Image = global::Hydrology.Properties.Resources.exit;
            this.tsButExit.ImageTransparentColor = System.Drawing.Color.Indigo;
            this.tsButExit.Name = "tsButExit";
            this.tsButExit.Size = new System.Drawing.Size(50, 45);
            this.tsButExit.Text = "退出";
            this.tsButExit.Click += new System.EventHandler(this.tsButExit_Click);
            // 
            // btnAddNewProtocol
            // 
            this.btnAddNewProtocol.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnAddNewProtocol.Image = global::Hydrology.Properties.Resources.add;
            this.btnAddNewProtocol.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnAddNewProtocol.Name = "btnAddNewProtocol";
            this.btnAddNewProtocol.Size = new System.Drawing.Size(39, 42);
            this.btnAddNewProtocol.Text = "添加";
            this.btnAddNewProtocol.Click += new System.EventHandler(this.btnAddNewProtocol_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.AutoSize = false;
            this.btnDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnDelete.Image = global::Hydrology.Properties.Resources.delete1;
            this.btnDelete.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnDelete.Margin = new System.Windows.Forms.Padding(0);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(35, 35);
            this.btnDelete.Text = "删除";
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.BackgroundImage = global::Hydrology.Properties.Resources.状态栏;
            this.statusStrip1.Location = new System.Drawing.Point(0, 356);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(535, 22);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 150F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.panel3, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 45);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(535, 311);
            this.tableLayoutPanel1.TabIndex = 3;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.panel4ChannelAdd);
            this.panel3.Controls.Add(this.panel4DataAdd);
            this.panel3.Controls.Add(this.panel2);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(153, 3);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(379, 305);
            this.panel3.TabIndex = 6;
            // 
            // panel4ChannelAdd
            // 
            this.panel4ChannelAdd.Controls.Add(this.lbl_ToolTip1);
            this.panel4ChannelAdd.Controls.Add(this.cmb_ChannelInterfaceNames);
            this.panel4ChannelAdd.Controls.Add(this.label15);
            this.panel4ChannelAdd.Controls.Add(this.txt_ChannelDllPath);
            this.panel4ChannelAdd.Controls.Add(this.txt_ChannelDllFileName);
            this.panel4ChannelAdd.Controls.Add(this.label12);
            this.panel4ChannelAdd.Controls.Add(this.btn_ChannelBrowse);
            this.panel4ChannelAdd.Controls.Add(this.label14);
            this.panel4ChannelAdd.Controls.Add(this.txt_ChannelClassName);
            this.panel4ChannelAdd.Controls.Add(this.label17);
            this.panel4ChannelAdd.Controls.Add(this.txt_ChannelProtocolName);
            this.panel4ChannelAdd.Controls.Add(this.lbl_ChannelClassName);
            this.panel4ChannelAdd.Controls.Add(this.btnChannelSaveAddNew);
            this.panel4ChannelAdd.Controls.Add(this.btnChannelCancelAddNew);
            this.panel4ChannelAdd.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4ChannelAdd.Location = new System.Drawing.Point(0, 0);
            this.panel4ChannelAdd.Name = "panel4ChannelAdd";
            this.panel4ChannelAdd.Size = new System.Drawing.Size(379, 305);
            this.panel4ChannelAdd.TabIndex = 8;
            // 
            // lbl_ToolTip1
            // 
            this.lbl_ToolTip1.AutoSize = true;
            this.lbl_ToolTip1.ForeColor = System.Drawing.Color.Red;
            this.lbl_ToolTip1.Location = new System.Drawing.Point(129, 134);
            this.lbl_ToolTip1.Name = "lbl_ToolTip1";
            this.lbl_ToolTip1.Size = new System.Drawing.Size(155, 12);
            this.lbl_ToolTip1.TabIndex = 27;
            this.lbl_ToolTip1.Text = "注意:类名使用完全限定名！";
            // 
            // cmb_ChannelInterfaceNames
            // 
            this.cmb_ChannelInterfaceNames.FormattingEnabled = true;
            this.cmb_ChannelInterfaceNames.Location = new System.Drawing.Point(131, 59);
            this.cmb_ChannelInterfaceNames.Name = "cmb_ChannelInterfaceNames";
            this.cmb_ChannelInterfaceNames.Size = new System.Drawing.Size(223, 20);
            this.cmb_ChannelInterfaceNames.TabIndex = 26;
            this.cmb_ChannelInterfaceNames.SelectedIndexChanged += new System.EventHandler(this.cmb_ChannelInterfaceNames_SelectedIndexChanged);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(21, 60);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(65, 12);
            this.label15.TabIndex = 25;
            this.label15.Text = "实现接口：";
            // 
            // txt_ChannelDllPath
            // 
            this.txt_ChannelDllPath.Location = new System.Drawing.Point(89, 217);
            this.txt_ChannelDllPath.Name = "txt_ChannelDllPath";
            this.txt_ChannelDllPath.ReadOnly = true;
            this.txt_ChannelDllPath.Size = new System.Drawing.Size(226, 21);
            this.txt_ChannelDllPath.TabIndex = 24;
            // 
            // txt_ChannelDllFileName
            // 
            this.txt_ChannelDllFileName.Location = new System.Drawing.Point(89, 187);
            this.txt_ChannelDllFileName.Name = "txt_ChannelDllFileName";
            this.txt_ChannelDllFileName.ReadOnly = true;
            this.txt_ChannelDllFileName.Size = new System.Drawing.Size(265, 21);
            this.txt_ChannelDllFileName.TabIndex = 23;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(21, 190);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(53, 12);
            this.label12.TabIndex = 22;
            this.label12.Text = "文件名：";
            // 
            // btn_ChannelBrowse
            // 
            this.btn_ChannelBrowse.Location = new System.Drawing.Point(321, 216);
            this.btn_ChannelBrowse.Name = "btn_ChannelBrowse";
            this.btn_ChannelBrowse.Size = new System.Drawing.Size(34, 23);
            this.btn_ChannelBrowse.TabIndex = 21;
            this.btn_ChannelBrowse.Text = "...";
            this.btn_ChannelBrowse.UseVisualStyleBackColor = true;
            this.btn_ChannelBrowse.Click += new System.EventHandler(this.btn_ChannelBrowse_Click);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(21, 220);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(41, 12);
            this.label14.TabIndex = 20;
            this.label14.Text = "路径：";
            // 
            // txt_ChannelClassName
            // 
            this.txt_ChannelClassName.Location = new System.Drawing.Point(130, 94);
            this.txt_ChannelClassName.Name = "txt_ChannelClassName";
            this.txt_ChannelClassName.ReadOnly = true;
            this.txt_ChannelClassName.Size = new System.Drawing.Size(224, 21);
            this.txt_ChannelClassName.TabIndex = 13;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(21, 21);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(53, 12);
            this.label17.TabIndex = 12;
            this.label17.Text = "协议名：";
            // 
            // txt_ChannelProtocolName
            // 
            this.txt_ChannelProtocolName.Enabled = false;
            this.txt_ChannelProtocolName.Location = new System.Drawing.Point(130, 14);
            this.txt_ChannelProtocolName.Name = "txt_ChannelProtocolName";
            this.txt_ChannelProtocolName.ReadOnly = true;
            this.txt_ChannelProtocolName.Size = new System.Drawing.Size(224, 21);
            this.txt_ChannelProtocolName.TabIndex = 11;
            // 
            // lbl_ChannelClassName
            // 
            this.lbl_ChannelClassName.AutoSize = true;
            this.lbl_ChannelClassName.Location = new System.Drawing.Point(21, 98);
            this.lbl_ChannelClassName.Name = "lbl_ChannelClassName";
            this.lbl_ChannelClassName.Size = new System.Drawing.Size(41, 12);
            this.lbl_ChannelClassName.TabIndex = 9;
            this.lbl_ChannelClassName.Text = "类名：";
            // 
            // btnChannelSaveAddNew
            // 
            this.btnChannelSaveAddNew.Location = new System.Drawing.Point(268, 255);
            this.btnChannelSaveAddNew.Name = "btnChannelSaveAddNew";
            this.btnChannelSaveAddNew.Size = new System.Drawing.Size(108, 37);
            this.btnChannelSaveAddNew.TabIndex = 7;
            this.btnChannelSaveAddNew.Text = "完成添加";
            this.btnChannelSaveAddNew.UseVisualStyleBackColor = true;
            this.btnChannelSaveAddNew.Click += new System.EventHandler(this.btnChannelSaveAddNew_Click);
            // 
            // btnChannelCancelAddNew
            // 
            this.btnChannelCancelAddNew.Location = new System.Drawing.Point(154, 255);
            this.btnChannelCancelAddNew.Name = "btnChannelCancelAddNew";
            this.btnChannelCancelAddNew.Size = new System.Drawing.Size(108, 37);
            this.btnChannelCancelAddNew.TabIndex = 6;
            this.btnChannelCancelAddNew.Text = "取消添加";
            this.btnChannelCancelAddNew.UseVisualStyleBackColor = true;
            this.btnChannelCancelAddNew.Click += new System.EventHandler(this.btnChannelCancelAddNew_Click);
            // 
            // panel4DataAdd
            // 
            this.panel4DataAdd.Controls.Add(this.lbl_ToolTip2);
            this.panel4DataAdd.Controls.Add(this.txt_DataDllPath);
            this.panel4DataAdd.Controls.Add(this.txt_DataDllFileName);
            this.panel4DataAdd.Controls.Add(this.label6);
            this.panel4DataAdd.Controls.Add(this.btn_DataBrowse);
            this.panel4DataAdd.Controls.Add(this.label7);
            this.panel4DataAdd.Controls.Add(this.txt_DataUp);
            this.panel4DataAdd.Controls.Add(this.lbl_DataDown);
            this.panel4DataAdd.Controls.Add(this.txt_DataDown);
            this.panel4DataAdd.Controls.Add(this.lbl_DataUp);
            this.panel4DataAdd.Controls.Add(this.txt_DataFlash);
            this.panel4DataAdd.Controls.Add(this.txt_DataUDisk);
            this.panel4DataAdd.Controls.Add(this.label8);
            this.panel4DataAdd.Controls.Add(this.txt_DataProtocolName);
            this.panel4DataAdd.Controls.Add(this.lbl_DataFlash);
            this.panel4DataAdd.Controls.Add(this.lbl_DataUDisk);
            this.panel4DataAdd.Controls.Add(this.btnDataSaveAddNew);
            this.panel4DataAdd.Controls.Add(this.btnDataCancelAddNew);
            this.panel4DataAdd.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4DataAdd.Location = new System.Drawing.Point(0, 0);
            this.panel4DataAdd.Name = "panel4DataAdd";
            this.panel4DataAdd.Size = new System.Drawing.Size(379, 305);
            this.panel4DataAdd.TabIndex = 6;
            this.panel4DataAdd.Visible = false;
            // 
            // lbl_ToolTip2
            // 
            this.lbl_ToolTip2.AutoSize = true;
            this.lbl_ToolTip2.ForeColor = System.Drawing.Color.Red;
            this.lbl_ToolTip2.Location = new System.Drawing.Point(168, 161);
            this.lbl_ToolTip2.Name = "lbl_ToolTip2";
            this.lbl_ToolTip2.Size = new System.Drawing.Size(155, 12);
            this.lbl_ToolTip2.TabIndex = 25;
            this.lbl_ToolTip2.Text = "注意:类名使用完全限定名！";
            // 
            // txt_DataDllPath
            // 
            this.txt_DataDllPath.Location = new System.Drawing.Point(89, 217);
            this.txt_DataDllPath.Name = "txt_DataDllPath";
            this.txt_DataDllPath.ReadOnly = true;
            this.txt_DataDllPath.Size = new System.Drawing.Size(226, 21);
            this.txt_DataDllPath.TabIndex = 24;
            // 
            // txt_DataDllFileName
            // 
            this.txt_DataDllFileName.Location = new System.Drawing.Point(89, 187);
            this.txt_DataDllFileName.Name = "txt_DataDllFileName";
            this.txt_DataDllFileName.ReadOnly = true;
            this.txt_DataDllFileName.Size = new System.Drawing.Size(265, 21);
            this.txt_DataDllFileName.TabIndex = 23;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(21, 190);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(53, 12);
            this.label6.TabIndex = 22;
            this.label6.Text = "文件名：";
            // 
            // btn_DataBrowse
            // 
            this.btn_DataBrowse.Location = new System.Drawing.Point(321, 216);
            this.btn_DataBrowse.Name = "btn_DataBrowse";
            this.btn_DataBrowse.Size = new System.Drawing.Size(34, 23);
            this.btn_DataBrowse.TabIndex = 21;
            this.btn_DataBrowse.Text = "...";
            this.btn_DataBrowse.UseVisualStyleBackColor = true;
            this.btn_DataBrowse.Click += new System.EventHandler(this.btn_DataBrowse_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(21, 220);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(41, 12);
            this.label7.TabIndex = 20;
            this.label7.Text = "路径：";
            // 
            // txt_DataUp
            // 
            this.txt_DataUp.Location = new System.Drawing.Point(170, 130);
            this.txt_DataUp.Name = "txt_DataUp";
            this.txt_DataUp.ReadOnly = true;
            this.txt_DataUp.Size = new System.Drawing.Size(184, 21);
            this.txt_DataUp.TabIndex = 19;
            // 
            // lbl_DataDown
            // 
            this.lbl_DataDown.AutoSize = true;
            this.lbl_DataDown.Location = new System.Drawing.Point(21, 107);
            this.lbl_DataDown.Name = "lbl_DataDown";
            this.lbl_DataDown.Size = new System.Drawing.Size(107, 12);
            this.lbl_DataDown.TabIndex = 18;
            this.lbl_DataDown.Text = "实现IDown接口类：";
            // 
            // txt_DataDown
            // 
            this.txt_DataDown.Location = new System.Drawing.Point(170, 100);
            this.txt_DataDown.Name = "txt_DataDown";
            this.txt_DataDown.ReadOnly = true;
            this.txt_DataDown.Size = new System.Drawing.Size(184, 21);
            this.txt_DataDown.TabIndex = 17;
            // 
            // lbl_DataUp
            // 
            this.lbl_DataUp.AutoSize = true;
            this.lbl_DataUp.Location = new System.Drawing.Point(21, 134);
            this.lbl_DataUp.Name = "lbl_DataUp";
            this.lbl_DataUp.Size = new System.Drawing.Size(95, 12);
            this.lbl_DataUp.TabIndex = 15;
            this.lbl_DataUp.Text = "实现IUp接口类：";
            // 
            // txt_DataFlash
            // 
            this.txt_DataFlash.Location = new System.Drawing.Point(170, 71);
            this.txt_DataFlash.Name = "txt_DataFlash";
            this.txt_DataFlash.ReadOnly = true;
            this.txt_DataFlash.Size = new System.Drawing.Size(184, 21);
            this.txt_DataFlash.TabIndex = 14;
            // 
            // txt_DataUDisk
            // 
            this.txt_DataUDisk.Location = new System.Drawing.Point(170, 44);
            this.txt_DataUDisk.Name = "txt_DataUDisk";
            this.txt_DataUDisk.ReadOnly = true;
            this.txt_DataUDisk.Size = new System.Drawing.Size(184, 21);
            this.txt_DataUDisk.TabIndex = 13;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(21, 34);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(53, 12);
            this.label8.TabIndex = 12;
            this.label8.Text = "协议名：";
            // 
            // txt_DataProtocolName
            // 
            this.txt_DataProtocolName.Location = new System.Drawing.Point(89, 27);
            this.txt_DataProtocolName.Name = "txt_DataProtocolName";
            this.txt_DataProtocolName.Size = new System.Drawing.Size(265, 21);
            this.txt_DataProtocolName.TabIndex = 11;
            // 
            // lbl_DataFlash
            // 
            this.lbl_DataFlash.AutoSize = true;
            this.lbl_DataFlash.Location = new System.Drawing.Point(21, 79);
            this.lbl_DataFlash.Name = "lbl_DataFlash";
            this.lbl_DataFlash.Size = new System.Drawing.Size(143, 12);
            this.lbl_DataFlash.TabIndex = 10;
            this.lbl_DataFlash.Text = "实现IFlashBatch接口类：";
            // 
            // lbl_DataUDisk
            // 
            this.lbl_DataUDisk.AutoSize = true;
            this.lbl_DataUDisk.Location = new System.Drawing.Point(21, 48);
            this.lbl_DataUDisk.Name = "lbl_DataUDisk";
            this.lbl_DataUDisk.Size = new System.Drawing.Size(119, 12);
            this.lbl_DataUDisk.TabIndex = 9;
            this.lbl_DataUDisk.Text = "实现IUBatch接口类：";
            // 
            // btnDataSaveAddNew
            // 
            this.btnDataSaveAddNew.Location = new System.Drawing.Point(268, 255);
            this.btnDataSaveAddNew.Name = "btnDataSaveAddNew";
            this.btnDataSaveAddNew.Size = new System.Drawing.Size(108, 37);
            this.btnDataSaveAddNew.TabIndex = 7;
            this.btnDataSaveAddNew.Text = "完成添加";
            this.btnDataSaveAddNew.UseVisualStyleBackColor = true;
            this.btnDataSaveAddNew.Click += new System.EventHandler(this.btnDataSaveAddNew_Click);
            // 
            // btnDataCancelAddNew
            // 
            this.btnDataCancelAddNew.Location = new System.Drawing.Point(154, 255);
            this.btnDataCancelAddNew.Name = "btnDataCancelAddNew";
            this.btnDataCancelAddNew.Size = new System.Drawing.Size(108, 37);
            this.btnDataCancelAddNew.TabIndex = 6;
            this.btnDataCancelAddNew.Text = "取消添加";
            this.btnDataCancelAddNew.UseVisualStyleBackColor = true;
            this.btnDataCancelAddNew.Click += new System.EventHandler(this.btnDataCancelAddNew_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.btn_Cancel);
            this.panel2.Controls.Add(this.btn_Save);
            this.panel2.Controls.Add(this.groupBox2);
            this.panel2.Controls.Add(this.groupBox1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(379, 305);
            this.panel2.TabIndex = 1;
            // 
            // btn_Cancel
            // 
            this.btn_Cancel.Location = new System.Drawing.Point(142, 255);
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.Size = new System.Drawing.Size(108, 37);
            this.btn_Cancel.TabIndex = 5;
            this.btn_Cancel.Text = "放弃修改";
            this.btn_Cancel.UseVisualStyleBackColor = true;
            this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
            // 
            // btn_Save
            // 
            this.btn_Save.Location = new System.Drawing.Point(255, 255);
            this.btn_Save.Name = "btn_Save";
            this.btn_Save.Size = new System.Drawing.Size(108, 37);
            this.btn_Save.TabIndex = 4;
            this.btn_Save.Text = "保存修改";
            this.btn_Save.UseVisualStyleBackColor = true;
            this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.textBox_DllPath);
            this.groupBox2.Controls.Add(this.textBox_DllFileName);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.btn_Browse);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Location = new System.Drawing.Point(3, 144);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(360, 86);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "配置";
            // 
            // textBox_DllPath
            // 
            this.textBox_DllPath.Location = new System.Drawing.Point(88, 47);
            this.textBox_DllPath.Name = "textBox_DllPath";
            this.textBox_DllPath.ReadOnly = true;
            this.textBox_DllPath.Size = new System.Drawing.Size(226, 21);
            this.textBox_DllPath.TabIndex = 9;
            // 
            // textBox_DllFileName
            // 
            this.textBox_DllFileName.Location = new System.Drawing.Point(89, 17);
            this.textBox_DllFileName.Name = "textBox_DllFileName";
            this.textBox_DllFileName.ReadOnly = true;
            this.textBox_DllFileName.Size = new System.Drawing.Size(264, 21);
            this.textBox_DllFileName.TabIndex = 8;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(20, 20);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 12);
            this.label4.TabIndex = 7;
            this.label4.Text = "文件名：";
            // 
            // btn_Browse
            // 
            this.btn_Browse.Location = new System.Drawing.Point(320, 46);
            this.btn_Browse.Name = "btn_Browse";
            this.btn_Browse.Size = new System.Drawing.Size(34, 23);
            this.btn_Browse.TabIndex = 6;
            this.btn_Browse.Text = "...";
            this.btn_Browse.UseVisualStyleBackColor = true;
            this.btn_Browse.Click += new System.EventHandler(this.btn_Browse_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(20, 50);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 12);
            this.label3.TabIndex = 4;
            this.label3.Text = "路径：";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.txtProtocolName);
            this.groupBox1.Controls.Add(this.cmb_Interfaces);
            this.groupBox1.Controls.Add(this.cmb_ClassNames);
            this.groupBox1.Controls.Add(this.lbl_Interfaces);
            this.groupBox1.Controls.Add(this.lbl_ClassNames);
            this.groupBox1.Location = new System.Drawing.Point(3, 9);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(360, 129);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "基本信息";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(20, 27);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 12);
            this.label5.TabIndex = 5;
            this.label5.Text = "协议名：";
            // 
            // txtProtocolName
            // 
            this.txtProtocolName.Location = new System.Drawing.Point(89, 20);
            this.txtProtocolName.Name = "txtProtocolName";
            this.txtProtocolName.Size = new System.Drawing.Size(264, 21);
            this.txtProtocolName.TabIndex = 4;
            // 
            // cmb_Interfaces
            // 
            this.cmb_Interfaces.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_Interfaces.Enabled = false;
            this.cmb_Interfaces.FormattingEnabled = true;
            this.cmb_Interfaces.Location = new System.Drawing.Point(89, 82);
            this.cmb_Interfaces.Name = "cmb_Interfaces";
            this.cmb_Interfaces.Size = new System.Drawing.Size(265, 20);
            this.cmb_Interfaces.TabIndex = 3;
            // 
            // cmb_ClassNames
            // 
            this.cmb_ClassNames.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_ClassNames.FormattingEnabled = true;
            this.cmb_ClassNames.Location = new System.Drawing.Point(89, 51);
            this.cmb_ClassNames.Name = "cmb_ClassNames";
            this.cmb_ClassNames.Size = new System.Drawing.Size(265, 20);
            this.cmb_ClassNames.TabIndex = 2;
            this.cmb_ClassNames.SelectedIndexChanged += new System.EventHandler(this.cmb_ClassNames_SelectedIndexChanged);
            // 
            // lbl_Interfaces
            // 
            this.lbl_Interfaces.AutoSize = true;
            this.lbl_Interfaces.Location = new System.Drawing.Point(18, 85);
            this.lbl_Interfaces.Name = "lbl_Interfaces";
            this.lbl_Interfaces.Size = new System.Drawing.Size(65, 12);
            this.lbl_Interfaces.TabIndex = 1;
            this.lbl_Interfaces.Text = "实现接口：";
            // 
            // lbl_ClassNames
            // 
            this.lbl_ClassNames.AutoSize = true;
            this.lbl_ClassNames.Location = new System.Drawing.Point(18, 54);
            this.lbl_ClassNames.Name = "lbl_ClassNames";
            this.lbl_ClassNames.Size = new System.Drawing.Size(53, 12);
            this.lbl_ClassNames.TabIndex = 0;
            this.lbl_ClassNames.Text = "类型名：";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.listBox_ChannelList);
            this.panel1.Controls.Add(this.labelInfoSelect);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(144, 305);
            this.panel1.TabIndex = 0;
            // 
            // listBox_ChannelList
            // 
            this.listBox_ChannelList.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.listBox_ChannelList.FormattingEnabled = true;
            this.listBox_ChannelList.ItemHeight = 12;
            this.listBox_ChannelList.Location = new System.Drawing.Point(0, 25);
            this.listBox_ChannelList.Name = "listBox_ChannelList";
            this.listBox_ChannelList.Size = new System.Drawing.Size(144, 280);
            this.listBox_ChannelList.TabIndex = 13;
            this.listBox_ChannelList.SelectedIndexChanged += new System.EventHandler(this.listBox_ChannelList_SelectedIndexChanged);
            // 
            // labelInfoSelect
            // 
            this.labelInfoSelect.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.labelInfoSelect.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelInfoSelect.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelInfoSelect.Location = new System.Drawing.Point(0, 0);
            this.labelInfoSelect.Name = "labelInfoSelect";
            this.labelInfoSelect.Padding = new System.Windows.Forms.Padding(5);
            this.labelInfoSelect.Size = new System.Drawing.Size(144, 25);
            this.labelInfoSelect.TabIndex = 12;
            this.labelInfoSelect.Text = "协议列表：";
            // 
            // m_openFileDialog
            // 
            this.m_openFileDialog.FileName = "openFileDialog1";
            // 
            // CProtocolConfigForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(535, 378);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.toolStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CProtocolConfigForm";
            this.Text = "通讯方式配置";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CChannelConfigForm_FormClosing);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel4ChannelAdd.ResumeLayout(false);
            this.panel4ChannelAdd.PerformLayout();
            this.panel4DataAdd.ResumeLayout(false);
            this.panel4DataAdd.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton tsButSave;
        private System.Windows.Forms.ToolStripButton tsButRevert;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton tsButExit;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label labelInfoSelect;
        private System.Windows.Forms.ListBox listBox_ChannelList;
        private System.Windows.Forms.OpenFileDialog m_openFileDialog;
        private System.Windows.Forms.ToolStripButton btnDelete;
        private System.Windows.Forms.ToolStripButton btnAddNewProtocol;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btn_Cancel;
        private System.Windows.Forms.Button btn_Save;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox textBox_DllPath;
        private System.Windows.Forms.TextBox textBox_DllFileName;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btn_Browse;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtProtocolName;
        private System.Windows.Forms.ComboBox cmb_Interfaces;
        private System.Windows.Forms.ComboBox cmb_ClassNames;
        private System.Windows.Forms.Label lbl_Interfaces;
        private System.Windows.Forms.Label lbl_ClassNames;
        private System.Windows.Forms.Panel panel4DataAdd;
        private System.Windows.Forms.Button btnDataSaveAddNew;
        private System.Windows.Forms.Button btnDataCancelAddNew;
        private System.Windows.Forms.TextBox txt_DataDllPath;
        private System.Windows.Forms.TextBox txt_DataDllFileName;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btn_DataBrowse;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txt_DataUp;
        private System.Windows.Forms.Label lbl_DataDown;
        private System.Windows.Forms.TextBox txt_DataDown;
        private System.Windows.Forms.Label lbl_DataUp;
        private System.Windows.Forms.TextBox txt_DataFlash;
        private System.Windows.Forms.TextBox txt_DataUDisk;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txt_DataProtocolName;
        private System.Windows.Forms.Label lbl_DataFlash;
        private System.Windows.Forms.Label lbl_DataUDisk;
        private System.Windows.Forms.Panel panel4ChannelAdd;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox txt_ChannelDllPath;
        private System.Windows.Forms.TextBox txt_ChannelDllFileName;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Button btn_ChannelBrowse;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox txt_ChannelClassName;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.TextBox txt_ChannelProtocolName;
        private System.Windows.Forms.Label lbl_ChannelClassName;
        private System.Windows.Forms.Button btnChannelSaveAddNew;
        private System.Windows.Forms.Button btnChannelCancelAddNew;
        private System.Windows.Forms.ComboBox cmb_ChannelInterfaceNames;
        private System.Windows.Forms.Label lbl_ToolTip2;
        private System.Windows.Forms.Label lbl_ToolTip1;
    }
}
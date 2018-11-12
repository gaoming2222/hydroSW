namespace Hydrology.Forms
{
    partial class CWaterFlowMapForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CWaterFlowMapForm));
            this.panelLeft = new System.Windows.Forms.Panel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.listBox_StationName = new System.Windows.Forms.ListBox();
            this.labelInfoSelect = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label7 = new System.Windows.Forms.Label();
            this.textBox_Search = new System.Windows.Forms.TextBox();
            this.cmb_SubCenter = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel_Right = new System.Windows.Forms.Panel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tsButSave = new System.Windows.Forms.ToolStripButton();
            this.tsButImport = new System.Windows.Forms.ToolStripButton();
            this.tsButExit = new System.Windows.Forms.ToolStripButton();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.panelLeft.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelLeft
            // 
            this.panelLeft.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelLeft.Controls.Add(this.tableLayoutPanel2);
            this.panelLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelLeft.Location = new System.Drawing.Point(3, 3);
            this.panelLeft.Name = "panelLeft";
            this.panelLeft.Size = new System.Drawing.Size(194, 412);
            this.panelLeft.TabIndex = 0;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.Controls.Add(this.listBox_StationName, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.labelInfoSelect, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.panel2, 0, 1);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 3;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 77F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(192, 410);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // listBox_StationName
            // 
            this.listBox_StationName.Dock = System.Windows.Forms.DockStyle.Left;
            this.listBox_StationName.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.listBox_StationName.FormattingEnabled = true;
            this.listBox_StationName.IntegralHeight = false;
            this.listBox_StationName.ItemHeight = 12;
            this.listBox_StationName.Location = new System.Drawing.Point(0, 102);
            this.listBox_StationName.Margin = new System.Windows.Forms.Padding(0);
            this.listBox_StationName.Name = "listBox_StationName";
            this.listBox_StationName.Size = new System.Drawing.Size(189, 308);
            this.listBox_StationName.TabIndex = 18;
            this.listBox_StationName.SelectedIndexChanged += new System.EventHandler(this.listBox_StationName_SelectedIndexChanged);
            // 
            // labelInfoSelect
            // 
            this.labelInfoSelect.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.labelInfoSelect.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelInfoSelect.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelInfoSelect.Location = new System.Drawing.Point(0, 0);
            this.labelInfoSelect.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.labelInfoSelect.Name = "labelInfoSelect";
            this.labelInfoSelect.Padding = new System.Windows.Forms.Padding(5);
            this.labelInfoSelect.Size = new System.Drawing.Size(195, 25);
            this.labelInfoSelect.TabIndex = 12;
            this.labelInfoSelect.Text = "选择测站";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.label7);
            this.panel2.Controls.Add(this.textBox_Search);
            this.panel2.Controls.Add(this.cmb_SubCenter);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(3, 28);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(192, 71);
            this.panel2.TabIndex = 13;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.BackColor = System.Drawing.Color.Transparent;
            this.label7.Location = new System.Drawing.Point(3, 11);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(53, 12);
            this.label7.TabIndex = 20;
            this.label7.Text = "分中心：";
            // 
            // textBox_Search
            // 
            this.textBox_Search.Location = new System.Drawing.Point(47, 36);
            this.textBox_Search.Name = "textBox_Search";
            this.textBox_Search.Size = new System.Drawing.Size(139, 21);
            this.textBox_Search.TabIndex = 15;
            this.textBox_Search.TextChanged += new System.EventHandler(this.textBox_Search_TextChanged);
            // 
            // cmb_SubCenter
            // 
            this.cmb_SubCenter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_SubCenter.FormattingEnabled = true;
            this.cmb_SubCenter.Location = new System.Drawing.Point(62, 8);
            this.cmb_SubCenter.Name = "cmb_SubCenter";
            this.cmb_SubCenter.Size = new System.Drawing.Size(121, 20);
            this.cmb_SubCenter.TabIndex = 21;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(5, 45);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 14;
            this.label1.Text = "查找：";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.tableLayoutPanel1);
            this.panel1.Controls.Add(this.toolStrip1);
            this.panel1.Controls.Add(this.statusStrip1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1030, 485);
            this.panel1.TabIndex = 2;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 200F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.panelLeft, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel_Right, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 45);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1030, 418);
            this.tableLayoutPanel1.TabIndex = 2;
            // 
            // panel_Right
            // 
            this.panel_Right.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_Right.Location = new System.Drawing.Point(203, 3);
            this.panel_Right.Name = "panel_Right";
            this.panel_Right.Size = new System.Drawing.Size(824, 412);
            this.panel_Right.TabIndex = 1;
            // 
            // toolStrip1
            // 
            this.toolStrip1.AutoSize = false;
            this.toolStrip1.BackgroundImage = global::Hydrology.Properties.Resources.菜单栏;
            this.toolStrip1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(35, 35);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsButSave,
            this.tsButImport,
            this.tsButExit});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip1.Size = new System.Drawing.Size(1030, 45);
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
            // tsButImport
            // 
            this.tsButImport.BackColor = System.Drawing.Color.Transparent;
            this.tsButImport.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsButImport.Image = global::Hydrology.Properties.Resources.导入;
            this.tsButImport.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsButImport.Name = "tsButImport";
            this.tsButImport.Size = new System.Drawing.Size(39, 42);
            this.tsButImport.Text = "导入数据";
            this.tsButImport.Click += new System.EventHandler(this.tsButImport_Click);
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
            // statusStrip1
            // 
            this.statusStrip1.BackgroundImage = global::Hydrology.Properties.Resources.状态栏;
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLabel});
            this.statusStrip1.Location = new System.Drawing.Point(0, 463);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1030, 22);
            this.statusStrip1.TabIndex = 0;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // statusLabel
            // 
            this.statusLabel.ForeColor = System.Drawing.Color.White;
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(0, 17);
            // 
            // CWaterFlowMapForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1030, 485);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CWaterFlowMapForm";
            this.Text = "水位流量(库容)管理";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CWaterFlowMapForm_FormClosing);
            this.panelLeft.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelLeft;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton tsButSave;
        private System.Windows.Forms.ToolStripButton tsButExit;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel statusLabel;
        private System.Windows.Forms.ToolStripButton tsButImport;
        private System.Windows.Forms.Panel panel_Right;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label labelInfoSelect;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TextBox textBox_Search;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox listBox_StationName;
        private System.Windows.Forms.ComboBox cmb_SubCenter;
        private System.Windows.Forms.Label label7;
    }
}
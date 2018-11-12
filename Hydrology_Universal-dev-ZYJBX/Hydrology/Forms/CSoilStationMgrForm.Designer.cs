namespace Hydrology.Forms
{
    partial class CSoilStationMgrForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CSoilStationMgrForm));
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.labelUserCount = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.cmb_SubCenter = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox_Search = new System.Windows.Forms.TextBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tsButSave = new System.Windows.Forms.ToolStripButton();
            this.tsButRevert = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsButImport = new System.Windows.Forms.ToolStripButton();
            this.tsButAdd = new System.Windows.Forms.ToolStripButton();
            this.tsButDelete = new System.Windows.Forms.ToolStripButton();
            this.tsButExit = new System.Windows.Forms.ToolStripButton();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.tableLayoutPanel.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel4.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Controls.Add(this.statusStrip1);
            this.panel1.Controls.Add(this.toolStrip1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(891, 472);
            this.panel1.TabIndex = 1;
            // 
            // panel2
            // 
            this.panel2.BackgroundImage = global::Hydrology.Properties.Resources.标签头部;
            this.panel2.Controls.Add(this.tableLayoutPanel);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 45);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(891, 405);
            this.panel2.TabIndex = 2;
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.ColumnCount = 1;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.Controls.Add(this.panel3, 0, 2);
            this.tableLayoutPanel.Controls.Add(this.panel4, 0, 0);
            this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 3;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.tableLayoutPanel.Size = new System.Drawing.Size(891, 405);
            this.tableLayoutPanel.TabIndex = 0;
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.SystemColors.Info;
            this.panel3.Controls.Add(this.labelUserCount);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(3, 379);
            this.panel3.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(885, 23);
            this.panel3.TabIndex = 0;
            // 
            // labelUserCount
            // 
            this.labelUserCount.Dock = System.Windows.Forms.DockStyle.Left;
            this.labelUserCount.Location = new System.Drawing.Point(0, 0);
            this.labelUserCount.Name = "labelUserCount";
            this.labelUserCount.Size = new System.Drawing.Size(155, 23);
            this.labelUserCount.TabIndex = 0;
            this.labelUserCount.Text = "共0个墒情站配置";
            this.labelUserCount.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panel4
            // 
            this.panel4.BackgroundImage = global::Hydrology.Properties.Resources.菜单栏;
            this.panel4.Controls.Add(this.cmb_SubCenter);
            this.panel4.Controls.Add(this.label4);
            this.panel4.Controls.Add(this.label3);
            this.panel4.Controls.Add(this.label2);
            this.panel4.Controls.Add(this.textBox_Search);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.Location = new System.Drawing.Point(0, 0);
            this.panel4.Margin = new System.Windows.Forms.Padding(0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(891, 25);
            this.panel4.TabIndex = 1;
            // 
            // cmb_SubCenter
            // 
            this.cmb_SubCenter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_SubCenter.FormattingEnabled = true;
            this.cmb_SubCenter.Location = new System.Drawing.Point(506, 2);
            this.cmb_SubCenter.Name = "cmb_SubCenter";
            this.cmb_SubCenter.Size = new System.Drawing.Size(121, 20);
            this.cmb_SubCenter.TabIndex = 19;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Location = new System.Drawing.Point(423, 6);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(77, 12);
            this.label4.TabIndex = 18;
            this.label4.Text = "分中心选择：";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Location = new System.Drawing.Point(633, 5);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(113, 12);
            this.label3.TabIndex = 16;
            this.label3.Text = "按站号或站名查找：";
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Location = new System.Drawing.Point(-3, 2);
            this.label2.Name = "label2";
            this.label2.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.label2.Size = new System.Drawing.Size(112, 20);
            this.label2.TabIndex = 0;
            this.label2.Text = "墒情站列表";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // textBox_Search
            // 
            this.textBox_Search.Location = new System.Drawing.Point(747, 2);
            this.textBox_Search.Name = "textBox_Search";
            this.textBox_Search.Size = new System.Drawing.Size(141, 21);
            this.textBox_Search.TabIndex = 17;
            this.textBox_Search.TextChanged += new System.EventHandler(this.textBox_Search_TextChanged);
            // 
            // statusStrip1
            // 
            this.statusStrip1.BackgroundImage = global::Hydrology.Properties.Resources.状态栏;
            this.statusStrip1.Location = new System.Drawing.Point(0, 450);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(891, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
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
            this.tsButImport,
            this.tsButAdd,
            this.tsButDelete,
            this.tsButExit});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip1.Size = new System.Drawing.Size(891, 45);
            this.toolStrip1.TabIndex = 0;
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
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 45);
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
            // tsButAdd
            // 
            this.tsButAdd.AutoSize = false;
            this.tsButAdd.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsButAdd.Image = global::Hydrology.Properties.Resources.add;
            this.tsButAdd.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsButAdd.Name = "tsButAdd";
            this.tsButAdd.Size = new System.Drawing.Size(50, 45);
            this.tsButAdd.Text = "添加";
            // 
            // tsButDelete
            // 
            this.tsButDelete.AutoSize = false;
            this.tsButDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsButDelete.Image = global::Hydrology.Properties.Resources.delete2;
            this.tsButDelete.ImageTransparentColor = System.Drawing.SystemColors.ButtonHighlight;
            this.tsButDelete.Name = "tsButDelete";
            this.tsButDelete.Size = new System.Drawing.Size(35, 35);
            this.tsButDelete.Text = "删除";
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
         
            // 
            // CSoilStationMgrForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(891, 472);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "CSoilStationMgrForm";
            this.Text = "墒情站管理";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.tableLayoutPanel.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label labelUserCount;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton tsButSave;
        private System.Windows.Forms.ToolStripButton tsButRevert;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton tsButAdd;
        private System.Windows.Forms.ToolStripButton tsButDelete;
        private System.Windows.Forms.ToolStripButton tsButExit;
        private System.Windows.Forms.ToolStripButton tsButImport;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox_Search;
        private System.Windows.Forms.ComboBox cmb_SubCenter;
        private System.Windows.Forms.Label label4;
    }
}
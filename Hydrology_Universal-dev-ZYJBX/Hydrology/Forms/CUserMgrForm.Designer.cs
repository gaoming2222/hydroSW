namespace Hydrology.Forms
{
    partial class CUserMgrForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CUserMgrForm));
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.labelUserCount = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tsButSave = new System.Windows.Forms.ToolStripButton();
            this.tsButRevert = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsButAddUser = new System.Windows.Forms.ToolStripButton();
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
            this.panel1.Size = new System.Drawing.Size(488, 360);
            this.panel1.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.BackgroundImage = global::Hydrology.Properties.Resources.标签头部;
            this.panel2.Controls.Add(this.tableLayoutPanel);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 45);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(488, 293);
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
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel.Size = new System.Drawing.Size(488, 293);
            this.tableLayoutPanel.TabIndex = 0;
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.SystemColors.Info;
            this.panel3.Controls.Add(this.labelUserCount);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(3, 268);
            this.panel3.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(482, 22);
            this.panel3.TabIndex = 0;
            // 
            // labelUserCount
            // 
            this.labelUserCount.Dock = System.Windows.Forms.DockStyle.Left;
            this.labelUserCount.Location = new System.Drawing.Point(0, 0);
            this.labelUserCount.Name = "labelUserCount";
            this.labelUserCount.Size = new System.Drawing.Size(72, 22);
            this.labelUserCount.TabIndex = 0;
            this.labelUserCount.Text = "共0个用户";
            this.labelUserCount.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panel4
            // 
            this.panel4.BackgroundImage = global::Hydrology.Properties.Resources.菜单栏;
            this.panel4.Controls.Add(this.label2);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.Location = new System.Drawing.Point(0, 0);
            this.panel4.Margin = new System.Windows.Forms.Padding(0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(488, 20);
            this.panel4.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Location = new System.Drawing.Point(-3, 0);
            this.label2.Name = "label2";
            this.label2.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.label2.Size = new System.Drawing.Size(112, 20);
            this.label2.TabIndex = 0;
            this.label2.Text = "用户列表";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // statusStrip1
            // 
            this.statusStrip1.BackgroundImage = global::Hydrology.Properties.Resources.状态栏;
            this.statusStrip1.Location = new System.Drawing.Point(0, 338);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(488, 22);
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
            this.tsButAddUser,
            this.tsButDelete,
            this.tsButExit});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip1.Size = new System.Drawing.Size(488, 45);
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
            // tsButAddUser
            // 
            this.tsButAddUser.AutoSize = false;
            this.tsButAddUser.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsButAddUser.Image = global::Hydrology.Properties.Resources.add_user;
            this.tsButAddUser.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsButAddUser.Name = "tsButAddUser";
            this.tsButAddUser.Size = new System.Drawing.Size(50, 45);
            this.tsButAddUser.Text = "添加";
            this.tsButAddUser.Click += new System.EventHandler(this.tsButAddUser_Click);
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
            this.tsButDelete.Click += new System.EventHandler(this.tsButDelete_Click);
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
            // CUserMgrForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(488, 360);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CUserMgrForm";
            this.Text = "用户管理";
            this.Load += new System.EventHandler(this.CUserMgrForm_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.tableLayoutPanel.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton tsButSave;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton tsButAddUser;
        private System.Windows.Forms.ToolStripButton tsButDelete;
        private System.Windows.Forms.ToolStripButton tsButExit;
        private System.Windows.Forms.ToolStripButton tsButRevert;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label labelUserCount;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Label label2;


    }
}
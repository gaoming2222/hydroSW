namespace Hydrology.Forms
{
    partial class FlowRateReport
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FlowRateReport));
            this.SubCenter = new System.Windows.Forms.Panel();
            this.Export = new System.Windows.Forms.Button();
            this.sort = new System.Windows.Forms.Button();
            this.DateTimer = new System.Windows.Forms.DateTimePicker();
            this.label3 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.TabelType = new System.Windows.Forms.ComboBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.SubCenter.SuspendLayout();
            this.SuspendLayout();
            // 
            // SubCenter
            // 
            this.SubCenter.Controls.Add(this.Export);
            this.SubCenter.Controls.Add(this.sort);
            this.SubCenter.Controls.Add(this.DateTimer);
            this.SubCenter.Controls.Add(this.label3);
            this.SubCenter.Controls.Add(this.comboBox1);
            this.SubCenter.Controls.Add(this.label2);
            this.SubCenter.Controls.Add(this.label1);
            this.SubCenter.Controls.Add(this.TabelType);
            this.SubCenter.Dock = System.Windows.Forms.DockStyle.Top;
            this.SubCenter.Location = new System.Drawing.Point(0, 0);
            this.SubCenter.Name = "SubCenter";
            this.SubCenter.Size = new System.Drawing.Size(975, 74);
            this.SubCenter.TabIndex = 0;
            // 
            // Export
            // 
            this.Export.Location = new System.Drawing.Point(828, 23);
            this.Export.Name = "Export";
            this.Export.Size = new System.Drawing.Size(91, 40);
            this.Export.TabIndex = 7;
            this.Export.Text = "导出Excel";
            this.Export.UseVisualStyleBackColor = true;
            this.Export.Click += new System.EventHandler(this.btnExportToExcel_Click);
            // 
            // sort
            // 
            this.sort.Location = new System.Drawing.Point(701, 23);
            this.sort.Name = "sort";
            this.sort.Size = new System.Drawing.Size(91, 40);
            this.sort.TabIndex = 6;
            this.sort.Text = "查询";
            this.sort.UseVisualStyleBackColor = true;
            this.sort.Click += new System.EventHandler(this.btnQuery_Click);
            // 
            // DateTimer
            // 
            this.DateTimer.Location = new System.Drawing.Point(515, 33);
            this.DateTimer.Name = "DateTimer";
            this.DateTimer.Size = new System.Drawing.Size(112, 21);
            this.DateTimer.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(474, 37);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 12);
            this.label3.TabIndex = 4;
            this.label3.Text = "日期:";
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(296, 34);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(111, 20);
            this.comboBox1.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(243, 37);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(47, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "分中心:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 37);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "报表类型:";
            // 
            // TabelType
            // 
            this.TabelType.FormattingEnabled = true;
            this.TabelType.Items.AddRange(new object[] {
            "日报表",
            "月报表"});
            this.TabelType.Location = new System.Drawing.Point(66, 34);
            this.TabelType.Name = "TabelType";
            this.TabelType.Size = new System.Drawing.Size(94, 20);
            this.TabelType.TabIndex = 0;
            this.TabelType.SelectedIndexChanged += new System.EventHandler(this.TabelType_SelectedIndexChanged);
            // 
            // panel2
            // 
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 74);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(975, 354);
            this.panel2.TabIndex = 1;
            // 
            // FlowRateReport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(975, 428);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.SubCenter);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FlowRateReport";
            this.Text = "定时数据接收畅通率报表";
            this.SubCenter.ResumeLayout(false);
            this.SubCenter.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel SubCenter;
        private System.Windows.Forms.Panel panel2;
        public System.Windows.Forms.ComboBox TabelType;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Button sort;
        private System.Windows.Forms.DateTimePicker DateTimer;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button Export;

    }
}
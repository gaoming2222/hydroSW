namespace Hydrology.Forms
{
    partial class MoreStationsMonthForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MoreStationsMonthForm));
            this.MessagePanel = new System.Windows.Forms.Panel();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.Export = new System.Windows.Forms.Button();
            this.btnButton = new System.Windows.Forms.Button();
            this.DateTimer = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.SubCenter = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.TableType = new System.Windows.Forms.GroupBox();
            //this.soil = new System.Windows.Forms.RadioButton();
            this.rain = new System.Windows.Forms.RadioButton();
            this.water = new System.Windows.Forms.RadioButton();
            this.TablePanel = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.StationSelect = new System.Windows.Forms.CheckedListBox();
            this.MessagePanel.SuspendLayout();
            this.TableType.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // MessagePanel
            // 
            this.MessagePanel.Controls.Add(this.checkBox1);
            this.MessagePanel.Controls.Add(this.Export);
            this.MessagePanel.Controls.Add(this.btnButton);
            this.MessagePanel.Controls.Add(this.DateTimer);
            this.MessagePanel.Controls.Add(this.label2);
            this.MessagePanel.Controls.Add(this.SubCenter);
            this.MessagePanel.Controls.Add(this.label1);
            this.MessagePanel.Controls.Add(this.TableType);
            this.MessagePanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.MessagePanel.Location = new System.Drawing.Point(0, 0);
            this.MessagePanel.Name = "MessagePanel";
            this.MessagePanel.Size = new System.Drawing.Size(1284, 98);
            this.MessagePanel.TabIndex = 0;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(316, 50);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(48, 16);
            this.checkBox1.TabIndex = 9;
            this.checkBox1.Text = "全选";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // Export
            // 
            this.Export.Location = new System.Drawing.Point(858, 27);
            this.Export.Name = "Export";
            this.Export.Size = new System.Drawing.Size(80, 54);
            this.Export.TabIndex = 8;
            this.Export.Text = "导出到Excel";
            this.Export.UseVisualStyleBackColor = true;
            this.Export.Click += new System.EventHandler(this.export_Click);
            // 
            // btnButton
            // 
            this.btnButton.Location = new System.Drawing.Point(742, 27);
            this.btnButton.Name = "btnButton";
            this.btnButton.Size = new System.Drawing.Size(80, 54);
            this.btnButton.TabIndex = 6;
            this.btnButton.Text = "查询";
            this.btnButton.UseVisualStyleBackColor = true;
            this.btnButton.Click += new System.EventHandler(this.search_Click);
            // 
            // DateTimer
            // 
            this.DateTimer.CustomFormat = "yyyy年MM月dd日";
            this.DateTimer.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.DateTimer.Location = new System.Drawing.Point(492, 47);
            this.DateTimer.Name = "DateTimer";
            this.DateTimer.ShowUpDown = true;
            this.DateTimer.Size = new System.Drawing.Size(117, 21);
            this.DateTimer.TabIndex = 7;
            this.DateTimer.Value = new System.DateTime(2010, 1, 1, 0, 0, 0, 0);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(427, 50);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 12);
            this.label2.TabIndex = 4;
            this.label2.Text = "日   期：";
            // 
            // SubCenter
            // 
            this.SubCenter.FormattingEnabled = true;
            this.SubCenter.Location = new System.Drawing.Point(189, 47);
            this.SubCenter.Name = "SubCenter";
            this.SubCenter.Size = new System.Drawing.Size(85, 20);
            this.SubCenter.TabIndex = 3;
            this.SubCenter.SelectedIndexChanged += new System.EventHandler(this.center_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(130, 50);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "分中心：";
            // 
            // TableType
            // 
            //this.TableType.Controls.Add(this.soil);
            this.TableType.Controls.Add(this.rain);
            this.TableType.Controls.Add(this.water);
            this.TableType.Location = new System.Drawing.Point(3, 3);
            this.TableType.Name = "TableType";
            this.TableType.Size = new System.Drawing.Size(121, 89);
            this.TableType.TabIndex = 0;
            this.TableType.TabStop = false;
            this.TableType.Text = "报表类型";
            // 
            // soil
            // 
            //this.soil.AutoSize = true;
            //this.soil.Location = new System.Drawing.Point(0, 67);
            //this.soil.Name = "soil";
            //this.soil.Size = new System.Drawing.Size(71, 16);
            //this.soil.TabIndex = 2;
            //this.soil.TabStop = true;
            //this.soil.Text = "墒    情";
            //this.soil.UseVisualStyleBackColor = true;
            //this.soil.CheckedChanged += new System.EventHandler(this.TableTypeChanged);
            // 
            // rain
            // 
            this.rain.AutoSize = true;
            this.rain.Checked = true;
            this.rain.Location = new System.Drawing.Point(0, 20);
            this.rain.Name = "rain";
            this.rain.Size = new System.Drawing.Size(71, 16);
            this.rain.TabIndex = 1;
            this.rain.TabStop = true;
            this.rain.Text = "雨    量";
            this.rain.UseVisualStyleBackColor = true;
            this.rain.CheckedChanged += new System.EventHandler(this.TableTypeChanged);
            // 
            // water
            // 
            this.water.AutoSize = true;
            this.water.Location = new System.Drawing.Point(0, 43);
            this.water.Name = "water";
            this.water.Size = new System.Drawing.Size(71, 16);
            this.water.TabIndex = 1;
            this.water.TabStop = true;
            this.water.Text = "水    位";
            this.water.UseVisualStyleBackColor = true;
            this.water.CheckedChanged += new System.EventHandler(this.TableTypeChanged);
            // 
            // TablePanel
            // 
            this.TablePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TablePanel.Location = new System.Drawing.Point(0, 98);
            this.TablePanel.Name = "TablePanel";
            this.TablePanel.Size = new System.Drawing.Size(1284, 505);
            this.TablePanel.TabIndex = 2;
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.StationSelect);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(0, 98);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(124, 505);
            this.panel1.TabIndex = 0;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 15);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 12);
            this.label3.TabIndex = 0;
            this.label3.Text = "站点选择:";
            // 
            // StationSelect
            // 
            this.StationSelect.FormattingEnabled = true;
            this.StationSelect.Location = new System.Drawing.Point(4, 43);
            this.StationSelect.Name = "StationSelect";
            this.StationSelect.Size = new System.Drawing.Size(120, 388);
            this.StationSelect.TabIndex = 0;
            // 
            // MoreStationsMonthForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1284, 603);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.TablePanel);
            this.Controls.Add(this.MessagePanel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "MoreStationsMonthForm";
            this.Text = "多站月报表";
            this.MessagePanel.ResumeLayout(false);
            this.MessagePanel.PerformLayout();
            this.TableType.ResumeLayout(false);
            this.TableType.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel MessagePanel;
        private System.Windows.Forms.Panel TablePanel;
        private System.Windows.Forms.GroupBox TableType;
        //private System.Windows.Forms.RadioButton soil;
        private System.Windows.Forms.RadioButton rain;
        private System.Windows.Forms.RadioButton water;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox SubCenter;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DateTimePicker DateTimer;
        private System.Windows.Forms.Button Export;
        private System.Windows.Forms.Button btnButton;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckedListBox StationSelect;
        private System.Windows.Forms.CheckBox checkBox1;
    }
}
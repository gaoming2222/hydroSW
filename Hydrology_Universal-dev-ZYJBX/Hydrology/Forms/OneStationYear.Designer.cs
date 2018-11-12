using System;
using System.Windows.Forms;

namespace Hydrology.Forms
{
    partial class OneStationYear
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OneStationYear));
            this.panel1 = new System.Windows.Forms.Panel();
            this.export = new System.Windows.Forms.Button();
            this.exit = new System.Windows.Forms.Button();
            this.search = new System.Windows.Forms.Button();
            this.date = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.center = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.type = new System.Windows.Forms.GroupBox();
            this.water = new System.Windows.Forms.RadioButton();
            this.rain = new System.Windows.Forms.RadioButton();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.stationSelect = new System.Windows.Forms.CheckedListBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.type.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.export);
            this.panel1.Controls.Add(this.exit);
            this.panel1.Controls.Add(this.search);
            this.panel1.Controls.Add(this.date);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.center);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.type);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1354, 95);
            this.panel1.TabIndex = 1;
            // 
            // export
            // 
            this.export.Location = new System.Drawing.Point(651, 28);
            this.export.Name = "export";
            this.export.Size = new System.Drawing.Size(101, 41);
            this.export.TabIndex = 7;
            this.export.Text = "导出Excel表格";
            this.export.UseVisualStyleBackColor = true;
            this.export.Click += new System.EventHandler(this.export_Click);
            // 
            // exit
            // 
            this.exit.Location = new System.Drawing.Point(566, 54);
            this.exit.Name = "exit";
            this.exit.Size = new System.Drawing.Size(59, 30);
            this.exit.TabIndex = 6;
            this.exit.Text = "退出";
            this.exit.UseVisualStyleBackColor = true;
            this.exit.Click += new System.EventHandler(this.exit_Click);
            // 
            // search
            // 
            this.search.Location = new System.Drawing.Point(566, 12);
            this.search.Name = "search";
            this.search.Size = new System.Drawing.Size(59, 33);
            this.search.TabIndex = 5;
            this.search.Text = "查询";
            this.search.UseVisualStyleBackColor = true;
            this.search.Click += new System.EventHandler(this.search_Click);
            // 
            // date
            // 
            this.date.CustomFormat = "yyyy年MM月";
            this.date.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.date.Location = new System.Drawing.Point(349, 40);
            this.date.Name = "date";
            this.date.ShowUpDown = true;
            this.date.Size = new System.Drawing.Size(114, 21);
            this.date.TabIndex = 4;
            this.date.Value = new System.DateTime(2016, 9, 1, 0, 0, 0, 0);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(291, 44);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "日   期：";
            // 
            // center
            // 
            this.center.FormattingEnabled = true;
            this.center.Location = new System.Drawing.Point(168, 39);
            this.center.Name = "center";
            this.center.Size = new System.Drawing.Size(78, 20);
            this.center.TabIndex = 2;
            this.center.SelectedIndexChanged += new System.EventHandler(this.center_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(121, 42);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "分中心：";
            // 
            // type
            // 
            this.type.Controls.Add(this.water);
            this.type.Controls.Add(this.rain);
            this.type.Location = new System.Drawing.Point(3, 0);
            this.type.Name = "type";
            this.type.Size = new System.Drawing.Size(91, 92);
            this.type.TabIndex = 0;
            this.type.TabStop = false;
            this.type.Text = "报表类型";
            // 
            // water
            // 
            this.water.AutoSize = true;
            this.water.Location = new System.Drawing.Point(6, 20);
            this.water.Name = "water";
            this.water.Size = new System.Drawing.Size(59, 16);
            this.water.TabIndex = 1;
            this.water.TabStop = true;
            this.water.Text = "水  位";
            this.water.UseVisualStyleBackColor = true;
            this.water.CheckedChanged += new System.EventHandler(this.TableTypeChanged);
            // 
            // rain
            // 
            this.rain.AutoSize = true;
            this.rain.Location = new System.Drawing.Point(6, 61);
            this.rain.Name = "rain";
            this.rain.Size = new System.Drawing.Size(59, 16);
            this.rain.TabIndex = 2;
            this.rain.TabStop = true;
            this.rain.Text = "雨  量";
            this.rain.UseVisualStyleBackColor = true;
            this.rain.CheckedChanged += new System.EventHandler(this.TableTypeChanged);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.stationSelect);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel2.Location = new System.Drawing.Point(0, 95);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(130, 516);
            this.panel2.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 14);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 0;
            this.label3.Text = "站点选择";
            // 
            // stationSelect
            // 
            this.stationSelect.FormattingEnabled = true;
            this.stationSelect.Items.AddRange(new object[] {
            ""});
            this.stationSelect.Location = new System.Drawing.Point(5, 43);
            this.stationSelect.Name = "stationSelect";
            this.stationSelect.Size = new System.Drawing.Size(104, 468);
            this.stationSelect.TabIndex = 0;
            this.stationSelect.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.staitonSelect_ItemCheck);
            this.stationSelect.SelectedIndexChanged += new System.EventHandler(this.stationSelect_SelectedIndexChanged);
            // 
            // panel3
            // 
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(130, 95);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1224, 516);
            this.panel3.TabIndex = 3;
            // 
            // OneStationYear
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1354, 611);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "OneStationYear";
            this.Text = "单站年报表";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.type.ResumeLayout(false);
            this.type.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }





        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button export;
        private System.Windows.Forms.Button exit;
        private System.Windows.Forms.Button search;
        private System.Windows.Forms.DateTimePicker date;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox center;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox type;
        private System.Windows.Forms.RadioButton water;
        private System.Windows.Forms.RadioButton rain;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckedListBox stationSelect;
        private System.Windows.Forms.Panel panel3;
    }
}
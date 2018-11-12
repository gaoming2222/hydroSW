namespace Hydrology.Forms
{
    partial class CWelcomePage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CWelcomePage));
            this.panel1 = new System.Windows.Forms.Panel();
            this.m_label = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::Hydrology.Properties.Resources.欢迎界面;
            this.panel1.Controls.Add(this.m_label);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(685, 457);
            this.panel1.TabIndex = 0;
            // 
            // m_label
            // 
            this.m_label.AutoSize = true;
            this.m_label.BackColor = System.Drawing.Color.Transparent;
            this.m_label.Font = new System.Drawing.Font("Microsoft YaHei", 18F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.m_label.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.m_label.Location = new System.Drawing.Point(64, 335);
            this.m_label.Name = "m_label";
            this.m_label.Size = new System.Drawing.Size(128, 31);
            this.m_label.TabIndex = 0;
            this.m_label.Text = "正在加载...";
            this.m_label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // CWelcomePage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(685, 457);
            this.Controls.Add(this.panel1);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "CWelcomePage";
            this.ShowIcon = false;
            this.Text = "欢迎使用水文监测管理系统";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label m_label;
    }
}
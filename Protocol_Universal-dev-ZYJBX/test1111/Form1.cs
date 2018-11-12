using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Protocol.Channel.Interface;
using Protocol.Channel.HDGprs;

namespace test1111
{
    public partial class Form1 : Form
    {
        
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            IHDGprs parser = new HDGpesParser();
            parser.DSStartService(5002,1,2,null,this.Handle);
            
           // parser.AddPort(5003);
        }
    }
}

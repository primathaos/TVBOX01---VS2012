using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TVBOX01
{
    public partial class Reprint : Form
    {
        public Reprint()
        {
            InitializeComponent();
            this.ControlBox = false;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            Dataset1.Context.ContextData.Clear();
            Dataset1.Context.ContextData.Add("Key1",this.button1.Text);
            this.Close();
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            Dataset1.Context.ContextData.Clear();
            Dataset1.Context.ContextData.Add("Key1", this.button2.Text);
            this.Close();
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            Dataset1.Context.ContextData.Clear();
            Dataset1.Context.ContextData.Add("Key1", this.button3.Text);
            this.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Dataset1.Context.ContextData.Clear();
            Dataset1.Context.ContextData.Add("Key1", this.button4.Text);
            this.Close();
        }
    }
}

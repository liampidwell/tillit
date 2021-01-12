using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ePOS
{
    public partial class frmMessageBox : Form
    {
        public frmMessageBox(string theText)
        {
            InitializeComponent();
            label2.Text = theText;
        }

        private void button40_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}

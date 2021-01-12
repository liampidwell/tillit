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
    public partial class frmInputBox : Form
    {
        private bool keysEnabled;
        private bool numsEnabled;

        private int theMinTextLength;
        private int theMaxTextLength;
        public frmInputBox(string theQuestion, bool enableKeys, bool enableNums, int theMinLength, int theMaxLength)
        {
            InitializeComponent();
            label2.Text = theQuestion;
            theMinTextLength = theMinLength;
            theMaxTextLength = theMaxLength;

            keysEnabled = enableKeys;
            numsEnabled = enableNums;

            theKeys(enableKeys);
            theNums(enableNums);
        }

        private void theKeys(bool theState)
        {
            button11.Enabled = theState;
            button12.Enabled = theState;
            button13.Enabled = theState;
            button14.Enabled = theState;
            button15.Enabled = theState;
            button16.Enabled = theState;
            button17.Enabled = theState;
            button18.Enabled = theState;
            button19.Enabled = theState;
            button20.Enabled = theState;
            button21.Enabled = theState;
            button22.Enabled = theState;
            button23.Enabled = theState;
            button24.Enabled = theState;
            button25.Enabled = theState;
            button26.Enabled = theState;
            button27.Enabled = theState;
            button28.Enabled = theState;
            button29.Enabled = theState;
            button30.Enabled = theState;
            button31.Enabled = theState;
            button32.Enabled = theState;
            button33.Enabled = theState;
            button34.Enabled = theState;
            button35.Enabled = theState;
            button36.Enabled = theState;
            button37.Enabled = theState;
            button38.Enabled = theState;
        }

        private void theNums(bool theState)
        {
            button1.Enabled = theState;
            button2.Enabled = theState;
            button3.Enabled = theState;
            button4.Enabled = theState;
            button5.Enabled = theState;
            button6.Enabled = theState;
            button7.Enabled = theState;
            button8.Enabled = theState;
            button9.Enabled = theState;
            button10.Enabled = theState;
        }

        public string getTheResponse()
        {
            return label1.Text;
        }

        private void checkTheLength()
        {
            if (label1.Text.Length >= theMinTextLength)
            {
                button40.Enabled = true;
                theKeys(false);
                theNums(false);
            }
            else
            {
                button40.Enabled = false;
                theKeys(keysEnabled);
                theNums(numsEnabled);
            }
        }

        private void keyboardButtonHandler(object sender, EventArgs e)
        {
            Button theButt = sender as Button;
            if (label1.Text.Length < theMaxTextLength)
            {
                label1.Text = label1.Text + theButt.Text;
            }
            checkTheLength();
        }

        private void button38_Click(object sender, EventArgs e)
        {
            if (label1.Text.Length < theMaxTextLength)
            {
                label1.Text = label1.Text + " ";
            }
            checkTheLength();
        }

        private void button40_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button41_Click(object sender, EventArgs e)
        {
            label1.Text = "";
            checkTheLength();
        }

        private void button39_Click(object sender, EventArgs e)
        {
            if (label1.Text.Length != 0)
            {
                label1.Text = label1.Text.Remove(label1.Text.Length - 1);
            }
            checkTheLength();
        }
    }
}

using MySqlConnector;
using System;
using System.Windows.Forms;

namespace ePOS
{
    public partial class frmMain : Form
    {
        private MySqlConnection theConnection = Extensions.theConnection;

        public frmMain()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            StartTimer();
        }

        private void logIn()
        {
            if (button3.Text == "Log In")
            {
                string thePasscode = Extensions.inputBox("Please input your employee pin", false, true, 4, 4);
                bool isFound = false;

                MySqlCommand theCommand = new MySqlCommand("SELECT emp_name, emp_passcode FROM oap_employees WHERE emp_passcode = " + thePasscode, theConnection);
                theConnection.Open();
                MySqlDataReader reader = theCommand.ExecuteReader();
                while (reader.Read())
                {
                    lblName.Text = reader.GetString("emp_name");
                    isFound = true;
                }
                theConnection.Close();

                if (isFound)
                {
                    checkBox1.Enabled = true;
                    button1.Enabled = true;
                    button2.Enabled = true;
                    button4.Enabled = true;
                    button5.Enabled = true;

                    button3.Text = "Log Out";

                    setThePage("1", tableLayoutPanel2);
                    setThePage("5", tableLayoutPanel1);
                }
                else
                {
                    Extensions.messageBox("User not found.");
                }
            }
            else
            {
                checkBox1.Enabled = false;
                button1.Enabled = false;
                button2.Enabled = false;
                button4.Enabled = false;
                button5.Enabled = false;

                button3.Text = "Log In";

                tableLayoutPanel2.Controls.Clear();
                tableLayoutPanel1.Controls.Clear();
                lblName.Text = "";
            }
        }

        Timer t = null;
        private void StartTimer()
        {
            t = new System.Windows.Forms.Timer();
            t.Interval = 500;
            t.Tick += new EventHandler(t_Tick);
            t.Enabled = true;
        }

        void t_Tick(object sender, EventArgs e)
        {
            lblTime.Text = DateTime.Now.ToString("dd MMM yyyy HH:mm");
        }

        private void setThePage(string thePageID, TableLayoutPanel thePanel)
        {
            thePanel.Controls.Clear();

            thePanel.Tag = thePageID;

            MySqlCommand theCommand = new MySqlCommand("SELECT btnpage_columns, btnpage_rows FROM oap_button_pages WHERE btnpage_id = " + thePageID, theConnection);
            theConnection.Open();
            MySqlDataReader reader = theCommand.ExecuteReader();
            while (reader.Read())
            {
                thePanel.RowCount = reader.GetInt32("btnpage_rows");
                thePanel.ColumnCount = reader.GetInt32("btnpage_columns");
            }
            theConnection.Close();

            thePanel.RowStyles.Clear();
            for (int y = 1; y <= thePanel.RowCount; y++)
            {
                RowStyle theStyle = new RowStyle(SizeType.Percent);
                theStyle.Height = 100 / (thePanel.RowCount);
                thePanel.RowStyles.Add(theStyle);
            }

            thePanel.ColumnStyles.Clear();
            for (int y = 1; y <= thePanel.ColumnCount; y++)
            {
                ColumnStyle theStyle = new ColumnStyle(SizeType.Percent);
                theStyle.Width = 100 / (thePanel.ColumnCount);
                thePanel.ColumnStyles.Add(theStyle);
            }

            theCommand = new MySqlCommand("SELECT * FROM oap_buttons WHERE button_page_id = " + thePageID + " AND button_isitem = 0", theConnection);
            theConnection.Open();
            reader = theCommand.ExecuteReader();
            while (reader.Read())
            {
                Button theButton = Extensions.newButton(reader.GetString("button_text"), reader.GetString("button_forecol"), reader.GetString("button_backcol"), reader.GetInt32("button_foreignkey").ToString(), 15);
                theButton.Click += catButtClick;
                thePanel.Controls.Add(theButton, reader.GetUInt16("button_column"), reader.GetUInt16("button_row"));
                thePanel.SetColumnSpan(theButton, reader.GetUInt16("button_colspan"));
                thePanel.SetRowSpan(theButton, reader.GetUInt16("button_rowspan"));
            }
            theConnection.Close();

            theCommand = new MySqlCommand("SELECT oap_buttons.*, oap_items.item_kitchenname, oap_items.item_isinstock FROM oap_buttons INNER JOIN oap_items ON oap_items.pk_item_id = button_foreignkey WHERE button_page_id = " + thePageID + " AND button_isitem = 1", theConnection);
            theConnection.Open();
            reader = theCommand.ExecuteReader();
            while (reader.Read())
            {
                Button theButton = Extensions.newButton(reader.GetString("item_kitchenname"), reader.GetString("button_forecol"), reader.GetString("button_backcol"), reader.GetInt32("button_foreignkey").ToString(), 15);
                if (reader.GetInt32("item_isinstock") == 0)
                {
                    theButton = theButton = Extensions.newButton(reader.GetString("item_kitchenname"), reader.GetString("button_forecol"), reader.GetString("button_backcol"), reader.GetInt32("button_foreignkey").ToString(), 15, false);
                }
                theButton.Click += productButtClick;
                thePanel.Controls.Add(theButton, reader.GetUInt16("button_column"), reader.GetUInt16("button_row"));
                thePanel.SetColumnSpan(theButton, reader.GetUInt16("button_colspan"));
                thePanel.SetRowSpan(theButton, reader.GetUInt16("button_rowspan"));
            }
            theConnection.Close();
        }

        private void getTheModifiers(string theProductID)
        {
            frmModifier theForm = new frmModifier(theProductID);
            theForm.ShowDialog();
            MySqlConnection theSecondConnection = new MySqlConnection(Extensions.theConnectionString);
            MySqlCommand theCommand = new MySqlCommand("SELECT * FROM oap_mods WHERE pk_modifier_id = " + theForm.getModifier(), theSecondConnection);
            theSecondConnection.Open();
            MySqlDataReader reader = theCommand.ExecuteReader();
            while (reader.Read())
            {
                string thePrice = "";
                if (reader.GetDouble("modifier_price").ToString("n2") != "0.00")
                {
                    thePrice = reader.GetDouble("modifier_price").ToString("n2");
                }
                dataGridView1.Rows.Add("", reader.GetString("modifier_kitchen_text"), thePrice, reader.GetInt32("pk_modifier_id").ToString(), "1");
            }
            theSecondConnection.Close();
            dataGridView1.FirstDisplayedScrollingRowIndex = dataGridView1.RowCount - 1;
            updateTheTotal();
        }

        void productButtClick(object sender, EventArgs e)
        {
            Button theButt = sender as Button;
            if (checkBox1.Checked)
            {
                if (setTheStock(theButt.Tag.ToString()) == 0){
                    
                }
            }
            else
            {
                if (theButt.BackgroundImageLayout != ImageLayout.Zoom)
                {
                    addAnItem(theButt.Tag.ToString());
                }
            }
        }

        private int setTheStock(string theProductID)
        {
            frmStockMode theForm = new frmStockMode(theProductID);
            theForm.ShowDialog();

            MySqlCommand theCommand = new MySqlCommand("UPDATE oap_items SET item_isinstock = " + theForm.numInStock().ToString() + " WHERE pk_item_id = " + theProductID, theConnection);
            theConnection.Open();
            theCommand.ExecuteNonQuery();
            theConnection.Close();

            setThePage(tableLayoutPanel2.Tag.ToString(), tableLayoutPanel2);

            return theForm.numInStock();
        }

        void addAnItem(string theItemID)
        {
            MySqlCommand theCommand = new MySqlCommand("SELECT * FROM oap_items WHERE pk_item_id = " + theItemID, theConnection);
            theConnection.Open();
            MySqlDataReader reader = theCommand.ExecuteReader();
            while (reader.Read())
            {
                if (reader.GetBoolean("item_act_as_text"))
                {
                    dataGridView1.Rows.Add("", "     " + reader.GetString("item_kitchenname"), "", reader.GetInt32("pk_item_id").ToString(), "0");
                }
                else
                {
                    dataGridView1.Rows.Add("1", reader.GetString("item_kitchenname"), reader.GetDouble("item_price").ToString("n2"), reader.GetInt32("pk_item_id").ToString(), "0");
                }
            }
            theConnection.Close();
            dataGridView1.FirstDisplayedScrollingRowIndex = dataGridView1.RowCount - 1;
            updateTheTotal();

            theCommand = new MySqlCommand("SELECT oap_modgrp_link.*, oap_modgrp.modgrp_show_ontill FROM oap_modgrp_link INNER JOIN oap_modgrp ON oap_modgrp.pk_modgrp_id = oap_modgrp_link.fk_modgrp_id WHERE modgrp_show_ontill = 1 AND fk_item_id = " + theItemID, theConnection);
            theConnection.Open();
            reader = theCommand.ExecuteReader();
            while (reader.Read())
            {
                getTheModifiers(reader.GetInt32("fk_modgrp_id").ToString());
            }
            theConnection.Close();
        }

        void updateTheTotal()
        {
            double theTotal = 0;
            foreach(DataGridViewRow theRow in dataGridView1.Rows)
            {
                if (theRow.Cells["item_price"].Value.ToString() != "")
                {
                    theTotal = theTotal + double.Parse(theRow.Cells["item_price"].Value.ToString());
                }
            }
            lblTotalPrice.Text = "£" + theTotal.ToString("n2");
        }

        void catButtClick(object sender, EventArgs e)
        {
            Button theButt = sender as Button;
            setThePage(theButt.Tag.ToString(), tableLayoutPanel2);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            setThePage("2", tableLayoutPanel1);
            setThePage("1", tableLayoutPanel2);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            setThePage("5", tableLayoutPanel1);
            setThePage("1", tableLayoutPanel2);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            double theTotal = 0;
            foreach (DataGridViewRow theRow in dataGridView1.Rows)
            {
                if (theRow.Cells["item_price"].Value.ToString() != "")
                {
                    theTotal = theTotal + double.Parse(theRow.Cells["item_price"].Value.ToString());
                }
            }
            lblTotalPrice.Text = "£" + theTotal.ToString("n2");
            Extensions.printBill(dataGridView1.Rows, theTotal);
        }

        private void dataGridView1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Clicked");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            setThePage("15", tableLayoutPanel2);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            logIn();
        }
    }
}

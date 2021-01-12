using MySqlConnector;
using System;
using System.Windows.Forms;

namespace ePOS
{
    public partial class frmStockMode : Form
    {
        private static MySqlConnection theConnection = new MySqlConnection(Extensions.theConnectionString);
        private string theItemNum = "";
        private int theStockNum;

        public frmStockMode(string theProductID)
        {
            InitializeComponent();
            theItemNum = theProductID;
        }

        private void frmStockMode_Load(object sender, EventArgs e)
        {
            MySqlCommand theCommand = new MySqlCommand("SELECT * FROM oap_items WHERE pk_item_id = " + theItemNum, theConnection);
            theConnection.Open();
            MySqlDataReader reader = theCommand.ExecuteReader();
            while (reader.Read())
            {
                if (reader.GetInt32("item_isinstock") == 0)
                {
                    radioButton1.Checked = true;
                }
                else
                {
                    radioButton2.Checked = true;
                }
            }
            theConnection.Close();
        }

        public int numInStock()
        {
            return theStockNum;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                theStockNum = 0;
            }
            else
            {
                theStockNum = -1;
            }
            Close();
        }
    }
}

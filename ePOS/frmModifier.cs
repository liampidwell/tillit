using MySqlConnector;
using System;
using System.Windows.Forms;

namespace ePOS
{
    public partial class frmModifier : Form
    {
        private static MySqlConnection theConnection = new MySqlConnection(Extensions.theConnectionString);
        private string theMod = "";
        private string theReturnedMod = "";

        public frmModifier(string theModID)
        {
            InitializeComponent();
            theMod = theModID;
        }

        private void performButtonClick(object sender, EventArgs e)
        {
            Button theButt = sender as Button;
            theReturnedMod = theButt.Tag.ToString();
            Close();
        }

        public string getModifier()
        {
            return theReturnedMod;
        }

        private void frmModifier_Load(object sender, EventArgs e)
        {
            MySqlCommand theCommand = new MySqlCommand("SELECT * FROM oap_modgrp INNER JOIN oap_mods ON oap_mods.fk_modgrp_id = pk_modgrp_id WHERE modgrp_show_ontill = 1 AND pk_modgrp_id = " + theMod, theConnection);
            theConnection.Open();
            MySqlDataReader reader = theCommand.ExecuteReader();
            while (reader.Read())
            {
                Button theButton = Extensions.newButton(reader.GetString("modifier_kitchen_text"), "ffffff", "0021a6", reader.GetInt32("pk_modifier_id").ToString(), 11);
                if (reader.GetInt32("modifier_isinstock") == 0)
                {
                    theButton = Extensions.newButton(reader.GetString("modifier_kitchen_text"), "ffffff", "0021a6", reader.GetInt32("pk_modifier_id").ToString(), 11, false);
                }
                else
                {
                    theButton.Click += performButtonClick;
                }
                tableLayoutPanel2.Controls.Add(theButton);
                label1.Text = reader.GetString("modgrp_text");
            }
            theConnection.Close();
        }
    }
}

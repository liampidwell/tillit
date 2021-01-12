using MySqlConnector;
using System;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Windows.Forms;
using ThermalDotNet;

namespace ePOS
{
    public class Extensions
    {
        private static string cutThePaper = '\x1B' + "d" + ('\x05' + ('\x1D' + "V1"));
        private static string setTheChar = '\x1B' + "R" + '\x03';
        private static string horizontalLine = new string('-', 42);

        public static string inputBox(string theQuestion, bool enableKeys, bool enableNumbers, int theMinLength, int theMaxLength)
        {
            frmInputBox theForm = new frmInputBox(theQuestion, enableKeys, enableNumbers, theMinLength, theMaxLength);
            theForm.ShowDialog();

            return theForm.getTheResponse();
        }

        public static void messageBox(string theText)
        {
            frmMessageBox theForm = new frmMessageBox(theText);
            theForm.ShowDialog();
        }

        public static Button newButton(string theText, string theForeCol, string theBackCol, string theTag, int theFontSize)
        {
            Button theButton = new Button();
            theButton.Dock = DockStyle.Fill;
            theButton.FlatStyle = FlatStyle.Flat;
            theButton.FlatAppearance.BorderSize = 0;
            theButton.Text = theText;
            theButton.ForeColor = ColorTranslator.FromHtml("#" + theForeCol);
            theButton.BackColor = ColorTranslator.FromHtml("#" + theBackCol);
            theButton.Font = new Font(theButton.Font.FontFamily, theFontSize);
            theButton.Tag = theTag;

            theButton.FlatAppearance.MouseOverBackColor = theButton.BackColor;
            theButton.FlatAppearance.BorderColor = theButton.BackColor;

            return theButton;
        }

        public static Button newButton(string theText, string theForeCol, string theBackCol, string theTag, int theFontSize, Boolean isEnabled)
        {
            Button theButton = new Button();
            theButton.Dock = DockStyle.Fill;
            theButton.FlatStyle = FlatStyle.Flat;
            theButton.FlatAppearance.BorderSize = 0;
            theButton.Text = theText;
            theButton.ForeColor = ColorTranslator.FromHtml("#" + theForeCol);
            theButton.BackColor = ColorTranslator.FromHtml("#" + theBackCol);
            theButton.Font = new Font(theButton.Font.FontFamily, theFontSize);
            theButton.Tag = theTag;
            if (!isEnabled)
            {
                theButton.BackColor = ControlPaint.Dark(theButton.BackColor, 0.2F);
                theButton.ForeColor = ControlPaint.Dark(theButton.ForeColor, 0.2F);
                theButton.BackgroundImageLayout = ImageLayout.Zoom;
                theButton.BackgroundImage = Image.FromFile("no_entry.png");
                theButton.FlatAppearance.MouseDownBackColor = theButton.BackColor;
            }

            theButton.FlatAppearance.MouseOverBackColor = theButton.BackColor;
            theButton.FlatAppearance.BorderColor = theButton.BackColor;

            return theButton;
        }

        public static string theConnectionString = File.ReadAllText("theconnection.config");
        public static MySqlConnection theConnection = new MySqlConnection(theConnectionString);

        public static void printBill(DataGridViewRowCollection theLines, double theTotal)
        {
            SerialPort printerPort = new SerialPort("COM4", 19200);

            printerPort.Open();
            ThermalPrinter printer = new ThermalPrinter(printerPort);
            printer.WakeUp();
            printer.WriteToBuffer(setTheChar);
            printer.SetAlignCenter();
            printer.WriteLine("The John Francis Basset");
            printer.WriteLine("TR14 8JZ");
            printer.WriteLine("01209 721720");
            printer.SetAlignLeft();
            printer.LineFeed();
            printer.WriteLine("Till 4");
            string theLine = ("Liam").PadRight(25) + DateTime.Now.ToString("dd MMM yyyy HH:mm").PadLeft(17);
            printer.WriteLine(theLine);
            printer.LineFeed();
            theLine = ("Table: 999").PadRight(25) + ("Acc No: 7653").PadLeft(17);
            printer.WriteLine(theLine);
            printer.LineFeed();

            foreach (DataGridViewRow theRow in theLines)
            {
                if (theRow.Cells["is_mod"].Value.ToString() == "0")
                {
                    MySqlCommand theCommand = new MySqlCommand("SELECT * FROM oap_items WHERE pk_item_id = " + theRow.Cells["item_id"].Value.ToString(), theConnection);
                    theConnection.Open();
                    MySqlDataReader reader = theCommand.ExecuteReader();
                    while (reader.Read())
                    {
                        if (!reader.GetBoolean("item_act_as_text"))
                        {
                            printer.WriteLine(getItemLine(theRow.Cells["item_quant"].Value.ToString(), reader.GetString("item_kitchenname"), false, reader.GetDouble("item_price")));
                        }
                    }
                    theConnection.Close();
                }
                else
                {
                    MySqlCommand theCommand = new MySqlCommand("SELECT * FROM oap_mods WHERE pk_modifier_id = " + theRow.Cells["item_id"].Value.ToString(), theConnection);
                    theConnection.Open();
                    MySqlDataReader reader = theCommand.ExecuteReader();
                    while (reader.Read())
                    {
                        if (reader.GetDouble("modifier_price") != 0)
                        {
                            printer.WriteLine(getItemLine(theRow.Cells["item_quant"].Value.ToString(), reader.GetString("modifier_name"), false, reader.GetDouble("modifier_price")));
                        }
                    }
                    theConnection.Close();
                }
            }

            printer.WriteLine(horizontalLine);
            printer.WriteLine(("Total").PadRight(10) + formatPounds("£" + theTotal.ToString("0.00")).PadLeft(32));
            printer.WriteLine(("EFT").PadRight(10) + formatPounds("-£" + theTotal.ToString("0.00")).PadLeft(32), ThermalPrinter.PrintingStyle.Bold);
            printer.WriteLine("Receipt no. 75/0206");
            printer.WriteLine(horizontalLine);
            printer.SetAlignCenter();
            printer.WriteLine("Account Closed", 32);
            printer.WriteLine(horizontalLine);
            printer.LineFeed();
            printer.WriteLine("Payment Receipt", 32);
            printer.LineFeed();
            printer.SetAlignLeft();
            printer.WriteLine(("EFT").PadRight(10) + formatPounds("£" + theTotal.ToString("0.00")).PadLeft(11), 32);
            printer.WriteLine(("5% VAT FOOD included").PadRight(32) + formatPounds("£0.03").PadLeft(10));
            printer.WriteLine("Receipt no. 75/0206");
            printer.WriteLine("Vat no. 396 331 433");
            printer.LineFeed(2);
            printer.SetAlignCenter();
            printer.WriteLine("THANK YOU FOR VISITING");
            printer.WriteLine("THE JOHN FRANCIS BASSET");
            printer.WriteLine("COMMERCIAL STREET, CAMBORNE, TR14 8JZ");
            printer.WriteLine("PLEASE CALL AGAIN");
            printer.LineFeed(2);
            printer.WriteToBuffer(cutThePaper);
            printer.Sleep();
            printerPort.Close();
        }

        private static string formatPounds(string thePoundString)
        {
            return thePoundString.Replace('£', '\x23');
        }

        private static string getItemLine(string theQuantity, string theItem, bool isIndented, double thePrice)
        {
            string theIndent = "";
            if (isIndented)
            {
                theIndent = "    ";
            }
            string priceString = "";
            if (thePrice != 0)
            {
                priceString = thePrice.ToString("0.00");
            }
            return theQuantity.ToString().PadLeft(4) + "  " + (theIndent + theItem).PadRight(27) + "=" + priceString.PadLeft(8);
        }
    }
}

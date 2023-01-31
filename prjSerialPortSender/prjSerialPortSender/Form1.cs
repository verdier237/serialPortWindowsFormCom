using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using System.Xml;
namespace prjSerialPortSender
{
    public partial class Form1 : Form
    {
        private SerialPort ComPort;
        private string[] portsTab;  // list of available ports
        private String finalMessage;
        public XmlDocument bytecharFile = new XmlDocument(); // File for translation (FO) -> (00)
        public XmlDocument statusSaverFile = new XmlDocument(); // File for Status
        public struct ByteChar
        {
            public string code; // For letters 
            public byte b; // For byte
        }
        public class StatusSaver
        {

            public String element { get; set; }
            public bool state { get; set; }
            public StatusSaver(string element, bool state)
            {
                this.element = element;
                this.state = state;
            }

        }
        private List<ByteChar> listOfByteChar; // list of all element 
        private List<StatusSaver> listOfElement; // list of element with status

        public Form1()
        {
            InitializeComponent();
            listOfByteChar = new List<ByteChar>();
            listOfElement = new List<StatusSaver>();
            portsTab = SerialPort.GetPortNames();
            Array.Sort(portsTab);
            openComPort1(); // open com1
            fillByteCharTabFromXmlRessources(); // get char and file from my file
            fillStatusSaverTabFromXmlRessources();

            // click on buttons
            button1.Click += btnEventsClick;
            button2.Click += btnEventsClick;
            button3.Click += btnEventsClick;
            button4.Click += btnEventsClick;
            button5.Click += btnEventsClick;
            button6.Click += btnEventsClick;
            button7.Click += btnEventsClick;
            button8.Click += btnEventsClick;
            button9.Click += btnEventsClick;
            button10.Click += btnEventsClick;
        }

        private void fillStatusSaverTabFromXmlRessources()
        {
            string url = "./sources/logic.xml";
            try
            {

                statusSaverFile.Load(url);
            }
            catch (Exception)
            {
                MessageBox.Show("ERREUR A L'OUVERTURE D'UN FICHIER ");
                throw;
                this.Close();
            }
            XmlNodeList getStatus = statusSaverFile.GetElementsByTagName("data"); // get all data elt
            XmlNode lines = getStatus[0]; // get the first data elt
            for (int i = 0; i < lines.ChildNodes.Count; i++) // browse owr children
            {
                XmlNode currentLine = lines.ChildNodes[i]; // get one 
                listOfElement.Add(new StatusSaver(Convert.ToString(currentLine.Attributes[0].Value), Convert.ToBoolean(currentLine.Attributes[1].Value))); // u know
            }
            //string g = "";
            //foreach (var item in listOfElement)
            //{
            //    g += Convert.ToString(item.element) + "  " + Convert.ToString(item.state);
            //}
            //MessageBox.Show(g);
        }
        /// <summary>
        /// check out the convertion of byte to string 
        /// 
        /// byte[] msgTosend = System.Text.Encoding.UTF8.GetBytes(finalMessage);
        /// 
        /// ex.
        /// 
        /// 0 become 48 
        /// 
        /// check out my receiver project
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEventsClick(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            if (btn.BackColor == Color.Red)
            {
                btn.BackColor = Color.Silver;
            }
            else
            {
                btn.BackColor = Color.Red;
            }
            string txt = btn.Text;
            string converted = "";
            foreach (var item in listOfByteChar)
            {
                if (item.code == txt.Substring(0,txt.Length-1))
                {
                    converted = Convert.ToString(item.b) + txt[txt.Length - 1];
                }
            }
            foreach (StatusSaver item in listOfElement)
            {
                if (converted == item.element)
                {
                    if (item.state)
                    {
                        converted += "0";
                        item.state = false;
                    }
                    else
                    {
                        converted += "1";
                        item.state = true;

                    }
                }
            }
            finalMessage = converted;
            //MessageBox.Show(converted);
            // sending messages

            try
            {
                byte[] msgTosend = System.Text.Encoding.UTF8.GetBytes(finalMessage);
                //MessageBox.Show(Encoding.Default.GetString(msgTosend));
                this.ComPort.Write(msgTosend, 0, 3);
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void fillByteCharTabFromXmlRessources()
        {
            string url = "./sources/convert.xml";
            try
            {

                bytecharFile.Load(url);
            }
            catch (Exception)
            {
                MessageBox.Show("ERREUR A L'OUVERTURE D'UN FICHIER ");
                throw;
            }
            XmlNodeList getbytechar = bytecharFile.GetElementsByTagName("data");// get the data
            XmlNode lines = getbytechar[0];
            ByteChar elt = new ByteChar();
            for (int i = 0; i < lines.ChildNodes.Count; i++)
            {
                XmlNode currentLine = lines.ChildNodes[i];
                elt.code = Convert.ToString(currentLine.Attributes[0].Value);
                elt.b = Convert.ToByte(currentLine.Attributes[1].Value);
                listOfByteChar.Add(elt);
            }
            //foreach (var item in listOfByteChar)
            //{
            //    MessageBox.Show(item.code);
            //}
            
        }

        private void openComPort1()
        {
            this.ComPort = new SerialPort(portsTab[0], 9600, Parity.None, 8, StopBits.One);
            if (!this.ComPort.IsOpen)
            {
                this.ComPort.Open();
                btnIsOpen.BackColor = Color.Green;
                lblInfoPort.Text = "le port " + portsTab[0] + " est maintenant ouvert";
            }
        }
        private void btnIsOpen_Click_1(object sender, EventArgs e)
        {
            if (!this.ComPort.IsOpen)
            {
                this.ComPort.Open();
                btnIsOpen.BackColor = Color.Green;
                lblInfoPort.Text = "le port " + portsTab[0] + " est maintenant ouvert";
            }
            else
            {
                this.ComPort.Close();
                lblInfoPort.Text = "le port " + portsTab[0] + " est maintenant ferme";
                btnIsOpen.BackColor = Color.Red;
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}

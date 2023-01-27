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
        public XmlDocument bytecharFile = new XmlDocument(); // File for translation (FO) -> (00)
        public struct ByteChar
        {
            public string code; // For letters 
            public byte b; // For byte
        }
        private List<ByteChar> listOfByteChar; // list of all element 

        public Form1()
        {
            InitializeComponent();
            listOfByteChar = new List<ByteChar>();
            portsTab = SerialPort.GetPortNames();
            Array.Sort(portsTab);
            openComPort1();
            fillByteCharTabFromXmlRessources();
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
            MessageBox.Show(listOfByteChar.ToString());
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
        private void btnIsOpen_Click(object sender, EventArgs e)
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

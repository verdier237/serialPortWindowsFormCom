using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.IO.Ports;
using System.Xml;
namespace prjSerialPortReceiver
{
    public partial class Form1 : Form
    {
        private SerialPort ComPort;
        private string[] portsTab;  // list of available ports

        private struct evenement
        {
            public string code;
            public string meaning;
        }
        private struct history
        {
            public String date;
            public String meaning;
        }
        /// <summary>
        /// check out the support file to understand my logic (./sources/support.xml)
        /// support file contains all my events 
        /// instead of use a db 
        /// </summary>
        public XmlDocument eventFile = new XmlDocument(); // File for all events
        public XmlDocument historyFile = new XmlDocument(); // My history

        private List<history> listHistory; //   History
        private List<evenement> listOfEvent; // list of all event 
        public Form1()
        {
            InitializeComponent();
            portsTab = SerialPort.GetPortNames();
            listOfEvent = new List<evenement>();
            listHistory = new List<history>();
            Array.Sort(portsTab);
            openComPort1();
            fillEventList(); // fill my list of event
            initializeHistory(); // --|--
            FormClosing += Form1_FormClosing;

        }
        /// <summary>
        /// if my form closed , replace the history by the new history according to my list<history>()
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            refreshHistory("999"); // code for closing message
            using (XmlWriter writer = XmlWriter.Create("./sources/history.xml"))
                {         
                    writer.WriteStartElement("data");
                foreach (var item in listHistory)
                {
                    writer.WriteStartElement("hi");
                    writer.WriteAttributeString("date", item.date);
                    writer.WriteAttributeString("meaning", item.meaning);
                    writer.WriteEndElement();
                }

                    writer.WriteEndDocument();
                    writer.Flush();
                    
                }
            
        }
        /// <summary>
        /// at the beginning 
        /// if history exists , load all everything 
        /// if it doesn't create history 
        /// the code between try ------------------------ foreach(){} can be a function
        /// </summary>
        private void initializeHistory()
        {
            if (!File.Exists("./sources/history.xml"))
            {
                using (XmlWriter writer = XmlWriter.Create("./sources/history.xml"))
                {         
                    writer.WriteStartElement("data");
                    writer.WriteStartElement("hi");
                    writer.WriteAttributeString("date", DateTime.Now.ToString());
                    writer.WriteAttributeString("meaning", "premiere initialisation du fichier historique Bienvenue");
                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                    writer.Flush();
                    
                }
                MessageBox.Show("Premiere utilisation Bienvenue");
                string url = "./sources/history.xml";
                try
                {
                    historyFile.Load(url);
                }
                catch (Exception)
                {
                    MessageBox.Show("ERREUR A L'OUVERTURE D'UN FICHIER ");
                    throw;
                    this.Close();
                }
                XmlNodeList getEvent = historyFile.GetElementsByTagName("data"); // get all data elt
                XmlNode lines = getEvent[0]; // get the first data elt
                history hi = new history();
                for (int i = 0; i < lines.ChildNodes.Count; i++) // browse owr children
                {
                    XmlNode currentLine = lines.ChildNodes[i]; // get one 
                    hi.date = Convert.ToString(currentLine.Attributes[0].Value);
                    hi.meaning = Convert.ToString(currentLine.Attributes[1].Value);
                    listHistory.Add(hi);
                }
                foreach (var item in listHistory)
                {

                    lbHistory.Items.Add(item.date + ":    " + item.meaning);

                }
            }
            else
            {
                string url = "./sources/history.xml";
                try
                {
                    historyFile.Load(url);
                }
                catch (Exception)
                {
                    MessageBox.Show("ERREUR A L'OUVERTURE D'UN FICHIER ");
                    throw;
                    this.Close();
                }
                XmlNodeList getEvent = historyFile.GetElementsByTagName("data"); // get all data elt
                XmlNode lines = getEvent[0]; // get the first data elt
                history hi = new history();
                for (int i = 0; i < lines.ChildNodes.Count; i++) // browse owr children
                {
                    XmlNode currentLine = lines.ChildNodes[i]; // get one 
                    hi.date = Convert.ToString(currentLine.Attributes[0].Value);
                    hi.meaning = Convert.ToString(currentLine.Attributes[1].Value);
                    listHistory.Add(hi);
                }
                foreach (var item in listHistory)
                {
                    
                    lbHistory.Items.Add(item.date + ":    " + item.meaning);

                }
            }
        }
        /// <summary>
        /// check out the support file to understand my logic (./sources/support.xml) 
        /// </summary>
        private void fillEventList()
        {
            string url = "./sources/support.xml";
            try
            {

                eventFile.Load(url);
            }
            catch (Exception)
            {
                MessageBox.Show("ERREUR A L'OUVERTURE D'UN FICHIER ");
                throw;
                this.Close();
            }
            XmlNodeList getEvent = eventFile.GetElementsByTagName("data"); // get all data elt
            XmlNode lines = getEvent[0]; // get the first data elt
            evenement evt = new evenement();
            for (int i = 0; i < lines.ChildNodes.Count; i++) // browse owr children
            {
                XmlNode currentLine = lines.ChildNodes[i]; // get one 
                evt.code = Convert.ToString(currentLine.Attributes[0].Value);
                evt.meaning = Convert.ToString(currentLine.Attributes[1].Value);
                listOfEvent.Add(evt);
            }
        }
        /// <summary>
        /// according to my support file 
        /// get a string code (001) , browse all my support file to find the meaning 
        /// then store the date and the meaning in history struct
        /// 
        /// </summary>
        /// <param name="code"></param>
        private void refreshHistory(string code)
        {
            
            foreach (var item in listOfEvent)
            {
                if (item.code == code)
                {
                    history hi = new history();
                    hi.date = DateTime.Now.ToString();
                    hi.meaning = item.meaning;
                    listHistory.Add(hi);
                    refreshList();
                }
            }
        }
        /// <summary>
        /// refresh actual list on a good thread
        /// </summary>
        private void refreshList()
        {
            string text;  
            lbHistory.Invoke((MethodInvoker)(() => lbHistory.Items.Clear()));
            foreach (var item in listHistory)
            {
                text = item.date+ ":    " + item.meaning;
                lbHistory.Invoke((MethodInvoker)(()=> lbHistory.Items.Add(text)));

            }
        }
        /// <summary>
        /// open comport
        /// </summary>
        private void openComPort1()
        {
            this.ComPort = new SerialPort(portsTab[1], 9600, Parity.None, 8, StopBits.One);
            if (!this.ComPort.IsOpen)
            {
                this.ComPort.ReceivedBytesThreshold = 1;
                this.ComPort.DataReceived += ComPort_DataReceived;
                this.ComPort.Open();
                btnIsOpen.BackColor = Color.Green;
                lblInfoPort.Text = "le port " + portsTab[1] + " est maintenant ouvert";
            }
        }
        /// <summary>
        ///  not recommended
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        private string changeChar(string c) // there is many reusable option to convert this i know but i'm bored
        {
            if (c == "0")
            {
                return "F";
            }
            else if (c == "1")
            {
                return "P";
            }
            else if (c == "2")
            {
                return "DM";
            }
            else if (c == "3")
            {
                return "DI";
            }
            else
            {
                return c;
            }
        }
        /// <summary>
        /// check out :
        /// List<Button> listButton = this.Controls.OfType<Button>().ToList();
        /// 
        /// in this line i get all the buttons controls in my forms 
        /// then i find the right button by his text argument
        /// 
        /// Controls.Find() is also good but i didn't wan to use name
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            byte[] msg = new byte[3];
            this.ComPort.Read(msg, 0, 3);
            string decoded = Encoding.Default.GetString(msg);
            //MessageBox.Show(decoded);
            refreshHistory(decoded);
            string decodedConverted = changeChar(decoded[0].ToString()) + decoded[1];
            //MessageBox.Show(decodedConverted);
            List<Button> listButton = this.Controls.OfType<Button>().ToList();  
            for (int i = 0; i < listButton.Count; i++)
            {
                    if (listButton[i].Text == decodedConverted)
                    {
                        if (listButton[i].BackColor == Color.Silver)
                        {
                        listButton[i].BackColor = Color.Red;
                        }
                        else
                        {
                        listButton[i].BackColor = Color.Silver;
                        }

                    }

            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }
        //private void btnIsOpen_Click(object sender, EventArgs e)
        //{
        //    if (!this.ComPort.IsOpen)
        //    {
        //        this.ComPort.Open();
        //        btnIsOpen.BackColor = Color.Green;
        //        lblInfoPort.Text = "le port " + portsTab[0] + " est maintenant ouvert";
        //    }
        //    else
        //    {
        //        this.ComPort.Close();
        //        lblInfoPort.Text= "le port " + portsTab[0] + " est maintenant ferme";
        //        btnIsOpen.BackColor = Color.Red;
        //    }
        //}
    }
}

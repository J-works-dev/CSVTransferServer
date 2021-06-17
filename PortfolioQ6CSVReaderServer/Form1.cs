using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PortfolioQ6CSVReaderServer
{
    public partial class Form1 : Form
    {
        private Pipe pipeServer = new Pipe();
        private bool isCSV = false;
        public Form1()
        {
            InitializeComponent();
            pipeServer.MessageReceived += pipeServer_MessageReceived;
            pipeServer.ClientDisconnected += pipeServer_ClientDisconnected;

            serverStart();
        }

        private void pipeServer_ClientDisconnected()
        {
            Invoke(new Pipe.ClientDisconnectedHandler(ClientDisconnected));
        }

        private void pipeServer_MessageReceived(byte[] message)
        {
            Invoke(new Pipe.MessageReceivedHandler(DisplayMessageReceived),
                new object[] { message });
        }

        void ClientDisconnected()
        {
            statusStrip.Text = "Client Disconnected!";
        }

        void DisplayMessageReceived(byte[] message)
        {
            ASCIIEncoding encoder = new ASCIIEncoding();
            //byte[] msgBuffer;
            // bool isCSV = false;
            string str = encoder.GetString(message, 0, message.Length);

            textBoxMessages.Text += "Client: " + str + "\r\n";

            //if (!isCSV)
            //{
            //    string[] clientInfo = str.Split(',');
            //    string id = clientInfo[0];
            //    string pw = clientInfo[1];

            //    foreach (User user in users)
            //    {
            //        if (user.Id.Equals(id))
            //        {
            //            isUser = true;
            //            if (user.Verify(pw))
            //            {
            //                msgBuffer = encoder.GetBytes("logged in");
            //                pipeServer.SendMessage(msgBuffer);
            //                isLogin = true;
            //            }
            //            else
            //            {
            //                msgBuffer = encoder.GetBytes("Wrong Password");
            //                pipeServer.SendMessage(msgBuffer);
            //            }
            //        }
            //    }

            //    if (!isUser) // [users.Count - 1]
            //    {
            //        msgBuffer = encoder.GetBytes("No User found");
            //        pipeServer.SendMessage(msgBuffer);
            //    }
            //}
            //else
            //{
            //    if (str == "Disconnect")
            //    {
            //        isLogin = false;
            //        isUser = false;
            //    }
            //    textBoxReceived.Text += "Client: " + str + "\r\n";
            //}
        }

        private void writeCSV()
        {
            dataGridView.ColumnCount = 5;
            dataGridView.Columns[0].Name = "Release Date";
            dataGridView.Columns[1].Name = "Track";
            dataGridView.Columns[2].Name = "Title";
            dataGridView.Columns[3].Name = "Artist";
            dataGridView.Columns[4].Name = "Album";

            string[] row0 = { "11/22/1968", "29", "Revolution 9",
            "Beatles", "The Beatles [White Album]" };
            string[] row1 = { "1960", "6", "Fools Rush In",
            "Frank Sinatra", "Nice 'N' Easy" };
            string[] row2 = { "11/11/1971", "1", "One of These Days",
            "Pink Floyd", "Meddle" };
            string[] row3 = { "1988", "7", "Where Is My Mind?",
            "Pixies", "Surfer Rosa" };
            string[] row4 = { "5/1981", "9", "Can't Find My Mind",
            "Cramps", "Psychedelic Jungle" };
            string[] row5 = { "6/10/2003", "13",
            "Scatterbrain. (As Dead As Leaves.)",
            "Radiohead", "Hail to the Thief" };
            string[] row6 = { "6/30/1992", "3", "Dress", "P J Harvey", "Dry" };

            dataGridView.Rows.Add(row0);
            dataGridView.Rows.Add(row1);
            dataGridView.Rows.Add(row2);
            dataGridView.Rows.Add(row3);
            dataGridView.Rows.Add(row4);
            dataGridView.Rows.Add(row5);
            dataGridView.Rows.Add(row6);

            dataGridView.Columns[0].DisplayIndex = 3;
            dataGridView.Columns[1].DisplayIndex = 4;
            dataGridView.Columns[2].DisplayIndex = 0;
            dataGridView.Columns[3].DisplayIndex = 1;
            dataGridView.Columns[4].DisplayIndex = 2;
        }

        private void buttonSendM_Click(object sender, EventArgs e)
        {
            ASCIIEncoding encoder = new ASCIIEncoding();
            byte[] msgBuffer = encoder.GetBytes(textBoxSend.Text);

            pipeServer.SendMessage(msgBuffer);
            textBoxSend.Clear();
        }

        private void buttonSendCSV_Click(object sender, EventArgs e)
        {
            writeCSV();
        }

        private void buttonSaveCSV_Click(object sender, EventArgs e)
        {

        }

        private void serverStart()
        {
            if (!pipeServer.Running)
            {
                pipeServer.Start(textBoxPipeName.Text);
                statusLabel.Text = "Admin Logged in";
            }
            else
            {
                statusLabel.Text = "Server already running.";
            }
        }

        private void textBoxSend_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                buttonSendM_Click(this, new EventArgs());
            }
        }
    }
}

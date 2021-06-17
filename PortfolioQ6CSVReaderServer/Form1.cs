using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using CsvHelper;
using CsvHelper.Configuration;

namespace PortfolioQ6CSVReaderServer
{
    public partial class Form1 : Form
    {
        private Pipe pipeServer = new Pipe();
        private bool isGetCSV = false;
        private bool isWant = false;
        private bool isWantToGet = false;
        Thread CSVThread;
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

            if (str.Equals("kajsdfaeeqr8f47hfqn8va48idf8aifnv8"))
            {
                DialogResult dialogResult = MessageBox.Show("Do you want to GET CSV File from Server?", "CSV Reciever", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    writeCSV();
                }
                else if (dialogResult == DialogResult.No)
                {
                    trashCSV();
                }
            }
            textBoxMessages.Text += "Client: " + str + "\r\n";
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

        private void trashCSV()
        {

        }

        private void buttonSendM_Click(object sender, EventArgs e)
        {
            ASCIIEncoding encoder = new ASCIIEncoding();
            byte[] msgBuffer = encoder.GetBytes(textBoxSend.Text);

            pipeServer.SendMessage(msgBuffer);
            textBoxSend.Clear();
        }

        private void sendCSV(string path)
        {
            ASCIIEncoding encoder = new ASCIIEncoding();
            
            while (true)
            {
                if (isWant)
                {
                    byte[] msgBuffer = encoder.GetBytes("CSV Writing Start Now!@#$%^&");
                    pipeServer.SendMessage(msgBuffer);

                    var streamReader = new StreamReader(path);
                    var csvReader = new CsvReader(streamReader, CultureInfo.InvariantCulture);

                    string value;
                    Console.WriteLine(csvReader.Read());
                    Console.WriteLine(csvReader.ReadHeader());
                    //Console.WriteLine(csvReader.ColumnCount);
                    string[] headers = csvReader.HeaderRecord;
                    int colCount = headers.Length;
                    Console.WriteLine("[{0}]", string.Join(", ", headers) + colCount);
                    while (csvReader.Read())
                    {
                        for (int i = 0; csvReader.TryGetField<string>(i, out value); i++)
                        {
                            Console.Write($"{value} ");
                        }

                        Console.WriteLine();
                    }

                    msgBuffer = encoder.GetBytes("CSV Writing End Now!@#$%^&");
                    pipeServer.SendMessage(msgBuffer);
                    isGetCSV = false;
                }
            }
        }

        private void buttonSendCSV_Click(object sender, EventArgs e)
        {
            string path = getPathfromDialog();

            if (path != null)
            {
                ASCIIEncoding encoder = new ASCIIEncoding();
                byte[] msgBuffer = encoder.GetBytes("kajsdfaeeqr8f47hfqn8va48idf8aifnv8");
                pipeServer.SendMessage(msgBuffer);
                isGetCSV = true;
                CSVThread = new Thread(new ThreadStart(sendCSV(path)));
                CSVThread.Start();
            }
        }

        private string getPathfromDialog()
        {
            openFileDialog.InitialDirectory = @"D:\";
            openFileDialog.Title = "Browse Text Files";
            openFileDialog.CheckFileExists = true;
            openFileDialog.CheckPathExists = true;
            openFileDialog.DefaultExt = "csv";
            openFileDialog.RestoreDirectory = true;
            openFileDialog.ReadOnlyChecked = true;
            openFileDialog.ShowReadOnly = true;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                return openFileDialog.FileName;
            }
            else
            {
                return null;
            }
        }

        private void buttonSaveCSV_Click(object sender, EventArgs e)
        {

        }

        private void serverStart()
        {
            if (!pipeServer.Running)
            {
                pipeServer.Start(textBoxPipeName.Text);
                statusLabel.Text = "Server Started.";
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

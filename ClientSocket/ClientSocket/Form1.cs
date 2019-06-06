using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace ClientSocket
{
    public partial class Form1 : Form
    {
        string serverIP = "localhost";
        int port = 8080;
        public Form1()
        {
            InitializeComponent();

            
        }

        private void submit_Click(object sender, EventArgs e)
        {
            TcpClient client = new TcpClient(serverIP, port);

            int byteCount = Encoding.ASCII.GetByteCount(message.Text);

            byte[] sendData = new byte[byteCount+1];

            // store the message in sendData
            sendData = Encoding.ASCII.GetBytes(message.Text + ";");

            // we already have a TCP client all setup. So we use its network stream
            NetworkStream stream = client.GetStream();

            //send the entire message, starting from 0 to sendData.Length
            stream.Write(sendData, 0, sendData.Length);

            stream.Close();
            client.Close();
        }
    }
}

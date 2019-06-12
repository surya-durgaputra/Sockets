using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SocketAsync;

namespace AsyncSocketServer
{
    public partial class Form1 : Form
    {
        SocketServer mServer;
        public Form1()
        {
            InitializeComponent();
            mServer = new SocketServer();
            // we will also assign the subscriber method to the publisher hook here
            mServer.ClientConnectedEvent += HandleClientConnected;
            mServer.ServerTextReceivedEvent += HandleServerTextReceived;
        }

        private void btnAcceptIncomingAsync_Click(object sender, EventArgs e)
        {
            mServer.StartListeningForIncomingConnection();
        }

        private void btnSendAll_Click(object sender, EventArgs e)
        {
            mServer.SendToAll(txtMessage.Text.Trim());
        }

        private void btnStopServer_Click(object sender, EventArgs e)
        {
            mServer.StopServer();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            mServer.StopServer();
        }

        // subscriber side of Publisher Subscriber model
        // event handler method will be called when a new client connects
        // utilization of this method is totally upto our form application (subscriber)
        // we need to define a method whose first parameter is an object and second parameter is ClientConnectedEventArgs so that it
        // conforms to the event defined by the SocketAsync library
        void HandleClientConnected(object sender, ClientConnectedEventArgs e)
        {
            txtConsole.AppendText(string.Format("{0} - New client connected: {1}{2}",
                DateTime.Now, e.NewClient, Environment.NewLine));
        }
        void HandleServerTextReceived(object sender, TextReceivedEventArgs e)
        {
            txtConsole.AppendText(string.Format("{0} - Client: {1} Message:{2}{3}",
                DateTime.Now, e.Client, e.Text, Environment.NewLine));
        }
    }
}

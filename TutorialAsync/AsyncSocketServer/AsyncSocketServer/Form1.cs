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
        }

        private void btnAcceptIncomingAsync_Click(object sender, EventArgs e)
        {
            mServer.StartListeningForIncomingConnection();
        }
    }
}

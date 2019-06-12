using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketAsync
{
    // these args will contain the IPAddress : PortNumber of the newly connected client
    public class ClientConnectedEventArgs : EventArgs
    {
        public string NewClient { get; set; }
        public ClientConnectedEventArgs(string _newClient)
        {
            NewClient = _newClient;
        }
    }
    // these args will contain the text sent by the connected client or server
    public class TextReceivedEventArgs : EventArgs
    {
        public string Client { get; set; }
        public string Text { get; set; }
        public TextReceivedEventArgs(string _text, string _client)
        {
            Client = _client;
            Text = _text;
        }
    }
}

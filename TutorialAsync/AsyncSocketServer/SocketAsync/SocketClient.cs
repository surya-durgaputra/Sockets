using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SocketAsync
{
    public class SocketClient
    {
        IPAddress mServerIPAddress;
        int mServerPort;
        // we will use the TcpClient helper class to create the client
        TcpClient mClient;

        public SocketClient()
        {
            mClient = null;
            mServerPort = -1;
            mServerIPAddress = null;
        }

        // ClientTextReceivedEvent implementation
        public EventHandler<TextReceivedEventArgs> ClientTextReceivedEvent;
        protected virtual void RaiseClientTextReceivedEvent(TextReceivedEventArgs e)
        {
            // within this method we are creating a temporary copy of the EventHandler object to avoid any race conditions
            EventHandler<TextReceivedEventArgs> custom_event = ClientTextReceivedEvent;
            if (custom_event != null)
            {
                // here Object sender will point to custom_event which is an instance of ClientConnectedEvent event
                custom_event(this, e);
            }
        }

        //hostname to IP address mapping using System.Net.Dns
        public static IPAddress ResolveHostNameToIPAddress(string strHostName)
        {
            // since we will be using GetHostAddresses that returns an array of all IP addresses available on the specified
            // this means that all IPv4 and IPv6 addresses will be there.
            // We will go with the first available IPv4 address because our server accepts requests on any IP address available on the host.
            IPAddress[] retAddr = null;

            // if there is not IP address mapped to the supplied hostname, an exception will occer. Lets handle it
            try
            {
                retAddr = Dns.GetHostAddresses(strHostName);

                // now lets iterate through the array of returned IP addresses to get the first available IPv4 address
                foreach(IPAddress addr in retAddr)
                {
                    if(addr.AddressFamily == AddressFamily.InterNetwork)
                    {
                        return addr;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            // when the hostname has no IP address mapped to it, we will return null
            // we will use empty string in our console application when null is returned
            return null;
        }

        //getter and setter for instance variables
        public int ServerPort
        {
            get
            {
                return mServerPort;
            }
        }
        public bool SetServerIPAddress(string _IPAddressServer)
        {
            IPAddress ipaddr = null;
            if(!IPAddress.TryParse(_IPAddressServer, out ipaddr))
            {
                Console.WriteLine("Invalid server IP Address supplied.");
                return false;
            }
            mServerIPAddress = ipaddr;
            return true;
        }
        public bool SetPortNumber(string _ServerPort)
        {
            int portNumber = 0;

            if (!int.TryParse(_ServerPort.Trim(), out portNumber))
            {
                Console.WriteLine("Invalid port number supplied, return.");
                return false;
            }

            if (portNumber <= 0 || portNumber > 65535)
            {
                Console.WriteLine("Port number must be between 0 and 65535");
                return false;
            }

            mServerPort = portNumber;
            return true;
        }

        public void CloseAndDisconnect()
        {
            if(mClient != null)
            {
                if (mClient.Connected)
                {
                    mClient.Close();
                }
            }
        }

        public async Task SendToServer(string strInputUser)
        {
            // dont do anything is user supplied null or empty parameter
            if (string.IsNullOrEmpty(strInputUser))
            {
                Console.WriteLine("Empty string supplied to send.");
                return;
            }
            
            if (mClient != null)
            {
                if (mClient.Connected)
                {
                    // in order to send the data, we will need to create an instance of 
                    // StreamWriter and set its autoflush property to true
                    StreamWriter clientStreamWriter = new StreamWriter(mClient.GetStream());
                    clientStreamWriter.AutoFlush = true;

                    await clientStreamWriter.WriteAsync(strInputUser);
                    Console.WriteLine("Data sent...");
                }
            }
        }

        public async Task ConnectToServer()
        {
            if(mClient == null)
            {
                mClient = new TcpClient();
            }
            try
            {
                await mClient.ConnectAsync(mServerIPAddress, mServerPort);
                Console.WriteLine(string.Format("Connected to server IP:Port {0}:{1}",
                    mServerIPAddress, mServerPort));
                //lets read data from the server
                ReadDataAsync(mClient);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                throw;
            }
        }

        private async Task ReadDataAsync(TcpClient mClient)
        {
            try
            {
                // Every TCP-IP socket or TcpClient has got a network Stream attached to it, just like the stream available in case of console-IO or File-IO.
                // we use the streamreader object to read data from the network stream which is associated with the TCP Client that is passed into
                // this method as a parameter.
                StreamReader clientStreamReader = new StreamReader(mClient.GetStream());
                char[] buff = new char[64];
                int readByteCount = 0;

                // inside this while loop, we will read data from the server
                while (true)
                {
                    readByteCount = await clientStreamReader.ReadAsync(buff, 0, buff.Length);
                    // if readByteCount is ever less than zero, it means that the connection to the server is broken
                    if (readByteCount <= 0)
                    {
                        Console.WriteLine("Disconnected from server");
                        mClient.Close();
                        break;
                    }

                    string receivedText = new string(buff);
                    Console.WriteLine(string.Format("Received bytes: {0} - Message: {1}", readByteCount, receivedText));
                    //lets clear the array buffer before reading again
                    Array.Clear(buff, 0, buff.Length);

                    //raise the text received event
                    RaiseClientTextReceivedEvent(new TextReceivedEventArgs(
                        receivedText,
                        mClient.Client.RemoteEndPoint.ToString()
                        ));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                throw;
            }
        }
    }
}

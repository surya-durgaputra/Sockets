using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace LahoreSocketAsync
{
    public class LahoreSocketClient
    {
        IPAddress mServerIPAddress;
        public IPAddress ServerIPAddress {
            get
            {
                return mServerIPAddress;
            }
        }
        public bool SetServerIPAddress(string _IPAddressServer)
        {
            IPAddress ipaddr = null;
            if (!IPAddress.TryParse(_IPAddressServer, out ipaddr))
            {
                Console.WriteLine("Invalid server IP supplied.");
                return false;
            }

            mServerIPAddress = ipaddr;
            return true; 
        }

        int mServerPort;
        public int ServerPort
        {
            get
            {
                return mServerPort;
            }
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
                Console.WriteLine("Port number must be between 0 and 65535.");
                return false;
            }

            mServerPort = portNumber;

            return true; 
        }

        TcpClient mClient;

        public async void SendToServer(string userInput)
        {
            if (string.IsNullOrEmpty(userInput))
            {
                Console.WriteLine("Empty string supplied to send.");
                return;
            }
            if(mClient != null)
            {
                if(mClient.Connected)
                {
                    StreamWriter clientStreamWriter = new StreamWriter(mClient.GetStream());
                    clientStreamWriter.AutoFlush = true;
                    clientStreamWriter.WriteAsync(userInput);
                    Console.WriteLine("Write data to server done. ");
                }
            }
        }

        public async Task ConnectToServer()
        {
            if (mClient == null)
            {
                mClient = new TcpClient();
            }
            try
            {
                await mClient.ConnectAsync(mServerIPAddress, mServerPort);
                Console.WriteLine(string.Format("Connected to server IP/Port: {0} / {1}", mServerIPAddress, mServerPort));

                StreamReader clientStreamReader = new StreamReader(mClient.GetStream());
                
                char[] buff = new char[64];
                int readByteCount = 0;

                while(true)
                {
                    readByteCount = await clientStreamReader.ReadAsync(buff, 0, buff.Length);
                    
                    if(readByteCount <= 0)
                    {
                        Console.WriteLine("Disconnected from server.");
                        break;
                    }

                    Console.WriteLine(string.Format("Received bytes: {0} - Message: {1}", readByteCount, new string(buff)));

                    Array.Clear(buff, 0, buff.Length);
                }
            }
            catch(Exception excp)
            {
                Console.WriteLine(excp.ToString());
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SocketClientStarter
{
    class Program
    {
        static void Main(string[] args)
        {
            Socket client = null;
            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress ipaddr = null;

            try
            {
                Console.WriteLine("Please type a valid server IP Address and Press Enter: ");
                string strIPAddress = Console.ReadLine();
                Console.WriteLine("Please type a valid Port Number 0-65535 and Press Enter: ");
                string strPortInput = Console.ReadLine();
                int nPortInput = 0;

                //convert the string to proper IPAddress type
                if(!IPAddress.TryParse(strIPAddress, out ipaddr))
                {
                    Console.WriteLine("Invalid IP Address supplied");
                    return;
                }
                if (!int.TryParse(strPortInput, out nPortInput))
                {
                    Console.WriteLine("Invalid port number supplied");
                    return;
                }
                if(nPortInput <=0 || nPortInput > 65535)
                {
                    Console.WriteLine("Port number must be between 0 and 65535");
                    return;
                }
                System.Console.WriteLine(string.Format("IPAddress: {0} - Port: {1}", ipaddr.ToString(), nPortInput));

                // and now we make a connection to the server process
                // NOTE: connect is a blocking method. The program is going to stop here until either the connection operation is successful
                // or there is a timeout (like if server is not found or the network has failed)
                client.Connect(ipaddr, nPortInput);
                Console.WriteLine("Connected to the server. Type text and press enter to send it to the server. Type <EXIT> to close.");
                string inputCommand = string.Empty;
                while (true)
                {
                    inputCommand = Console.ReadLine();
                    if (inputCommand.Equals("<EXIT>"))
                    {
                        break;
                    }

                    // the pre-requisite to sending any data(inputCommand in our case) to the server is to convert inputCommand to a byte array
                    // Encoding.ASCII.GetBytes() is used to convert to byte array format
                    // for clarity sake, we will store the encoded result in an array.
                    byte[] buffSend = Encoding.ASCII.GetBytes(inputCommand);
                    client.Send(buffSend);

                    // since our server has been implemented so that it echoes back the data sent to it by the client,
                    // we will make a receive method call to receive the echoed data from the server
                    // also the receive method call is going to need a buffer to store the data
                    // we could have re-used buffSend, but to keep everything clear, we will create a new variable
                    byte[] buffReceived = new byte[128];
                    // Receive methods returns the number of bytes it received
                    int nRecv = client.Receive(buffReceived);

                    // again the received data will need to be converted to string format for us to see
                    Console.WriteLine("Data received: {0}", Encoding.ASCII.GetString(buffReceived, 0, nRecv));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            // we will add a finally block where we will shut down the socket and dispose it
            finally
            {
                // we add this if block. Without this, if we specified wrong server credentials by mistake,
                // it would try to shutdown the client socket that does not exist and thus get an exception
                // (since finally code block always gets executed)
                if (client != null) { 
                    // only call shutdown on a socket that is connected
                    if (client.Connected)
                    {
                        // shutdown method call disables both send and receive calls on the socket
                        client.Shutdown(SocketShutdown.Both);
                    }
                    
                    // now close the socket
                    client.Close();
                    client.Dispose();
                }
            }
            Console.WriteLine("Press a key to exit...");
            Console.ReadKey();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace SocketsServerStarter
{
    class Program
    {
        static void Main(string[] args)
        {
            Socket listenerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            // IPAddress.Any means that we will be listening to any IPAddress available on this PC
            // Note that a PC can have multiple IP address
            // having IPAddress.Any means that we can listen on loopback IP address (127.0.0.1), also called localhost
            // or alternately we can also use the IP Address assigned to this machine as returned by ipconfig 
            IPAddress ipaddr = IPAddress.Any;

            //now lets also define an IP Endpoint
            IPEndPoint ipep = new IPEndPoint(ipaddr, 23000);

            //lets bind this endpoint to the socket
            listenerSocket.Bind(ipep);

            //note: at this point, the socket is still not listening
            // following are the steps to make a socket listen

            // lets call listen
            //(5) in parens means that max 5 clients can wait for a connection
            // while the socket is busy serving another connection
            listenerSocket.Listen(5);
            Console.WriteLine("About to accept incoming connection");
            // finally, to actually make it listen, we will need to call the Accept method on the listener socket
            // Note: Accept() is a synchronous and blocking operation. Our program will not move forward until this
            // is finished (or returned).
            // Accept() will keep waiting for incoming connection and will not return until a client has made a connection.
            // In other words, never to be used in production. Only for understanding concepts.
            // Once, Accept() has returned, we will get an object of socket type.
            // This object of Socket type will represent the client that just got connected with the Accept method call.
            Socket client = listenerSocket.Accept();
            Console.WriteLine("Client connected. " + client.ToString() + " - Client IP End Point: " + client.RemoteEndPoint.ToString());
            // we will now use this return value to recieve data from the client
            // for receiving data from the client, we create a byte array. This array is passed by reference to the client.Receive
            // call and is filled there. The client.Receive call itself return the number of bytes received.
            byte[] buff = new byte[128];
            int numberOfReceivedBytes = client.Receive(buff);
            Console.WriteLine("Number of received bytes: " + numberOfReceivedBytes);
            // note that the data received from the client in the byte format (inside the buff), cannot be directly printed
            // it needs to be decoded
            Console.WriteLine("Data returned by client is: " + buff);
            // as you can see that the above WriteLine just returns "byte[]" and not the actual data that was sent by the client
            // we will now convert the byte array into a human readable ASCII string 
            string receivedText = Encoding.ASCII.GetString(buff, 0, numberOfReceivedBytes);
            Console.WriteLine("Data returned by client is: " + receivedText);
            Console.Read();
        }
    }
}

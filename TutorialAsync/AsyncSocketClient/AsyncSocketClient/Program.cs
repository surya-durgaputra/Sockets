using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SocketAsync;

namespace AsyncSocketClient
{
    class Program
    {
        static void Main(string[] args)
        {
            SocketClient client = new SocketClient();
            Console.WriteLine("*** Welcome to SOcket Client Starter Example ***");
            Console.WriteLine("Please type a Valid Server IP Address and Press Enter: ");

            string strIPAddress = Console.ReadLine();

            Console.WriteLine("Please supply a valid Port Number 0 - 65535 and Press Enter: ");
            string strPortInput = Console.ReadLine();

            if(!client.SetServerIPAddress(strIPAddress) || !client.SetPortNumber(strPortInput))
            {
                Console.WriteLine(
                        string.Format("Wrong IP Address or port number supplied - {0} - {1}",
                        strIPAddress, strPortInput)
                    );
                Console.WriteLine("Press a key to exit...");
                Console.ReadKey();
                return;
            }

            client.ConnectToServer();

            string strInputUser = null;

            do
            {
                strInputUser = Console.ReadLine();
                // here we are implementing logic to send something back to server
                if (strInputUser.Trim() != "<EXIT>")
                {
                    client.SendToServer(strInputUser);
                }
                // we will close and disconnect from server
                else if (strInputUser.Equals("<EXIT>"))
                {
                    client.CloseAndDisconnect();
                }
                
            } while (strInputUser != "<EXIT>");
        }
    }
}

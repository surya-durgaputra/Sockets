﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SocketAsync
{
    public class SocketServer
    {
        //Event and its handler
        //EventHandler is a delegate provided by the .NET framework to simplify the creation of custom events. 
        //By using EventHandler you remove the requirement to create your own delegate to support your new event. 
        //The EventHandler delegate defines two parameters (object sender, EventArgs e), which match the standard event pattern.
        // http://www.blackwasp.co.uk/EventHandlerT.aspx
        //Using generic version of the EventHandler delegate EventHandler<T>
        // the following line says that the event ClientConnectedEvent can call any method that is pointed to by
        // EventHandler<ClientConnectedEventArgs> delegate
        // EventHandler<ClientConnectedEventArgs> excatly determines what method call signature will look like
        // (object sender, ClientConnectedEventArgs e) and the return type of these methods will be void
        public EventHandler<ClientConnectedEventArgs> ClientConnectedEvent;
        // now we declare the method to raise the above event
        // of course we will need to prepare and pass it the event arguments that will be sent to all its subscribers
        // we have declared it as protected virtual so that any derived classes can override the event invocation behavior
        protected virtual void RaiseClientConnectedEvent(ClientConnectedEventArgs e)
        {
            // within this method we are creating a temporary copy of the EventHandler object to avoid any race conditions
            EventHandler<ClientConnectedEventArgs> custom_event = ClientConnectedEvent;
            if(custom_event != null)
            {
                // here Object sender will point to custom_event which is an instance of ClientConnectedEvent event
                custom_event(this, e);
            }
        }
        // ServerTextReceivedEvent implementation
        public EventHandler<TextReceivedEventArgs> ServerTextReceivedEvent;
        protected virtual void RaiseServerTextReceivedEvent(TextReceivedEventArgs e)
        {
            // within this method we are creating a temporary copy of the EventHandler object to avoid any race conditions
            EventHandler<TextReceivedEventArgs> custom_event = ServerTextReceivedEvent;
            if (custom_event != null)
            {
                // here Object sender will point to custom_event which is an instance of ClientConnectedEvent event
                custom_event(this, e);
            }
        }

        // since we know that we need to supply an IPAddress and port number
        IPAddress mIP;
        int mPort;

        public bool KeepRunning { get; set; }

        // lets also get TcpListener instance. This is a helper class that allows
        // us to create servers in easy fashion. We will actually use async
        // methods provided by this class
        TcpListener mTcpListener;

        // we will create a list of TCP Clients that are connected to the server
        // when a client connects, we add it to this list
        // when a client disconnects, we remove it from this list
        List<TcpClient> mClients;


        

        public SocketServer()
        {
            mClients = new List<TcpClient>();
        }

        // now we will create a method for listener
        public async void StartListeningForIncomingConnection(IPAddress ipaddr=null, int port = 23000)
        {
            //some sanity check code
            if (ipaddr == null)
            {
                ipaddr = IPAddress.Any;
            }
            if (port <= 0)
            {
                port = 23000;
            }

            //lets store the user supplied IPAddress and port number in instance variables
            mIP = ipaddr;
            mPort = port;

            //some disgnostics
            System.Diagnostics.Debug.WriteLine(string.Format("IP Address: {0} - Port: {1}", mIP.ToString(), mPort));

            mTcpListener = new TcpListener(mIP, mPort);
            try
            {
                mTcpListener.Start();

                // following is to continuously accept client connections
                KeepRunning = true;
                while (KeepRunning)
                {

                    // in order to accept incoming connections, we need to call another method AcceptTcpClientAsync
                    // this method returns a TcpClient object. TcpClient is yet another helper class that allows us to 
                    // handle client sockets in an easy fashion
                    var returnedByAccept = await mTcpListener.AcceptTcpClientAsync();

                    // add the new client to our list
                    mClients.Add(returnedByAccept);

                    Debug.WriteLine("Client connected successfully, count: {0} - {1}", mClients.Count, returnedByAccept.Client.RemoteEndPoint);

                    TakeCareOfTcpClient(returnedByAccept);

                    // now we will raise the event that a new client has connected
                    // prepare the event args
                    ClientConnectedEventArgs eaClientConnected = new ClientConnectedEventArgs(returnedByAccept.Client.RemoteEndPoint.ToString());
                    // now we raise the event
                    RaiseClientConnectedEvent(eaClientConnected);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
            
        }

        // in this method we will perform the asyc read operation
        private async void TakeCareOfTcpClient(TcpClient paramClient)
        {
            // Every TCP-IP socket or TcpClient has got a network Stream attached to it, just like the stream available in case of console-IO or File-IO.
            // we use the streamreader object to read data from the network stream which is associated with the TCP Client that is passed into
            // this method as a parameter.
            NetworkStream stream = null;
            StreamReader reader = null;

            // lets also create a try catch block.
            // here we will assign the NetworkStream object a value, and the StreamReader object a value. Then we will be get data from the 
            // network stream.
            try
            {
                stream = paramClient.GetStream();
                reader = new StreamReader(stream);

                //in order to receive the data sent by the client, through the Streamreader, we will define an array of chars
                char[] buff = new char[64];
                while (KeepRunning)
                {
                    Debug.WriteLine("***Ready to read***");
                    int nRet = await reader.ReadAsync(buff, 0, buff.Length);
                    // note: reader.ReadAsync returns 0, it means that the socket connection has been closed
                    Debug.WriteLine("Returned: " + nRet);

                    if(nRet == 0)
                    {
                        Debug.WriteLine("Socket disconnected");
                        // remove the disconnected client from the list
                        RemoveClient(paramClient);
                        break;
                    }

                    // we need to convert the character array which was received through the streamreader, into a string
                    string receivedText = new string(buff);

                    Debug.WriteLine("***Received text: " + receivedText);

                    // before we start the next read operation, we also need to clear the byte array buffer which we used for 
                    // receiving data.
                    Array.Clear(buff, 0, buff.Length);

                    // we now raise the ServerTextReceivedEvent event
                    // to raise the event, we first prepare the event args
                    RaiseServerTextReceivedEvent(new TextReceivedEventArgs(
                        receivedText,
                        paramClient.Client.RemoteEndPoint.ToString()
                        ));
                }
            }
            catch (Exception e)
            {
                RemoveClient(paramClient);
                Debug.WriteLine(e.ToString());
            }
        }

        private void RemoveClient(TcpClient paramClient)
        {
            if (mClients.Contains(paramClient))
            {
                mClients.Remove(paramClient);
                Debug.WriteLine(String.Format("Client removed, count: {0}", mClients.Count));
            }
        }

        // just a demo message to show how to send a message to all connected clients in an asynchronous fashion
        public async void SendToAll(string message)
        {
            if (string.IsNullOrEmpty(message))
            { 
                return;
            }
            try
            {
                // since the message will not chamge from client to client,
                // lets convert it to byte buffer only once
                byte[] buffMessage = Encoding.ASCII.GetBytes(message);
                foreach (TcpClient c in mClients)
                {
                    // since we dont need WriteAsync call to be awaited, no need to add await
                    await c.GetStream().WriteAsync(buffMessage,0,buffMessage.Length);

                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
        }

        public void StopServer()
        {
            try
            {
                if (mTcpListener != null)
                {
                    // note: calling Stop() will have side effects.
                    // we will get an exception in the AcceptTcpClientAsync() method of mTcpListener
                    // (inside StartListeningForIncomingConnection method)
                    // that exception will get handled by the catch block
                    // and we will see its trace on the Debug log
                    mTcpListener.Stop();
                }
                foreach (TcpClient c in mClients)
                {
                    // when Close() is called, there will be side effects
                    // an exception will occur in each of the connected client
                    // this will occur in TakeCareOfTcpClient() method
                    // the catch block will first call RemoveClient() method
                    // and then show the trace on the Debug log
                    c.Close();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
        }
    }
}

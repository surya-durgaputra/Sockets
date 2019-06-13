# Sockets
Cheatsheet:

Host: Any computer attached to a network is a host.

Host (in a network) is uniquely identified by Internet Protocol (IP) address

Hostname is translated into an IP address by DNS (Domain Name Server).

IPv4 address is a group of four 8 bit numbers written in a dotted decimal notation like 168.192.1.5

Ports: Every computer has 65536 ports. Think of a computer as an apartment building with ports as apartments. Each port has a unique numeric id called port number. Port numbers from 0 to 1023 are reserved for OS usage. These are called Well-Known Ports or System Ports.

A single port can be used to send or receive data by only one process at any given time.

When we need to send/receive data to/from a process running on another computer, we need to know the IP Address of the remote peer and the port number being used by the peer software process.

Endpoint: Combination of IP_Address and Port is called an Endpoint. 192.168.1.169:5 is an endpoint and 5 is the port number, rest being the IP Address (excluding :).
When a process running on a computer A in a network needs to access another process running on computer B on the same network, it needs to access it using Endpoint B.

# Client Server Model
Client and Server are two processes that can be running on the same box/virtual-machine or on two different machines connected by a network. Two separate threads in the same process can also use sockets to share data.

### Steps involed in Client Server model functioning:
1. Server must start first
2. Server must perform an "Accept Connections" operations.
	- To perform the operation, the server will use a specific IP address and a port number like 192.169.5.2:20000 (also called Endpoint)
3. After the server process has started and is accepting connections, a client process will start and will try to connect to the server.
	- In order for the client to connect to the server, the client will need to know the IP address and the port number (server endpoint) on which the server is listening for incoming connections or accepting incoming connections.
	- If the connection attempt is successful, the client and the server will be able to communicate (read and write any amount of data to and fro) for any amount of time until either one of the participants decides to close the connection or the network connection goes offline.
	- If the connection attempt is unsuccessful:
		* an exception will occur in the client process.
		* the connection attempts can fail for various reasons:
			- Windows firewall has blocked either the client process or the server process
			- The IP address supplied to the client is incorrect.
			- The port number supplied to the client process as server port number is wrong.
			- The port number supplied to the server process as port number to start accepting connections is already occupied by another process.
			- The server process was not started at all.
			- The server process was started but crashed.
	- After client-server connection has been established, **if one end goes offline, the other end will receive an exception.**
	- **Remember that TCP/IP is a stream based protocol. Content that are sent through many SendData operations might get read with only one ReadData operation. And it could be jumbled data.**
		- Typically, "\n" is used as a message delimiter to handle this problem in production systems.
4. You can enable Telnet client and Telnet server on your windows machine via the control panel.
	- You can run telnet client by opening the command prompt and typing telnet. You will get a prompt "Microsoft Telnet". This means that telnet client is running successfully on your computer. To close telnet client, just click the close button on the prompt window.
	- Telnet utility is useful as it can be used as the other side. For example if we have are coding a server, we can test its working by having telnet-client turned on. This way we will not also have to implement a client to just test the server. 
	
_______________________________________________________________________________________________________________________________________________________________________________________

# Client and Server side Socket Programming
- Client side socket program is very similar to the server side except that client side sockets dont call the bind, listen and accept method calls. Instead the client side programs call the connect method on the socket. The socket object is the same. It is just the difference of method calls.
- Client side processes need to know the exact server process endpoint. 
- **Stopping the Server**
	* Prevent TcpListener from accepting new connections
	* Disconnect the client sockets (TcpClient)
	* Write StopServer method
_______________________________________________________________________________________________________________________________________________________________________________________

# Events: Publisher-Subscriber model
- Server/Client Events:
	* Server Client Connected
	* Server Text Received
	* Client Text Received
	* Client Disconnected (in server)
	* Server Disconnected (in client)
	* Publisher Subscriber Model: 
		+ This approach of raising and handling events is known as **Publisher-Subscriber** model. Our socket library (SocketAsync consisting of SocketClient and SocketServer) will act as event publisher. The applications using this library will act as subscribers.
		+ When the Events are raised, they will trigger some code in the Subscriber class and that code will do the business logic with the info attached with the Event.
- steps:
	+ Publisher Side:
		1. Create the custom EventArgs class to define any custom eventArgs that are passed with the event invocation
		2. Declare the delegate in the library that is going to raise the event (public EventHandler<ClientConnectedEventArgs> ClientConnectedEvent;)
		3. Usually in the same class as of point 2, add the code to raise the event (you will need to create the event args before raising the event)
	+ Subscriber Side:
		1. Implement the method to be called when the event is raised
		2. hook the method in 1 to the Event exposed by the instance of class where Publisher was implemented(client.ClientTextReceivedEvent += HandleClientTextReceived;) 
		
_______________________________________________________________________________________________________________________________________________________________________________________
# Role of DHCP and DNS
	- **DHCP**
		- Dynamic Host Control Protocol
		- Usually built into every home router
		- Its job is to assign IP addresses to hosts dynamically
	- **DNS - Domain Name Server**
		- Once the DHCP server assign the IP addresses to the hosts, the job of DNS starts
		- DNS keeps track of IP Addresses of all hosts on the network
		- DNS can be used to look up the IP Address of any host on the network using the hostname
		- DNS is built into every home router
		- .NET class System.Net.Dns
		- **In a production system, we should assume that hosts will be assigned IP dynamically by the DHCP and hence always use the hostname and then map the hostname to the IP address using methods exposed by the System.Net.Dns class**
		- 
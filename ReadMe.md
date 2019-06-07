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
using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace DedicatedServer
{
    public class Server
    {
        public static int _playersCount { get; private set; }

        public static int _port { get; private set; }
        public static Dictionary<int, Client> _clientDictionary = new Dictionary<int, Client>();
        private static TcpListener _tcpListener;

        public delegate void PacketHandler(int clientID, Packet message);
        public static Dictionary<int, PacketHandler> _packetHandlerDictionary;

        public static MessageListManager messageListManager = new MessageListManager(20);


        public static void TCPConnectCallback(IAsyncResult AsyncResult)
        {
            TcpClient client = _tcpListener.EndAcceptTcpClient(AsyncResult);
            string playerIP = client.Client.RemoteEndPoint.ToString().Split(':')[0];
            _tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);
            Console.WriteLine($"Incoming connection: {client.Client.RemoteEndPoint}");


            for (int i = 0; i < _playersCount; i++)
            {
                if (playerIP == _clientDictionary[i]._ip)
                {
                    _clientDictionary[i].tcp.Connect(client);
                    Console.WriteLine($"{client.Client.RemoteEndPoint} existed player successfully reconnected");
                    return;
                }
            }

            /// Write new Player
            _clientDictionary.Add(_playersCount, new Client(_playersCount, playerIP));
            _clientDictionary[_playersCount].tcp.Connect(client);
            Console.WriteLine($"{client.Client.RemoteEndPoint} successfully connected");
            _playersCount++;

        }

        public static void StartServer(int port)
        {
            _playersCount = 0;
            _port = port;

            _tcpListener = new TcpListener(IPAddress.Any, _port);
            _tcpListener.Start();
            _tcpListener.BeginAcceptSocket(new AsyncCallback(TCPConnectCallback), null);
            Console.WriteLine($"Server started on: {IPAddress.Any} : {_port}.");
            InitServerData();

        }

        private static void InitServerData()
        {
            _packetHandlerDictionary = new Dictionary<int, PacketHandler>()
            {
                {
                    (int) ClientPackets.welcomeReceived , ServerReceiveManager.WelcomeReceivedFromPlayer
                },
                {
                    (int) ClientPackets.messageReceived, ServerReceiveManager.ReceiveChatMessage
                }
            };

            Console.WriteLine("Finished Init Server data");
        }

    }

}

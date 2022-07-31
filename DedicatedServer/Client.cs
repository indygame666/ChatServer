using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace DedicatedServer
{
    public class Client
    {
        /// tcp static data
        public static int dataBufferSize = 4096;

        /// client Properties
        public int _id { get; private set; }
        public string _ip { get; private set; }
        public int _colorID { get; private set; }
        public string _nickname { get; private set; }
        public string _status { get; private set; }

        public TCP tcp;
        public Client(int id, string ip)
        {
            _id = id;
            _ip = ip;
            tcp = new TCP(id);
        }

        public void SetNickName(string nickname)
        {
            if (_nickname != nickname)
            {
                _nickname = nickname;
                ServerSendManager.SendServerMessage($"Player with ID:{_id} changed his name to {nickname}");
            }
        }

        public void SetColorId(int id)
        {
            _colorID = id;
        }

        public void SetStatus(string status) 
        {
            _status = status;
        }

        public class TCP 
        {
            public TcpClient _clientSocket;
            private int _id;
            private byte[] _buffer;
            private NetworkStream _networkStream;
            private Packet packetData;

            public TCP(int id)
            {
                _id = id;
            }

            public void Connect(TcpClient socket)
            {
                _clientSocket = socket;
                _clientSocket.ReceiveBufferSize = dataBufferSize;
                _clientSocket.SendBufferSize = dataBufferSize;

                _networkStream = _clientSocket.GetStream();

                packetData = new Packet();
                _buffer = new byte[dataBufferSize];

                _networkStream.BeginRead(_buffer,0,dataBufferSize,RecieveCallback,null);

                ServerSendManager.WelcomeMessage(_id, "Welcome to the server!");
            }

            private void RecieveCallback(IAsyncResult result)
            {
                try
                {
                    int byteLength = _networkStream.EndRead(result);
                    if (byteLength <= 0)
                    {
                        Server._clientDictionary[_id].Disconnect();
                        return;
                    }
                    byte[] data = new byte [byteLength];
                    Array.Copy(_buffer, data, byteLength);

                    packetData.Reset(HandleData(data));

                    _networkStream.BeginRead(_buffer,0,dataBufferSize,RecieveCallback,null); 

                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error recieving TCP data: {e}");
                }
            }

            private bool HandleData(byte[] data)
            {
                int _packetLength = 0;

                packetData.SetBytes(data);

                if (packetData.UnreadLength() >= 4)
                {
                    _packetLength = packetData.ReadInt();
                    if (_packetLength <= 0)
                    {
                        return true;
                    }
                }

                while (_packetLength > 0 && _packetLength <= packetData.UnreadLength())
                {
                    byte[] _packetBytes = packetData.ReadBytes(_packetLength);
                    ThreadManager.ExecuteOnMainThread(() =>
                    {
                        using (Packet _packet = new Packet(_packetBytes))
                        {
                            int _packetId = _packet.ReadInt();
                            Server._packetHandlerDictionary[_packetId](_id,_packet);
                        }
                    });

                    _packetLength = 0;
                    if (packetData.UnreadLength() >= 4)
                    {
                        _packetLength = packetData.ReadInt();
                        if (_packetLength <= 0)
                        {
                            return true;
                        }
                    }
                }

                if (_packetLength <= 1)
                {
                    return true;
                }

                return false;
            }

            public void SendData(Packet packet)
            {
                try
                {
                    if (_clientSocket != null)
                    {
                        _networkStream.BeginWrite(packet.ToArray(), 0, packet.Length(), null, null);
                    }
                }catch (Exception e)
                {
                    Console.WriteLine($"Smth went wrong. Can't send the data...");
                }
            }

            public void Disconnect()
            {
                _clientSocket.Close();
                _networkStream = null;
                _buffer = null;
                packetData = null;
                _clientSocket = null;
            }
        }

        public void Disconnect()
        {
            Console.WriteLine($"{tcp._clientSocket.Client.RemoteEndPoint} has just disconnected...");
            tcp.Disconnect();
            ServerSendManager.SendServerMessage($"Player {_nickname} disconnected from the server");
            ServerSendManager.UpdateClientList(_id, _nickname, _colorID, "(offline)");
            
        }

    }
}
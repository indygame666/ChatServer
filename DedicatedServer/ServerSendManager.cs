using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DedicatedServer
{
    class ServerSendManager
    {
        public static void SendData(int clientID, Packet messagePacket)
        {
            messagePacket.WriteLength();
            Server._clientDictionary[clientID].tcp.SendData(messagePacket);
        }

        public static void SendDataToAll(Packet messagePacket)
        {
            messagePacket.WriteLength();
            for (int i = 0; i < Server._playersCount; i++)
            {
                Server._clientDictionary[i].tcp.SendData(messagePacket);
            }
        }

        public static void SendDataToAll(int ignoreID,Packet messagePacket)
        {
            messagePacket.WriteLength();
            for (int i = 0; i < Server._playersCount; i++)
            {
                if (i != ignoreID)
                {
                    Server._clientDictionary[i].tcp.SendData(messagePacket);
                }
            }
        }

        public static void WelcomeMessage(int clientID, string message)
        {
            using (Packet packet = new Packet((int)ServerPackets.welcome))
            {
                packet.Write(message);
                packet.Write(clientID);

                SendData(clientID, packet);

            }
        }

        public static void InitClientData(int clientID)
        {
            using (Packet packet = new Packet((int)ServerPackets.viewData))
            {
                packet.Write(Server.messageListManager._messageList.Count);
                packet.Write(Server.messageListManager._messageList);

                packet.Write(Server._clientDictionary.Count);
                packet.Write(Server._clientDictionary);

                SendData(clientID, packet);
            }
        }

        public static void UpdateClientList(int id, string nickname, int colorID, string status)
        {
            using (Packet packet = new Packet((int)ServerPackets.clientList))
            {
                packet.Write(id);
                packet.Write(nickname);
                packet.Write(colorID);
                packet.Write(status);

                SendDataToAll(id,packet);


            }
        }

        public static void SendServerMessage(string message)
        {
            using (Packet packet = new Packet((int)ServerPackets.serverMessage))
            {
                packet.Write(message);

                SendDataToAll(packet);
            }
        }

        /*    public static void UpdateChatList(int id, string nickname, int colorID, string status)
            {
                using (Packet packet = new Packet((int)ServerPackets.clientList))
                {
                    packet.Write(id);
                    packet.Write(nickname);
                    packet.Write(colorID);
                    packet.Write(status);

                    SendDataToAll(packet);
                }
            }*/
    }
}

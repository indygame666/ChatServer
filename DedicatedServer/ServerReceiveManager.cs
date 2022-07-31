using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DedicatedServer
{
    class ServerReceiveManager
    {
        public static void WelcomeReceivedFromPlayer(int clientID, Packet packet)
        {
            ///Read client data
            int id = packet.ReadInt();
            int colorID = packet.ReadInt();
            string nickname = packet.ReadString();

            Console.WriteLine($"{Server._clientDictionary[clientID].tcp._clientSocket.Client.RemoteEndPoint} connected as {clientID} ID player with colorID: {colorID} and nickname: {nickname}");

            ///send current relevant data to connected player 
            ServerSendManager.InitClientData(clientID);

            ///set players data (in case if player changed nickname or color while entering again)
            Server._clientDictionary[clientID].SetNickName(nickname);
            Server._clientDictionary[clientID].SetColorId(colorID);
            Server._clientDictionary[clientID].SetStatus("(online)");

            ////UpdatingClientList
            ServerSendManager.UpdateClientList(clientID, nickname, colorID, "(online)");

            ///Sending server notifications to all player
            ServerSendManager.SendServerMessage($"Player {nickname} connected to the server");

        }

        public static void ReceiveChatMessage(int clientID, Packet packet)
        {
            string nickName = packet.ReadString();
            int colorID = packet.ReadInt();
            string message = packet.ReadString();

            Console.WriteLine($"Incoming message from:{nickName}. ColorID: {colorID}. Text: {message}.");
            
            Message recievedMessage = new Message(nickName, message, colorID);
            Server.messageListManager.AddToQueue(recievedMessage);

           ///SendNewMessageToEveryone
           ServerSendManager.SendDataToAll(packet);
        }
    }
}

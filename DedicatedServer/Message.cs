using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DedicatedServer
{
    public class Message
    {
        public string _owner;
        public string _messageText;
        public int _colorID;

        public Message(string ownerID, string message, int colorID)
        {
            _owner = ownerID;
            _messageText = message;
            _colorID = colorID;
        }

    }
}

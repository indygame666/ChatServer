using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DedicatedServer
{
    public class  MessageListManager
    {
        public List<Message> _messageList;

        public int _maxCapacity;
        public MessageListManager(int capacity)
        {
            _maxCapacity = capacity;

            _messageList = new List<Message>();
        }

        public void AddToQueue(Message message)
        {

            if (_messageList.Count == _maxCapacity)
            {
                _messageList.RemoveAt(0);

                _messageList.Add(message);
            }
            else
            {
                _messageList.Add(message);
            }

        }


    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DedicatedServer
{
    public class Player
    {
        public int _id;
        public int _colorID;
        public string _nickname;

        public Player(int id, int colorID, string nickname)
        {
            _id = id;
            _colorID = colorID;
            _nickname = nickname;
        }
    }
}

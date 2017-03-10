using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestWechatGame.PKTEN
{
    public class PlayerManager
    {
        private Dictionary<ulong, Player> mPlayers = new Dictionary<ulong, Player>();

        public Player CreatePlayer(string name)
        {
            ulong max = 80000;
            if (mPlayers.Count > 0) {
                max = mPlayers.Keys.Max();
            }
            ulong id = max + 1;
            Player player = new Player();
            player.Name = name;
            player.ID = id;
            player.Score = 0;
            return player;
        }


        public Player GetPlayer(ulong id)
        {
            Player player = null;
            if (mPlayers.TryGetValue(id, out player)){
                return player;
            }
            return null;
        }

    }
}

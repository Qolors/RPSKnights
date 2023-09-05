using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueStatusBot.RPGEngine.Core.Engine.Beings
{
    public class Player : Being
    {
        public Player(Image<Rgba32> idleImage, ulong userId) : base(idleImage) 
        {
            this.userId = userId;
        }
        public int Health { get; set; } = 3;
        public bool IsAlive => Health > 0;
        private readonly ulong userId;
        public ulong GetUserId => userId;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueStatusBot.RPGEngine.Core.Engine.Beings
{
    public class Player : Being
    {
        public Player(Image<Rgba32> idleImage) : base(idleImage) { }
    }
}

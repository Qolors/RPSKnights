using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueStatusBot.RPGEngine.Core.Engine.Beings
{
    public class Being
    {
        public Image CurrentSprite { get; set; }

        public Being(Image idleSprite)
        {
            CurrentSprite = idleSprite;
        }
    }
}

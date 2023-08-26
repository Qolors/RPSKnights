﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueStatusBot.RPGEngine.Core.Engine
{
    public class Effect
    {
        public string Name { get; set; }
        public EffectType Type { get; set; }
        public bool IsActive => BufferDuration == 0;
        public string Description { get; set; }
        public int Duration { get; set; }
        public int BufferDuration { get; set; }
        public float ModifierAmount { get; set; }
    }
}

using LeagueStatusBot.RPGEngine.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueStatusBot.RPGEngine.Data.Repository
{
    public class MonsterRepository
    {
        private GameDbContext context;
        public MonsterRepository(GameDbContext context)
        {
            this.context = context;
        }


    }
}

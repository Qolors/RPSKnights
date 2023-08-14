using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueStatusBot.RPGEngine.Core.Engine
{
    public abstract class Being
    {
        public string Name { get; set; }
        public int Attack { get; set; }
        public int Defense { get; set; }
        public int HitPoints { get; set; }
        public Stats BaseStats { get; set; } = new Stats();
        public ulong DiscordId { get; set; }
        public int MaxHitPoints { get; set; }
        public bool IsHuman { get; set; } = false;
        public bool IsAlive => HitPoints > 0;
        public Being? Target { get; set; }

        public event EventHandler<string> ActionPerformed;
        public event EventHandler<string> DamageTaken;
        public event EventHandler Killed;

        public virtual void TakeDamage(int damage)
        {
            HitPoints -= damage;

            DamageTaken?.Invoke(this, $"- {Name} took {damage} damage. HP is Now ({this.HitPoints}/{this.MaxHitPoints})\n");

            if (!IsAlive) Killed?.Invoke(this, EventArgs.Empty);
        }

        public virtual void AttackTarget()
        {
            if (Target == null) return;

            int attackRoll = Dice.Roll();

            if (attackRoll > 10)
            {
                ActionPerformed?.Invoke(this, $"[{Name} ({HitPoints}/{MaxHitPoints})]: Attacked {Target.Name}!");
                Target.TakeDamage(1);
            }
            else
            {
                ActionPerformed?.Invoke(this, $"[{Name} ({HitPoints}/{MaxHitPoints})]: Missed their attack on {Target.Name}!\n");
            }
        }

        public virtual void SetTarget(Being target)
        {
            this.Target = target;
        }

    }
}

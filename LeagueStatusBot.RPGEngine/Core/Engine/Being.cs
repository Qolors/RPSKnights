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

            DamageTaken?.Invoke(this, $"{Name} took {damage} damage. HP is Now {this.HitPoints}");

            if (!IsAlive) Killed?.Invoke(this, EventArgs.Empty);
        }

        public virtual void AttackTarget()
        {
            if (Target == null) return;
            
            //Chosen Attack <-- We can Build a Skill System here

            ActionPerformed?.Invoke(this, $"{Name} attacked {Target.Name} for 1 damage.");

            Target.TakeDamage(1); // <-- We can Build a custom Modifier here
        }

        public virtual void SetTarget(Being target)
        {
            // --> WE CAN ADD A DEFENSE MODIFICATION SYSTEM HERE
            this.Target = target;
        }

    }
}

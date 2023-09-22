using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Interfaces
{
    public interface IEnemy
    {
        public int XPOnDeath { get; set; }

        public delegate void DeathEventHandler(IEnemy enemy);
        public static event DeathEventHandler Death;
        public void InvokeDeath(IEnemy enemy)
        {
            Death?.Invoke(enemy);
        }
    }
}

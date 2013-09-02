// DUBBELCHECKAD!


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace HeroBattleArena.Game.GameObjects
{
    public class Invisbuff: Buff
    {
        public Invisbuff(Unit target):
			base(target)
		{
		}

        public override void Activate()
		{
            LifeTime = Configuration.GetValue("Invisible_Time");
            Target.Hide();
            Target.Shadow = false;

            List<Buff> buffs = Target.Buffs;
            foreach (Buff buff in buffs)
            {
                if (buff is StickyGrenadeBuff)
                {
                    buff.Remove();
                }
            }
			
		}

        public override void Deactivate()
        {
            Target.Show();
            Target.Shadow = true;
        }
    }
}

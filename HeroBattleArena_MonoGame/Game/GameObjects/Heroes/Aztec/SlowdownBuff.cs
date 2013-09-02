using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace HeroBattleArena.Game.GameObjects
{
    class SlowdownBuff : Buff
    {
        float m_slowFactor = Configuration.GetValue("Slow_Down_Spear_SlowFactor");
        public SlowdownBuff(Unit target) :
			base(target)
		{
		}
        public override void Activate()
		{
            base.Activate();
            LifeTime = Configuration.GetValue("Slow_Down_Spear_Duration");
		}
        public override void Update(float delta)
        {
           base.Update(delta);
	       Target.MoveSpeed *= m_slowFactor;
        }
    }
}

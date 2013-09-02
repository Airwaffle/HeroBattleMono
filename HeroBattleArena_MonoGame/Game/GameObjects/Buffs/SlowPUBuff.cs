using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HeroBattleArena.Game.GameObjects
{
    public class SlowPUBuff : Buff
    {
        private float m_TimeMultiplier;

        public SlowPUBuff(Unit target)
            : base(target)
        { }

        public override void Activate()
        {
            base.Activate();

            LifeTime = Configuration.GetValue("Powerup_Slowdown_Duration");
            m_TimeMultiplier = Configuration.GetValue("Powerup_Slowdown_Multiplier");
            GameMode.Instance.SafeTimePlayer = Target as Hero;
            GameMode.Instance.TimeModifier = m_TimeMultiplier;
        }

        public override void Deactivate()
        {
            base.Deactivate();

            GameMode.Instance.TimeModifier = 1;
            
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HeroBattleArena.Game.GameObjects
{
    public class VampyricDrain : Buff
    {
        float m_DrainRate = 0.0f;
        Unit m_Vampire = null;
        public VampyricDrain(Unit target, float lifetime, float drainRate, Unit vampire) :
			base(target)
		{
            LifeTime = lifetime;
            m_DrainRate = drainRate;
            m_Vampire = vampire;
		}

        public override void Activate()
		{
            
            
		}

        public override void Update(float delta)
        {
            base.Update(delta);
            float amount = delta * m_DrainRate;
            if (Target.ReduceLife(amount, m_Vampire))
            {
                m_Vampire.Heal(amount);
            }
        }

        public override void Deactivate()
        {
            
        }
    }
}

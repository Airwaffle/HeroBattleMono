// DUBBELCHECKAD!

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace HeroBattleArena.Game.GameObjects
{
    public class StickyGrenadeBuff : Buff
    {
        StickyGrenadeProjectile m_grenade;
        bool m_removedFromGrenade = false;

        public StickyGrenadeBuff(StickyGrenadeProjectile grenade, Unit target) :
			base(target)
		{
            m_grenade = grenade;
		}

        public StickyGrenadeProjectile Grenade{get {return m_grenade;}}

        public void TellRemoved()
        {
            m_removedFromGrenade = true;
            Remove();
        }

        public override void Activate()
		{
            base.Activate();
            LifeTime = 1000.0f; 
            
		}
        public override void Deactivate()
        {
            base.Deactivate();
            if (!m_removedFromGrenade)
                m_grenade.Drop();
        }
    }
}

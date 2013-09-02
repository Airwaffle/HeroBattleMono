using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HeroBattleArena.Game.GameObjects
{
    public class DamagePUBuff : Buff
    {
        private float m_DamageMultiplier;

        public DamagePUBuff(Unit target)
            : base(target)
        { }

        public override void Activate()
        {
            base.Activate();

            LifeTime = Configuration.GetValue("Powerup_Damage_Duration");
            m_DamageMultiplier = Configuration.GetValue("Powerup_Damage_Multiplier");
            Effects.SpawnFollowing(Target, "damage_indicator", LifeTime);
        }

        public override void Update(float delta)
        {
            base.Update(delta);

            Target.AttackDamage *= m_DamageMultiplier;
        }
    }
}

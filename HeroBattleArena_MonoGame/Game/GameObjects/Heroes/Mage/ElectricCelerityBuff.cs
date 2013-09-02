using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HeroBattleArena.Game.GameObjects
{
    public class ElectricCelerityBuff : Buff
    {
        float m_speedFactor = Configuration.GetValue("ElectricCelerityBuff_SpeedFactor");

        public ElectricCelerityBuff(Unit target)
            : base(target)
        {
        }



        public override void Activate()
        {
            LifeTime = Configuration.GetValue("ElectricCelerityBuff_Duration");
            Target.IsSolid = false;
        }

        public override void Deactivate()
        {
            Unit target = Target;
            (target as Hero).HeroState = (HeroStates.IDLE);
            Target.IsSolid = true;

            List<IEntity> entities = EntityManager.Entities;
            for (int i = 0; i < entities.Count; i++)
            {
                if (entities[i] is ElectricCelerityHit)
                {
                    entities[i].Remove();
                }
            }
        }

        public override void Update(float delta)
        {
            base.Update(delta);

            Unit target = Target;
            Target.MoveSpeed = (Target.MoveSpeed * m_speedFactor);
        }
    }
}


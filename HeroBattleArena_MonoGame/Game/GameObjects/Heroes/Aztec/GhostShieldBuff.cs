using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HeroBattleArena.Game.GameObjects
{
    public class GhostShieldBuff: Buff
    {

        public GhostShieldBuff(Unit target):base(target){}


        public override void Activate()
        {
            LifeTime = Configuration.GetValue("GhostShieldHit_Life");
            if (EntityManager.AmountOfInstancesExisting<GhostShieldHit>() < 20)
            {
                GhostShieldHit hit0 = new GhostShieldHit(0, Target);
                GhostShieldHit hit1 = new GhostShieldHit(2.094f, Target);
                GhostShieldHit hit2 = new GhostShieldHit(4.189f, Target);
                EntityManager.Spawn(hit0);
                EntityManager.Spawn(hit1);
                EntityManager.Spawn(hit2);
            }
        }

        public override void Deactivate()
        {
            List<IEntity> entities = EntityManager.Entities;
	        for (int i = 0; i < entities.Count; i++)
	        {
		        if(entities[i] is GhostShieldHit)
		        {
		         entities[i].Remove();
		        }
	        }
        }
    }
}

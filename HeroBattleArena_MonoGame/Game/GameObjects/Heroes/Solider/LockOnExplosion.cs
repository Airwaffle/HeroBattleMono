using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace HeroBattleArena.Game.GameObjects
{
    public class LockOnExplosion : Projectile
    {
        float m_counterTilHitGround = 0.35f;
	    float m_exploDelay = Configuration.GetValue("Lock_On_Explosion_Delay");
	    float m_exploCounter = 0;
	    float m_exploDistance = Configuration.GetValue("Lock_On_Explosion_Distance");
	    int m_currentExplo = 0;
	    float m_aoe = Configuration.GetValue("Lock_On_Explosion_AOE");
	    Vector2[] exploPos = new Vector2[5];

        public LockOnExplosion(Unit owner, Vector2 direction)
            : base(owner, direction)
		{
		}
        public override void Initialize()
        {
            Name = "Lock On Explosion";
            exploPos[0] = new Vector2(- m_exploDistance, - m_exploDistance);
            exploPos[1] = new Vector2(m_exploDistance, - m_exploDistance);
            exploPos[2] = new Vector2(0, 0);
            exploPos[3] = new Vector2(m_exploDistance, - m_exploDistance);
            exploPos[4] = new Vector2(m_exploDistance, m_exploDistance);
            Hide();
	        base.Initialize();
	        Life = 100.0f;
	        Vector2 pos = Position;
	        Effects.Spawn(pos, "lock_bomb");
        }
        public override void Update(float delta)
        {
            base.Update(delta);
            m_counterTilHitGround -= delta;
	        if (m_counterTilHitGround < 0)
            {
		        m_exploCounter -= delta;
		        if (m_exploCounter <= 0)
                {
			        m_exploCounter = m_exploDelay;
			        Effects.Spawn(Position + exploPos[m_currentExplo], "Grenade_Explosion");
			        Vector2 ourPos = Position + exploPos[m_currentExplo];
        			
                    List<Unit> units = EntityManager.Units;
			        foreach (Unit unit in units)
			        {
				        Vector2 unitPos = unit.Position;
				        Vector2 dif = ourPos - unitPos;
				        if (dif.Length() < m_aoe){
					        unit.ApplyDamage(Damage, Owner);
				        }
			        }
			        m_currentExplo ++;
			        if (m_currentExplo > 4)
                    {
				        Life = 0.0f;
                    }
                    else if (m_currentExplo == 1)
                    {
                        ScreenBounce.Amount = 75;
                    }
		        }
	        }
        }
    }
}
    
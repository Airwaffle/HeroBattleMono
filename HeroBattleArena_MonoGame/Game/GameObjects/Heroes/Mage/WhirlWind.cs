using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HeroBattleArena.Game.GameObjects
{
    public class WhirlWind : Ability
    {
     
    private float m_radiusSq2 = Configuration.GetValue("Whirl_Wind_Radius");
	private float m_damage = 0;


    public WhirlWind(Hero owner): base(owner)
{
}


public override void Initialize()
{
	Cooldown = Configuration.GetValue("Whirl_Wind_CoolDown");
	ManaCost = Configuration.GetValue("Whirl_Wind_ManaCost");
    Icon = Graphics.GetTexture("gui_whirlwind_icon");
	
	m_radiusSq2 *= m_radiusSq2;

	
}


public override bool Activate()
{
	float WhirlWindTime = Configuration.GetValue("Whirl_Wind_Duration");
    float WhirlWindDmg = Configuration.GetValue("Whirl_Wind_Damage");

	
	if(base.Activate())
	{
		SoundCenter.Instance.Play(SoundNames.MageWhirlWind);

		Vector2 playerPos = Owner.Position;
        List<Unit> units = EntityManager.Units;
		Hero ownerAsHero = Owner as Hero;
		ownerAsHero.HeroState = HeroStates.SPECIAL3;
        ScreenBounce.Amount = 100;

        Input.SetAllRumble(1);
		

		for (int i = 0; i < units.Count; i++)
		{
			if(units[i] == Owner) continue;
			Vector2 dif = units[i].Position - playerPos;
			
			if(Vector2.Dot(dif, dif) < m_radiusSq2){				
				WhirlWindBuff buff = new WhirlWindBuff(units[i]);
                dif.Normalize();
                buff.DirectionVector = dif;
				units[i].AddBuff(buff);
				m_damage = WhirlWindDmg;
				units[i].ApplyDamage(m_damage, ((Unit)Owner));
			}

		}

		List<IEntity> entities = EntityManager.Entities;

		for (int i = 0; i < entities.Count; i++)
		{
			if((entities[i]) is LaserBladeProjectile)
			{
				entities[i].Remove();			
			}
		}
		
		return true;
	}
	return false;
}

    }
}

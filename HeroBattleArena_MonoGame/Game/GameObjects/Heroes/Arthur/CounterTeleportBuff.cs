using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace HeroBattleArena.Game.GameObjects
{
	public class CounterTeleportBuff : Buff
	{
		Unit m_Victim = null;
		float m_StunTime;

		public CounterTeleportBuff(Unit target):
			base(target)
		{
		}

		public override void Activate()
		{
			m_StunTime = Configuration.GetValue("Arthur_Counter_Buff_Stuntime");
			LifeTime = Configuration.GetValue("Arthur_Counter_Buff_Duration");
			(Target as Hero).HeroState = HeroStates.SPECIAL1;
			Target.SetStun(LifeTime);
			Target.SetInvincibility(LifeTime);
		}

		public override void Deactivate()
		{
			if (m_Victim != null)
			{
                SoundCenter.Instance.Play(SoundNames.ArthurCounterTeleportEnd);
				Target.Show();
				((Hero)Target).HeroState = HeroStates.SPECIAL6;
				Target.RemoveStun();
				
				Vector2 newPosition = new Vector2(m_Victim.Position.X, m_Victim.Position.Y);
				Target.ChangeDirection(m_Victim.AnimationDirection);

				newPosition -= m_Victim.Direction * Configuration.GetValue("Arthur_Counter_Teleport_Distance");
				m_Victim.SetStun(Configuration.GetValue("Arthur_Counter_Appear_Animation_Duration") + Configuration.GetValue("Arthur_Counter_Additional_Stun"));
				Target.Position = newPosition;
				Target.ChangeAnimation((int)Target.AnimationDirection);
			} 
			else 
			{
				(Target as Hero).HeroState = HeroStates.IDLE;
				Target.SetStun(m_StunTime);
				Target.RemoveInvincibility();
				Target.ChangeDirection(Directions.Down);
				Target.Animations.ChangeAnimation((int)HeroStates.IDLE* 4 + (int)Directions.Down);
			}
		}

		public override void OnTargetCollide(AABB aabb)
		{
			if (m_Victim != null) return;

			if(aabb.Owner is StickyGrenadeProjectile)
			{
				if(((Projectile)aabb.Owner).Owner is Unit)
				{
					List<Buff> buffs = Target.Buffs;
					for(int i = 0; i < buffs.Count; ++i)
					{
						if(buffs[i] is StickyGrenadeBuff)
						{
#warning Följande rad var bortkommenterad i C++-versionen, men jag undrar varför. Ville vi inte ha detta systemet med skyddade enheter?
                            (buffs[i] as StickyGrenadeBuff).Grenade.ProtectedUnit = Target;
							buffs[i].Remove(); 
							m_Victim = null;
						}
					}
					
				}	
				return;
			}

			if(aabb.Owner is Projectile)
			{
				if(((Projectile)aabb.Owner).Owner is Unit)
				{			
					LifeTime = Configuration.GetValue("Arthur_Counter_Time_Invicible") + Configuration.GetValue("Arthur_Counter_Disappear_Animation_Duration");
					m_Victim = ((Projectile)aabb.Owner).Owner;
					((Hero)Target).HeroState = HeroStates.SPECIAL4;

					List<Buff> buffs = Target.Buffs;
					for(int i = 0; i < buffs.Count; ++i)
					{
						if(buffs[i] is StickyGrenadeBuff)
                        {
#warning Följande rad var bortkommenterad i C++-versionen, men jag undrar varför. Ville vi inte ha detta systemet med skyddade enheter?
                            (buffs[i] as StickyGrenadeBuff).Grenade.ProtectedUnit = Target;
							buffs[i].Remove(); 

							m_Victim = null;
						}
					}
				}
			}

			if(aabb.Owner is LaserBladeProjectile)
			{
				if(((LaserBladeProjectile)aabb.Owner).Owner is Unit)
				{
					LifeTime = Configuration.GetValue("Arthur_Counter_Time_Invicible")+ Configuration.GetValue("Arthur_Counter_Disappear_Animation_Duration");
					m_Victim = ((LaserBladeProjectile)aabb.Owner).Owner;
					((Hero)Target).HeroState = HeroStates.SPECIAL4;
				}
			}
			if(aabb.Owner is HitObject)
			{
				if(((HitObject)aabb.Owner).Owner is Unit)
				{
					LifeTime = Configuration.GetValue("Arthur_Counter_Time_Invicible") + Configuration.GetValue("Arthur_Counter_Disappear_Animation_Duration");
					m_Victim = ((HitObject)aabb.Owner).Owner;
					((Hero)Target).HeroState = HeroStates.SPECIAL4;
				}
			}
		}

		public override void Update(float delta)
		{
			base.Update(delta);
		}
	}
}

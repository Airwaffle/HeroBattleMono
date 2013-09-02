using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HeroBattleArena.Game.GameObjects
{
    public class SilenceShine : Ability
    {
        private float m_RadiusSquared;
        private float m_SilenceTime;

        public SilenceShine(Hero owner):
            base(owner)
        {
        }

        public override void Initialize()
        {
            base.Initialize();

            Icon = Graphics.GetTexture("gui_silence_icon");

            Cooldown = Configuration.GetValue("Silence_Shine_CoolDown");
            ManaCost = Configuration.GetValue("Silence_Shine_ManaCost");

	        m_RadiusSquared = Configuration.GetValue("Silence_Shine_Radius");
	        m_RadiusSquared *= m_RadiusSquared;

            m_SilenceTime = Configuration.GetValue("Silence_Shine_Duration");
        }

        public override bool Activate()
        {
            if (base.Activate())
            {
				SoundCenter.Instance.Play(SoundNames.ArthurSilenceShine);
                Effects.SpawnFollowing(Owner, "Silence_Effect", 0.0f);

                Owner.DispellBuffs();

		        Vector2 playerPos = Owner.Position;
                List<Hero> heroes = EntityManager.Heroes;

                foreach (Hero hero in heroes)
                {
                    if (hero == Owner) continue;
                    Vector2 dif = hero.Position - playerPos;
                    
                    if (Vector2.Dot(dif, dif) < m_RadiusSquared)
                    {
                        hero.SetSilence(m_SilenceTime);
						Console.WriteLine("Silenced for: " + m_SilenceTime);
                        Effects.SpawnFollowing(hero, "Silenced_Effect", m_SilenceTime);

                        List<Buff> buffs = hero.Buffs;
                        foreach (Buff buff in buffs)
                        {
                            if (buff is Invisbuff)
                                buff.Remove();
                            else if (buff is ElectricCelerityBuff)
                                buff.Remove();
							else if (buff is GhostShieldBuff)
                                buff.Remove();
                        }
                    }
                }

                List<IEntity> entities = EntityManager.Entities;
                foreach (IEntity entity in entities)
                {
                    if (entity is FlurryAttack)
                    {
                        (entity as FlurryAttack).RemoveNext();
                    }
                }
		        return true;
            }
            return false;
        }
    }
}

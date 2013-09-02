// DUBBLECHECKAD!

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
namespace HeroBattleArena.Game.GameObjects
{
    public class Flashbang : Ability
    {
        private float m_stunRange = Configuration.GetValue("Invisible_Stun_Range");
        private float m_stunTime = Configuration.GetValue("Invisible_Stun_Time");

        public Flashbang(Hero owner) :
			base(owner)
		{
		}
        public override void Initialize()
        {
            base.Initialize();
            Icon = Graphics.GetTexture("gui_flashbang_icon");
            Cooldown = Configuration.GetValue("Invisible_CoolDown");
            ManaCost = Configuration.GetValue("Invisible_ManaCost");
        }
        public override bool Activate()
        {
            if (base.Activate())
            {
				SoundCenter.Instance.Play(SoundNames.SoldierFlashbang);
                Owner.HeroState = HeroStates.SPECIAL3;

				Effects.Spawn(Owner.Position, "flash_bang_effect");
                List<Unit> units = EntityManager.Units;

                foreach(Unit unit in units){
                    if (unit != Owner)
                    {
                        Vector2 distance = Owner.Position - unit.Position;
                        if (distance.Length() < m_stunRange)
                        {
                            unit.SetStun(m_stunTime);
                            Effects.SpawnFollowing(unit, "Stunned_Effect", m_stunTime);
                            List<Buff> buffs = unit.Buffs;
				            foreach(Buff buff in buffs)
					        {
								if (buff is ElectricCelerityBuff)
									buff.Remove();
				            }
                        }
                    }
                }
                return true;
            }
            return false;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HeroBattleArena.Game.GameObjects
{
    public class LaserBlade : Ability
    {
        public LaserBlade(Hero hero) :
            base(hero)
        {
        }

        public override void Initialize()
        {
            base.Initialize();

            Icon = Graphics.GetTexture("gui_laserblade_icon");

            Cooldown = Configuration.GetValue("Laser_Sword_Cooldown");
            ManaCost = Configuration.GetValue("Laser_Sword_ManaCost");
        }

        public override bool Activate()
        {
            if (base.Activate())
            {
                SoundCenter.Instance.Play(SoundNames.ArthurLaserBlade);
                LaserBladeProjectile laser = new LaserBladeProjectile(Owner, Owner.Direction, this);
				laser.Position = Owner.Position;
		        EntityManager.Spawn(laser);

				Owner.CanAttack = false;
				Owner.HeroState = HeroStates.SPECIAL8;

                return true;
            }
            return false;
        }
    }
}

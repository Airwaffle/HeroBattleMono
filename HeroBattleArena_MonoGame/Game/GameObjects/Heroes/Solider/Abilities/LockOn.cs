using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace HeroBattleArena.Game.GameObjects
{
    public class LockOn : Ability
    {
        public LockOn(Hero owner) :
			base(owner)
		{
		}
        public override void Initialize()
        {
            base.Initialize();
            Icon = Graphics.GetTexture("gui_lockon_icon");
            Cooldown = Configuration.GetValue("Lock_On_CoolDown");
            ManaCost = Configuration.GetValue("Lock_On_ManaCost");
            
        }
        public override bool Activate()
        {
            if (base.Activate())
            {
				SoundCenter.Instance.Play(SoundNames.SoldierLockOn);
		        Owner.CanAttack = false;
                Owner.IsStrafing = true;
                Owner.HeroState = HeroStates.SPECIAL5;

                LockOnLaser laser = new LockOnLaser(Owner, Owner.Direction);
                laser.Position = Owner.Position; 
                laser.Ability = this;
                EntityManager.Spawn(laser);
                return true;
            }
            return false;
        }
    }
}
